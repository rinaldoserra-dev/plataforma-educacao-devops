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
|   +-- configmap.yaml
|   +-- secret.yaml
|   +-- rabbitmq.yaml
|   +-- bff-api.yaml
|   +-- gestao-identidade.yaml
|   +-- gestao-conteudo.yaml
|   +-- gestao-aluno.yaml
|   +-- gestao-financeira.yaml
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
| BFF API Gateway | http://localhost:5450/swagger/ |
| RabbitMQ Management | http://localhost:15672 |

Para derrubar:
```bash
docker compose down -v
```

### 7.4. Execução no Kubernetes (Kind)

1. Crie o cluster Kind:
   ```bash
   kind create cluster --name plataforma-educacao
   ```

2. Aplique os manifests:
   ```bash
   kubectl apply -f k8s/
   ```

3. Verifique os pods:
   ```bash
   kubectl get pods -n plataforma-educacao
   ```

4. Acesse os serviços via port-forward:
   ```bash
   kubectl port-forward -n plataforma-educacao service/bff-api 5450:8080
   kubectl port-forward -n plataforma-educacao service/gestao-identidade-api 5430:8080
   # etc.
   ```

5. Para deletar o cluster:
   ```bash
   kind delete cluster --name plataforma-educacao
   ```

## 8. Pipeline CI/CD

### CI (Integração Contínua)

Disparada em **pull requests** e **push** para `main`:

A etapa de CI é composta por 2 jobs paralelos:

1. **build-test (Build e testes)**
   - `actions/checkout@v4` → `actions/setup-dotnet@v4` (utilizando .NET 8.0.x)
   - `dotnet restore PlataformaEducacao.sln` — restaura as dependências da solução
   - `dotnet build PlataformaEducacao.sln -c Release --no-restore` — compila o projeto em modo Release
   - `dotnet test PlataformaEducacao.sln -c Release --no-build --collect:"XPlat Code Coverage"` — executa os testes unitários e coleta a cobertura de código utilizando o coletor XPlat.

2. **lint (Lint e análise estática)**
   - `actions/checkout@v4` → `actions/setup-dotnet@v4` (utilizando .NET 8.0.x)
   - `dotnet restore PlataformaEducacao.sln` — restaura as dependências da solução
   - `dotnet format PlataformaEducacao.sln --verify-no-changes --no-restore` — valida se o código segue os padrões de formatação definidos, falhando o job caso existam divergências.

### CD (Deploy Contínuo)

Disparada automaticamente após o sucesso dos jobs de CI (build-test e lint) apenas quando o evento for um push na branch `main`:

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
| BFF | https://localhost:5451/swagger/ | http://localhost:5450/swagger/ | http://localhost:5450/swagger/ |

### Autenticação no Swagger

1. Faça login via **Identidade API**: `POST /api/identidade/autenticar`
2. Copie o `accessToken` da resposta
3. Clique em **Authorize** no Swagger e insira `Bearer {token}`

## 11. Configuração

- **JWT:** Configurado em `AppSettings` no `appsettings.json` de cada serviço (Secret, Emissor, ValidoEm, ExpiracaoHoras)
- **RabbitMQ:** String de conexão em `MessageBus` ou `MessageQueueConnection__MessageBus`
- **Banco de Dados:** SQL Server via container Docker. Migrations aplicadas automaticamente na inicialização (`UseDbMigrationHelper`)
- **Observabilidade:**  
  - **Logs:** Serilog com CorrelationId, Console + arquivo rotativo (`logs/log-.txt`, 7 dias de retenção)
- **Health Checks:** Endpoints `/health` de todas as APIs

## 12. Avaliação

- Projeto acadêmico — não aceita contribuições externas.
- Feedbacks e dúvidas via Issues do GitHub.
- O arquivo `FEEDBACK.md` consolida avaliações do instrutor e melhorias realizadas.
