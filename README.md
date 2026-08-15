# **Plataforma Educacional Distribuída com Microsserviços REST**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com)
[![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)
[![Kubernetes](https://img.shields.io/badge/Kubernetes-enabled-success)](https://kubernetes.io)
[![CI/CD Pipeline](https://github.com/rinaldoserra-dev/plataforma-educacao-devops/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/rinaldoserra-dev/plataforma-educacao-devops/actions/workflows/ci-cd.yml)

## **1. Apresentação**

Bem-vindo ao repositório do projeto **Plataforma Educacional com Pipeline CI/CD, Docker e Kubernetes**. Este projeto é uma entrega do MBA DevXpert Full Stack .NET e é referente ao quinto módulo do MBA Desenvolvedor.IO.

O objetivo é evoluir a Plataforma Educacional Distribuída desenvolvida no módulo 4, transformando-a em um ecossistema DevOps completo, com automação de build, testes, integração, entrega e orquestração em ambiente Kubernetes.

O trabalho tem como finalidade aplicar, de forma prática, os conceitos de Git/GitHub, Docker, GitHub Actions, Kubernetes e Cultura DevOps, preparando o sistema para rodar em ambientes reais com escalabilidade, resiliência e processos automatizados de entrega contínua.

### **Autores**

- **Diego Junqueira**
- **Felício Melloni**
- **Márcio Gomes**
- **Renato Carrasco**
- **Rinaldo Serra**
- **Saulo Araújo**

## **2. Proposta do Projeto**

O projeto consiste em:

- **Gestão de Identidade API (Auth API):** Serviço responsável pelo registro de usuários (alunos e administradores), autenticação e geração de tokens JWT.
- **Gestão de Conteúdo API:** Serviço para gerenciamento de cursos e aulas da plataforma, com operações de CRUD utilizando CQRS.
- **Gestão de Aluno API:** Serviço para gerenciamento de alunos, matrículas, progresso de aulas, finalização de cursos e geração de certificados.
- **Gestão Financeira API (Pagamentos API):** Serviço de processamento de pagamentos de matrículas, integrado com o gateway de pagamento simulado (EduPag).
- **BFF (Backend for Frontend):** API Gateway que centraliza as chamadas para o front-end e orquestra os fluxos complexos entre serviços, evitando que o front-end seja obrigado a orquestrar a chamada de N APIs.
- **Building Blocks:** Bibliotecas compartilhadas contendo o kernel do domínio, abstração de mensageria e configurações comuns de Web API.

## **3. Tecnologias Utilizadas**

- **Linguagem:** C# 12
- **Framework:** ASP.NET Core 8 Web API, Entity Framework Core 8
- **Padrões:** CQRS com MediatR, FluentValidation, Domain Events
- **Mensageria:** RabbitMQ com EasyNetQ
- **Resiliência:** Polly (Retry, Circuit Breaker)
- **Banco de Dados:** SQL Server
- **Autenticação:** ASP.NET Core Identity + JWT
- **Containerização:** Docker, Docker Compose
- **Orquestração:** Kubernetes (Kind local)
- **CI/CD:** GitHub Actions
- **Documentação:** Swagger / OpenAPI
- **Observabilidade:** prometheus-net.AspNetCore, Prometheus, Grafana e Serilog

## **4. Estrutura do Projeto**

A estrutura do projeto é organizada da seguinte forma:

```
plataforma-educacao-devops/
|
+-- .github/workflows/
|   +-- ci-cd.yml            # CI: build + testes + push de imagens no push à main
|
+-- k8s/                     # Kubernetes manifests
|   +-- namespace.yaml
|   +-- secret.yaml
|   +-- configmap.yaml
|   +-- sqlserver.yaml
|   +-- rabbitmq.yaml
|   +-- gestao-identidade.yaml
|   +-- gestao-conteudo.yaml
|   +-- gestao-aluno.yaml
|   +-- gestao-financeira.yaml
|   +-- bff-api.yaml
|   +-- ingress.yaml          # Ingress (nginx) roteando host plataforma.local -> bff-api
|   +-- hpa.yaml              # HorizontalPodAutoscalers das 5 APIs
|   +-- observability/        # Prometheus, Grafana, ConfigMaps e PVCs
|   +-- deploy.sh             # Script único que aplica todos os manifests na ordem correta
|
+-- src/
|   +-- api-gateways/
|   |   +-- PlataformaEducacao.Bff.Api/
|   +-- services/
|   |   +-- GestaoIdentidade/        (1 projeto: Api)
|   |   +-- GestaoConteudo/          (4 projetos: Api, Application, Data, Domain)
|   |   +-- GestaoAluno/             (4 projetos: Api, Application, Data, Domain)
|   |   +-- GestaoFinanceira/        (3 projetos: Api, Business, EduPag)
|   +-- building-blocks/
|   |   +-- PlataformaEducacao.Core/
|   |   +-- PlataformaEducacao.MessageBus/
|   |   +-- PlataformaEducacao.WebApi.Core/
|   +-- tests/
|       +-- PlataformaEducacao.GestaoIdentidade.Api.Tests/   (SQL Server + RabbitMQ)
|       +-- PlataformaEducacao.GestaoConteudo.Api.Tests/     (SQL Server)
|       +-- PlataformaEducacao.GestaoAluno.Api.Tests/        (SQL Server + RabbitMQ)
|       +-- PlataformaEducacao.GestaoFinanceira.Api.Tests/   (SQL Server + RabbitMQ)
|       +-- ... (demais projetos de teste)
|
+-- docker-compose.yml
+-- observability/            # Configurações de Prometheus e Grafana
|   +-- prometheus/
|   |   +-- prometheus.yml
|   +-- grafana/
|       +-- provisioning/
|       |   +-- datasources/
|       |   +-- dashboards/
|       +-- dashboards/
|           +-- plataforma-overview.json
+-- PlataformaEducacao.sln
+-- build.ps1 / build.sh
```

## **5. Funcionalidades Implementadas**

- **Registro e Autenticação de Usuários:** Cadastro de alunos com integração assíncrona entre os serviços de Identidade e Gestão de Alunos via RabbitMQ. Autenticação via JWT com roles (ADMIN e ALUNO).
- **CRUD de Cursos:** Administradores podem criar, atualizar e listar cursos. Cada curso possui conteúdo programático (descrição e carga horária) e aulas associadas.
- **CRUD de Aulas:** Administradores podem adicionar aulas a um curso, com título, conteúdo, ordem e material complementar.
- **Matrícula em Cursos:** Alunos podem se matricular em cursos disponíveis, com controle de status (pendente de pagamento, em processamento, ativa).
- **Processamento de Pagamentos:** Integração assíncrona entre os serviços de Gestão de Alunos e Gestão Financeira para processamento de pagamentos via gateway simulado (EduPag).
- **Registro de Progresso de Aulas:** Alunos podem registrar o progresso de aulas concluídas, com cálculo automático do percentual de conclusão.
- **Finalização de Curso:** Ao concluir todas as aulas, o aluno pode finalizar o curso, alterando o status do histórico de aprendizado.
- **Geração e Validação de Certificados:** Após a conclusão do curso, é possível gerar e baixar o certificado em PDF, além de validar um certificado pelo código de verificação.
- **Consulta de Histórico do Aluno:** Visualização do histórico completo do aluno com matrículas, progresso e certificados.
- **API Gateway (BFF):** Ponto único de entrada que agrega chamadas dos serviços de Identidade, Conteúdo e Alunos.
- **Documentação da API:** Documentação automática dos endpoints de cada API utilizando Swagger.
- **Health checks nativos (/health)**
- **Observabilidade:** métricas HTTP e de runtime, endpoint `/metrics`, Prometheus, Grafana e logs estruturados em JSON.

## 6. Arquitetura DevOps

```
                    +-------------------+
                    |    Front-end      |
                    +--------+----------+
                             |
                    +--------v----------+
                    |  BFF API Gateway  |
                    |  :5450 / :8080    |
                    +---+---+---+---+---+
                        |   |   |   |
            +-----------+   |   |   +-----------+
            |               |   |               |
    +-------v------+ +------v--++ +------v------+ +-------v------+
    |  Identidade  | | Conteudo| |   Aluno     | |  Financeira  |
    |  API :5430   | | :5440   | |  :5460      | |  :7083       |
    +-------+------+ +----+-----+ +------+------+ +-------+------+
            |              |            |                |
            +--------------+------------+----------------+
                           |
                    +------v------+
                    |   RabbitMQ  |
                    |  :5672      |
                    +-------------+
```

### Fluxo DevOps

```
Desenvolvedor
    |
    v
feature/ branch --> PR --> main
                            |
                            v
                        CI CD (build + testes + lint + push imagens Docker Hub)
                            |                            
                            v
                    kubectl apply -f k8s/
                            |
                            v
                    Cluster Kubernetes (Kind)
```


## **7. Como Executar o Projeto**

### 7.1. Pré-requisitos

- .NET SDK 8.0 ou superior
- Docker e Docker Compose
- RabbitMQ (pode ser executado via Docker)
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git
- (Opcional) Kind para Kubernetes local

### 7.2. Execução Local (sem Docker)

1. Clone o repositório:
   ```bash
   git clone https://github.com/rinaldoserra-dev/plataforma-devops.git
   cd plataforma-devops
   ```

2. Inicie o RabbitMQ:
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```
   Painel: http://localhost:15672 (guest/guest)

3. Execute as APIs (em terminais separados):
   ```bash
   dotnet run --project src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api
   dotnet run --project src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api
   dotnet run --project src/services/GestaoAluno/PlataformaEducacao.GestaoAluno.Api
   dotnet run --project src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api
   dotnet run --project "src/api-gateways/PlataformaEducacao.Bff.Api"
   ```

4. Usuários de teste:
   - admin@teste.com (ADMIN) / aluno@teste.com (ALUNO)
   - Senha: **Teste@123**

### 7.3. Execução com Docker Compose

```bash
docker compose up --build
```

Todos os serviços + RabbitMQ sobem automaticamente. Swagger acessível em:

| Serviço | URL |
|---------|-----|
| Gestão Identidade API | http://localhost:5430/swagger/ |
| Gestão Conteúdo API | http://localhost:5440/swagger/ |
| Gestão Aluno API | http://localhost:5460/swagger/ |
| Gestão Financeira API | http://localhost:7083/swagger/ |
| BFF API Gateway | http://localhost:5000/swagger/ |
| RabbitMQ Management | http://localhost:15672 |

### 7.3.1. Observabilidade

O ambiente Docker Compose inclui Prometheus e Grafana. Para iniciar os serviços, incluindo os componentes de observabilidade:

```bash
docker compose up -d --build
```

| Componente | URL |
|------------|-----|
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 |
| Métricas do BFF | http://localhost:5000/metrics |
| Métricas de Identidade | http://localhost:5430/metrics |
| Métricas de Conteúdo | http://localhost:5440/metrics |
| Métricas de Aluno | http://localhost:5460/metrics |
| Métricas Financeiras | http://localhost:7083/metrics |

O Grafana utiliza o Prometheus como datasource, carrega automaticamente o dashboard **Plataforma Educacao - Overview** e o define como dashboard inicial após o login.

Credenciais padrão para o ambiente local:

- Usuário: `admin`
- Senha: `admin`

Altere essas credenciais antes de utilizar o Grafana em qualquer ambiente compartilhado ou produtivo.

As métricas e os health checks podem ser verificados diretamente com:

```bash
curl http://localhost:5000/metrics
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

No Prometheus, os targets podem ser consultados em http://localhost:9090/targets. As cinco aplicações devem aparecer com o estado `UP`.

Para derrubar:
```bash
docker compose down -v
```

### 7.4. Execução no Kubernetes (Kind)

> **Pré-requisito:** As imagens Docker dos serviços da plataforma são publicadas no Docker Hub pelo pipeline CI/CD (seção 8). O Kind faz o pull dessas imagens do Docker Hub automaticamente, como em qualquer cluster Kubernetes. Não é necessário construir carregar imagens localmente.
> Certifique-se de estar autenticado no Docker Hub no host (`docker login`) caso o repositório seja privado; caso seja público, nenhuma configuração extra é necessária.

#### Limpar ambiente anterior (se existir)

```bash
kind delete cluster --name plataforma-educacao
```

#### Criar cluster do zero

```bash
# 1. Criar cluster
kind create cluster --name plataforma-educacao

# 2. Label do node (necessário para o ingress-nginx)
kubectl label node plataforma-educacao-control-plane ingress-ready=true

# 3. Instalar ingress controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.0/deploy/static/provider/kind/deploy.yaml

# 4. Aguardar ingress-nginx ficar pronto
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=120s
```

*(Opcional, recomendado para acelerar o primeiro cold-start)* Pré-carregue as imagens de infra nos nodes do kind, evitando que o Kind as baixe do Docker Hub sob a rede padrão. SQL Server (~1.5 GB) pode levar vários minutos no primeiro pull:

```bash
docker pull busybox:1.36                          && kind load docker-image busybox:1.36                          --name plataforma-educacao
docker pull mcr.microsoft.com/mssql/server:2022-latest && kind load docker-image mcr.microsoft.com/mssql/server:2022-latest --name plataforma-educacao
docker pull rabbitmq:3-management                 && kind load docker-image rabbitmq:3-management                 --name plataforma-educacao
```

#### Executar o deploy (script único)

O script `k8s/deploy.sh` orquestra toda a aplicação dos manifests na ordem correta (namespace → secret → configmap → infra → wait → APIs → observabilidade → ingress → HPA):

```bash
./k8s/deploy.sh
```

> **Importante:** execute o script **a partir da raiz do repositório** (não de dentro de `k8s/`), pois ele referencia os manifests com caminho relativo (`k8s/namespace.yaml`, etc.). No Windows, use Git Bash, WSL ou PowerShell com `bash k8s/deploy.sh`.

> Caso queira pular o script e aplicar manualmente, use `kubectl apply -f k8s/` (pode ocorrer race condition no namespace na primeira execução — aguarde alguns segundos e execute novamente).

#### Verificar o estado do cluster

```bash
kubectl get pods -n plataforma-educacao
```
São esperados 9 pods em `Running` (sqlserver, rabbitmq, Prometheus, Grafana, 4 APIs e o BFF) e os pods do `ingress-nginx` no namespace `ingress-nginx`.

```bash
kubectl get hpa -n plataforma-educacao    # ver os autoscalers
kubectl get ingress -n plataforma-educacao
```

#### Acessar a aplicação

**Opção A — Via Ingress (recomendada, sem port-forward):**

O `k8s/ingress.yaml` define o host `plataforma.local` roteando para o serviço `bff-api`. Para que esse hostname resolva no seu navegador/curl, adicione a entrada abaixo ao arquivo de hosts do SO:

- **Windows:** `C:\Windows\System32\drivers\etc\hosts` (abrir como Administrador)
- **Linux/macOS:** `/etc/hosts`

```
127.0.0.1  plataforma.local
```

Após salvar, acesse:

```
http://plataforma.local/swagger/         # BFF (Swagger)
http://plataforma.local/health            # health check do BFF
```

> O kind com `ingress-nginx` expõe a porta 80 do host automaticamente; em alguns ambientes pode ser necessário `kubectl port-forward -n ingress-nginx service/ingress-nginx-controller 80:80`.

**Opção B — Via port-forward (alternativa):**

```bash
kubectl port-forward -n plataforma-educacao service/bff-api             5450:8080
kubectl port-forward -n plataforma-educacao service/gestao-identidade-api 5430:8080
# etc.
```
Swagger do BFF em `http://localhost:5450/swagger/`.

#### Acessar a observabilidade no Kubernetes

O deploy também instala Prometheus e Grafana no namespace `plataforma-educacao`:

```bash
kubectl port-forward -n plataforma-educacao service/prometheus 9090:9090
kubectl port-forward -n plataforma-educacao service/grafana 3000:3000
```

Os comandos devem ser executados em terminais separados. Depois, acesse:

| Componente | URL |
|------------|-----|
| Prometheus | http://localhost:9090 |
| Prometheus targets | http://localhost:9090/targets |
| Grafana | http://localhost:3000 |
| Dashboard Grafana | http://localhost:3000/d/plataforma-overview/plataforma-educacao-overview |

Credenciais locais do Grafana:

- Usuário: `admin`
- Senha: `admin`

O Prometheus coleta internamente os endpoints `/metrics` dos Services das cinco aplicações. Após gerar tráfego no BFF, valide no Prometheus com:

```promql
up
sum(rate(http_requests_received_total[5m]))
```

Todos os cinco targets devem aparecer como `UP` em `http://localhost:9090/targets`.

#### Verificando o log estruturado (JSON)

No k8s, por padrão, apenas o sink Console está ativo (stdout do pod — o sink File é neutralizado via env var no `k8s/configmap.yaml`). Para inspecionar:

```bash
kubectl logs -n plataforma-educacao deploy/gestao-aluno-api --tail=20
```

Cada linha é um JSON compacto (formato `Serilog.Formatting.Compact`) contendo `@t` (timestamp), `@mt` (mensagem template), `CorrelationId`, `RequestMethod`, `RequestPath`, `StatusCode`, `Elapsed`, `SourceContext`, `MachineName`, `ProcessId`, `ThreadId`, `Application`, `Service`, entre outros. Pronto para ingestão por stacks de observabilidade (ELK, Datadog, Seq, Loki, etc.) lendo o stdout do pod.

Para rastrear uma requisição ponta-a-ponta entre serviços, envie um header `X-Correlation-ID` e procure o mesmo valor nos logs de todos os pods envolvidos:

```bash
curl -i -H "X-Correlation-ID: meu-teste-123" http://plataforma.local/api/alunos/matriculas-ativas
kubectl logs -n plataforma-educacao -l app=bff-api          | grep meu-teste-123
kubectl logs -n plataforma-educacao -l app=gestao-aluno-api  | grep meu-teste-123
```

#### Deletar o cluster

```bash
kind delete cluster --name plataforma-educacao
```

## 8. Pipeline CI/CD

### CI (Integração Contínua)

Disparada em **pull requests** e **push** para `main`:

A etapa de CI é composta por 3 jobs paralelos:

1. **build-test (Build e testes)**
   - `actions/checkout@v4` → `actions/setup-dotnet@v4` (utilizando .NET 8.0.x)
   - `dotnet restore PlataformaEducacao.sln` — restaura as dependências da solução
   - `dotnet build PlataformaEducacao.sln -c Release --no-restore` — compila o projeto em modo Release
   - `dotnet test PlataformaEducacao.sln -c Release --no-build --collect:"XPlat Code Coverage"` — executa os testes unitários e coleta a cobertura de código utilizando o coletor XPlat.

2. **lint (Lint e análise estática)**
   - `actions/checkout@v4` → `actions/setup-dotnet@v4` (utilizando .NET 8.0.x)
   - `dotnet restore PlataformaEducacao.sln` — restaura as dependências da solução
   - `dotnet format PlataformaEducacao.sln --verify-no-changes --no-restore` — valida se o código segue os padrões de formatação definidos, falhando o job caso existam divergências.

3. **observability-validation (Validação de observabilidade)**
   - Valida a configuração com `docker compose config`.
   - Confirma os cinco targets configurados no Prometheus.
   - Valida o JSON do dashboard Grafana.
   - Verifica a existência dos endpoints de health check e métricas.

### CD (Deploy Contínuo)

Disparada automaticamente após o sucesso dos jobs de CI (`build-test`, `lint` e `observability-validation`) apenas quando o evento for um push na branch `main`:

1. **docker-push (Build e push das imagens) (matrix strategy)**
   - `docker/login-action@v3` — realiza a autenticação no Docker Hub
   - `docker/build-push-action@v6` — executa o build e push de cada microsserviço
   - Aplica as tags `latest` e o SHA do commit (`${{ github.sha }}`) correspondente.

Imagens publicadas em: https://hub.docker.com/u/`${{ secrets.DOCKERHUB_USERNAME }}`

### Secrets do GitHub

| Secret | Descrição |
|--------|-----------|
| `DOCKERHUB_USERNAME` | Nome de usuário no Docker Hub |
| `DOCKERHUB_TOKEN` | Token de acesso do Docker Hub |

## **9. Instruções de Execução e Cobertura dos Testes**

O projeto adota o **Test-Driven Development (TDD)**, e todos os testes podem ser executados com o comando `dotnet test`.

### **Executar Todos os Testes**

Para rodar todos os testes unitários e de integração na solução, execute na raiz do projeto:
   ```bash
dotnet test PlataformaEducacao.sln
   ```

### **Geração e Visualização do Relatório de Cobertura**

O projeto possui scripts de automação que, além de rodarem os testes, utilizam as ferramentas `XPlat Code Coverage` e `ReportGenerator` para gerar relatórios unificados de cobertura em HTML.

#### Execução dos Testes e Geração do Relatório

Para garantir que a cobertura de código seja gerada corretamente, utilize os scripts específicos do seu ambiente (na raiz do projeto):

* **Ambientes Windows (PowerShell):**

    ```powershell
    .\build.ps1
    ```

* **Ambientes Linux/macOS (Shell Script):**

    ```bash
    chmod +x build.sh # Torna o script executável, se necessário
    ./build.sh
    ```
    *Nota: Se preferir rodar manualmente sem o script, utilize o comando `dotnet test --collect:"XPlat Code Coverage" PlataformaEducacao.sln` seguido da execução do `ReportGenerator`.*

#### Visualização do Relatório

Após a execução do script, o relatório completo (incluindo cobertura detalhada por projeto e arquivo) é gerado na pasta `coveragereport/` na raiz do projeto.

Para visualizar os resultados, abra o arquivo `index.html` no seu navegador:

```bash
cd coveragereport/
# Comando para abrir o arquivo no Windows
start index.html
# Comando para abrir o arquivo no macOS/Linux
open index.html
 ```

A cobertura mínima exigida no CI é de **80%**. Relatório é gerado via `reportgenerator` em `TestResults/merged/index.html`.

## 10. Documentação das APIs (Swagger)

Todas as APIs possuem Swagger configurado com autenticação Bearer JWT.

### URLs de acesso

| Serviço | Desenvolvimento (HTTPS) | Docker (HTTP) | Kubernetes (port-forward) |
|---------|------------------------|---------------|---------------------------|
| Identidade | https://localhost:5431/swagger/ | http://localhost:5430/swagger/ | http://localhost:5430/swagger/ |
| Conteúdo | https://localhost:5441/swagger/ | http://localhost:5440/swagger/ | http://localhost:5440/swagger/ |
| Aluno | https://localhost:5461/swagger/ | http://localhost:5460/swagger/ | http://localhost:5460/swagger/ |
| Financeira | https://localhost:7083/swagger/ | http://localhost:7083/swagger/ | http://localhost:7083/swagger/ |
| BFF | https://localhost:5451/swagger/ | http://localhost:5000/swagger/ | http://localhost:5000/swagger/ |

### Autenticação no Swagger

1. Faça login via **Identidade API**: `POST /api/identidade/autenticar`
2. Copie o `accessToken` da resposta
3. Clique em **Authorize** no Swagger e insira `Bearer {token}`

## 11. Configuração

- **JWT:** Configurado em `AppSettings` no `appsettings.json` de cada serviço (Secret, Emissor, ValidoEm, ExpiracaoHoras)
- **RabbitMQ:** String de conexão em `MessageBus` ou `MessageQueueConnection__MessageBus`
- **Banco de Dados:** SQL Server via container Docker. Migrations aplicadas automaticamente na inicialização (`UseDbMigrationHelper`)
- **Observabilidade:**  
  - **Logs:** Serilog com CorrelationId, sink Console + sink File rotativo em JSON compacto (`logs/log-YYYYMMDD.json`, rotação diária, retenção de 7 dias). Formato `Serilog.Formatting.Compact.CompactJsonFormatter` — cada evento é um objeto JSON de linha única, pronto para ingestão em ELK/Datadog/Seq/Loki.
    - Configuração centralizada no building block `PlataformaEducacao.WebApi.Core` (`Extensions/LoggingConfig.cs`), exposta via `AddLoggingConfiguration`, `AddCorrelationIdConfiguration` e `UseLoggingConfiguration` (invocadas nos 5 `Program.cs`).
    - Pacotes: `Serilog` 4.3.0, `Serilog.AspNetCore` 8.0.3, `Serilog.Extensions.Hosting` 8.0.0, `Serilog.Sinks.Console` 6.0.0, `Serilog.Sinks.File` 6.0.0, `Serilog.Sinks.Debug` 3.0.0, `Serilog.Enrichers.Environment` 3.0.1, `Serilog.Enrichers.Process` 3.0.0, `Serilog.Enrichers.Thread` 4.0.0, `Serilog.Settings.Configuration` 8.0.4, `Serilog.Formatting.Compact` 3.0.0, `CorrelationId` 3.0.1 (stevejgordon).
    - Campos estruturados por evento: `@t` (timestamp UTC ISO 8601), `@mt` (message template), `@l` (level), `@tr`/`@sp` (TraceId/SpanId), `CorrelationId`, `SourceContext`, `MachineName`, `ProcessId`, `ThreadId`, `Application`, `Service`, e campos de request HTTP (`RequestMethod`, `RequestPath`, `StatusCode`, `Elapsed`) via `UseSerilogRequestLogging`.
  - **Correlação entre serviços:** middleware `CorrelationId` lê/gera o header `X-Correlation-ID` (presente na resposta HTTP). No BFF, `AddCorrelationIdForwarding` propaga o header em chamadas `HttpClient` outbound para os serviços backend. O mesmo `CorrelationId` atravessa BFF → serviço backend → log, permitindo rastrear uma requisição ponta-a-ponta.
  - **Logs no Kubernetes:** o sink File é neutralizado via env vars no `k8s/configmap.yaml` (`Serilog__WriteTo__1__Args__path=/dev/null`), de modo que **apenas o sink Console** grava em **stdout** do pod — consumido via `kubectl logs -n plataforma-educacao <pod>`. No docker-compose o File sink permanece ativo (`logs/log-YYYYMMDD.json`, 7 dias).
  - **Métricas:** todas as cinco aplicações expõem o endpoint `/metrics`, com métricas HTTP, runtime e de exceções não tratadas.
    - Métricas HTTP incluem quantidade de requisições, duração, status HTTP e requisições em andamento.
    - Métricas de runtime incluem uso de memória e CPU do processo .NET.
    - Exceções não tratadas são contabilizadas em `application_exceptions_total`.
    - A configuração centralizada está em `src/building-blocks/PlataformaEducacao.WebApi.Core/Extensions/MetricsConfig.cs`.
  - **Prometheus:** coleta as métricas das aplicações a cada 15 segundos, conforme `observability/prometheus/prometheus.yml`.
  - **Grafana:** utiliza o Prometheus como datasource e provisiona o dashboard `observability/grafana/dashboards/plataforma-overview.json`.
  - **Health Checks:**
    - `/health/live` verifica somente se a aplicação está em execução.
    - `/health/ready` verifica SQL Server, RabbitMQ e as APIs dependentes do BFF.
    - As dependências possuem timeouts para evitar respostas bloqueadas indefinidamente.

## 12. Avaliação

- Projeto acadêmico — não aceita contribuições externas.
- Feedbacks e dúvidas via Issues do GitHub.
- O arquivo `FEEDBACK.md` consolida avaliações do instrutor e melhorias realizadas.
