# deploy.ps1
param(
    [string]$Namespace = "plataforma-educacao",
    [string]$ClusterName = "plataforma-educacao",
    [string]$K8sPath = "k8s"
)

$ErrorActionPreference = "Stop"

# Cabeçalho
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Deploy - Plataforma Educacao DevOps" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Namespace: $Namespace" -ForegroundColor Yellow
Write-Host "Cluster: $ClusterName" -ForegroundColor Yellow
Write-Host ""

# Verificar kubectl
Write-Host "Verificando dependencias..." -ForegroundColor Yellow
if (!(Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: kubectl nao encontrado" -ForegroundColor Red
    Write-Host "Instale: https://kubernetes.io/docs/tasks/tools/" -ForegroundColor Yellow
    exit 1
}
Write-Host "OK: kubectl encontrado" -ForegroundColor Green

# Verificar cluster
Write-Host "Verificando conexao com cluster..." -ForegroundColor Yellow
try {
    kubectl cluster-info 2>&1 | Out-Null
    Write-Host "OK: Cluster Kubernetes conectado" -ForegroundColor Green
} catch {
    Write-Host "AVISO: Nenhum cluster Kubernetes encontrado." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Para criar um cluster local com Kind:" -ForegroundColor Yellow
    Write-Host "  kind create cluster --name $ClusterName" -ForegroundColor Green
    Write-Host ""
    Write-Host "Ou use um cluster existente:" -ForegroundColor Yellow
    Write-Host "  kubectl config use-context 'seu-contexto'" -ForegroundColor Green
    exit 1
}

# Início do deploy
Write-Host ""
Write-Host "Iniciando deploy..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Namespace
Write-Host "[1/8] Aplicando Namespace..." -ForegroundColor Cyan
kubectl apply -f "$K8sPath/namespace.yaml"
Write-Host ""

# Step 2: Secrets
Write-Host "[2/8] Aplicando Secrets..." -ForegroundColor Cyan
kubectl apply -f "$K8sPath/secret.yaml"
Write-Host ""

# Step 3: ConfigMap
Write-Host "[3/8] Aplicando ConfigMap..." -ForegroundColor Cyan
kubectl apply -f "$K8sPath/configmap.yaml"
Write-Host ""

# Step 4: Infraestrutura
Write-Host "[4/8] Aplicando infraestrutura (SQL Server + RabbitMQ)..." -ForegroundColor Cyan
kubectl apply -f "$K8sPath/sqlserver.yaml"
kubectl apply -f "$K8sPath/rabbitmq.yaml"
Write-Host ""

# Step 5: Aguardar infraestrutura
Write-Host "[5/8] Aguardando infraestrutura ficar pronta..." -ForegroundColor Cyan
Write-Host "Aguardando SQL Server..." -ForegroundColor Yellow
try {
    kubectl wait --for=condition=ready pod -l app=sqlserver -n $Namespace --timeout=300s 2>&1 | Out-Null
    Write-Host "OK: SQL Server pronto" -ForegroundColor Green
} catch {
    Write-Host "AVISO: SQL Server nao ficou pronto no tempo limite" -ForegroundColor Yellow
}

Write-Host "Aguardando RabbitMQ..." -ForegroundColor Yellow
try {
    kubectl wait --for=condition=ready pod -l app=rabbitmq -n $Namespace --timeout=120s 2>&1 | Out-Null
    Write-Host "OK: RabbitMQ pronto" -ForegroundColor Green
} catch {
    Write-Host "AVISO: RabbitMQ nao ficou pronto no tempo limite" -ForegroundColor Yellow
}
Write-Host ""

# Step 6: Deployments
Write-Host "[6/8] Aplicando deployments de aplicacao..." -ForegroundColor Cyan
kubectl apply -f "$K8sPath/gestao-identidade.yaml"
kubectl apply -f "$K8sPath/gestao-conteudo.yaml"
kubectl apply -f "$K8sPath/gestao-aluno.yaml"
kubectl apply -f "$K8sPath/gestao-financeira.yaml"
kubectl apply -f "$K8sPath/bff-api.yaml"
Write-Host ""

# Step 7: Observabilidade
Write-Host "[7/8] Aplicando observabilidade (Prometheus + Grafana)..." -ForegroundColor Cyan
if (Test-Path "$K8sPath/observability/") {
    kubectl apply -f "$K8sPath/observability/"
    Write-Host "OK: Observabilidade aplicada" -ForegroundColor Green
} else {
    Write-Host "AVISO: Pasta observability/ nao encontrada, pulando..." -ForegroundColor Yellow
}
Write-Host ""

# Step 8: Ingress e HPA
Write-Host "[8/8] Aplicando Ingress + HPA..." -ForegroundColor Cyan
Write-Host "Verificando Ingress Controller..." -ForegroundColor Yellow
try {
    kubectl wait --namespace ingress-nginx --for=condition=ready pod --selector=app.kubernetes.io/component=controller --timeout=120s 2>&1 | Out-Null
    Write-Host "OK: Ingress Controller pronto" -ForegroundColor Green
} catch {
    Write-Host "AVISO: Ingress Controller nao esta pronto" -ForegroundColor Yellow
}

kubectl apply -f "$K8sPath/ingress.yaml"
kubectl apply -f "$K8sPath/hpa.yaml"
Write-Host ""

# Finalização
Write-Host "==========================================" -ForegroundColor Green
Write-Host " Deploy concluido com sucesso!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

# Status dos pods
Write-Host "Status dos pods:" -ForegroundColor Cyan
kubectl get pods -n $Namespace -o wide
Write-Host ""

# Services
Write-Host "Services:" -ForegroundColor Cyan
kubectl get svc -n $Namespace
Write-Host ""

# Informações úteis
Write-Host "Comandos uteis:" -ForegroundColor Yellow
Write-Host "  Port-forward BFF:  kubectl port-forward -n $Namespace service/bff-api 5450:8080" -ForegroundColor Green
Write-Host "  Ver logs BFF:      kubectl logs -n $Namespace -l app=bff-api -f" -ForegroundColor Green
Write-Host "  Ver todos os pods: kubectl get pods -n $Namespace -w" -ForegroundColor Green