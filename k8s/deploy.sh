#!/bin/bash
set -e

NAMESPACE="plataforma-educacao"
CLUSTER_NAME="plataforma-educacao"

echo "=========================================="
echo " Deploy - Plataforma Educacao DevOps"
echo "=========================================="

# Verificar kubectl
if ! command -v kubectl &> /dev/null; then
    echo "ERRO: kubectl nao encontrado. Instale: https://kubernetes.io/docs/tasks/tools/"
    exit 1
fi

# Verificar cluster
if ! kubectl cluster-info &> /dev/null; then
    echo "AVISO: Nenhum cluster Kubernetes encontrado."
    echo "Para criar com Kind:"
    echo "  kind create cluster --name $CLUSTER_NAME"
    exit 1
fi

echo ""
echo "[1/8] Aplicando Namespace..."
kubectl apply -f k8s/namespace.yaml

echo ""
echo "[2/8] Aplicando Secrets..."
kubectl apply -f k8s/secret.yaml

echo ""
echo "[3/8] Aplicando ConfigMap..."
kubectl apply -f k8s/configmap.yaml

echo ""
echo "[4/8] Aplicando infraestrutura (SQL Server + RabbitMQ)..."
kubectl apply -f k8s/sqlserver.yaml
kubectl apply -f k8s/rabbitmq.yaml

echo ""
echo "[5/8] Aguardando infraestrutura ficar pronta..."
kubectl wait --for=condition=ready pod -l app=sqlserver -n $NAMESPACE --timeout=300s || true
kubectl wait --for=condition=ready pod -l app=rabbitmq -n $NAMESPACE --timeout=120s || true

echo ""
echo "[6/8] Aplicando deployments de aplicacao..."
kubectl apply -f k8s/gestao-identidade.yaml
kubectl apply -f k8s/gestao-conteudo.yaml
kubectl apply -f k8s/gestao-aluno.yaml
kubectl apply -f k8s/gestao-financeira.yaml
kubectl apply -f k8s/bff-api.yaml

echo ""
echo "[7/8] Aplicando observabilidade (Prometheus + Grafana)..."
kubectl apply -f k8s/observability/

echo ""
echo "[8/8] Aguardando Ingress Controller e aplicando Ingress + HPA..."
kubectl wait --namespace ingress-nginx --for=condition=ready pod --selector=app.kubernetes.io/component=controller --timeout=120s 2>/dev/null || echo "AVISO: Ingress controller nao estava pronto, tentando mesmo assim..."
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml

echo ""
echo "=========================================="
echo " Deploy concluido!"
echo "=========================================="
echo ""
echo "Status dos pods:"
kubectl get pods -n $NAMESPACE -o wide
echo ""
echo "Services:"
kubectl get svc -n $NAMESPACE
echo ""
echo "Para acessar o BFF via port-forward:"
echo "  kubectl port-forward -n $NAMESPACE service/bff-api 5450:8080"
echo ""
echo "Para verificar logs:"
echo "  kubectl logs -n $NAMESPACE -l app=bff-api -f"
