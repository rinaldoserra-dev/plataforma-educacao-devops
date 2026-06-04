# **Plataforma Educacional Distribuída com Microsserviços REST**

## **1. Apresentação**

Bem-vindo ao repositório do projeto **Plataforma Educacional Distribuída**. Este projeto é uma entrega do MBA DevXpert Full Stack .NET e é referente ao módulo **Construção de Aplicações Corporativas**.
O objetivo principal é desenvolver uma evolução do projeto do Módulo 3, transformando a aplicação monolítica em um conjunto de APIs independentes, distribuídas por contexto de negócio (Bounded Context), utilizando boas práticas de arquitetura de microsserviços, comunicação resiliente entre serviços, mensageria e segurança baseada em JWT. Cada API representa um Bounded Context (BC) com banco de dados isolado e responsabilidade clara, integrando-se via HTTP e eventos assíncronos (RabbitMQ), com autenticação centralizada por JWT e um BFF (Backend for Frontend) que orquestra as chamadas para o front-end.

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

- **Linguagem de Programação:** C# 12
- **Frameworks e Bibliotecas:**
  - ASP.NET Core 8 Web API
  - Entity Framework Core 8
  - MediatR (Mediator pattern para CQRS)
  - FluentValidation (Validação de Commands)
  - EasyNetQ (Abstração do RabbitMQ)
  - Polly (Resiliência e retry policies)
- **Banco de Dados:**
  - SQLite (Desenvolvimento e Testes)
- **Mensageria:** RabbitMQ
- **Autenticação e Autorização:**
  - ASP.NET Core Identity
  - JWT (JSON Web Token) para autenticação nas APIs
- **Documentação da API:** Swagger 

## **4. Estrutura do Projeto**
A estrutura do projeto é organizada da seguinte forma:

- src/
  - api gateways/
    - PlataformaEducacao.Bff.Api/                           - BFF - API Gateway
  - building-blocks/
    - PlataformaEducacao.Core/                              - Kernel compartilhado (Entities, Value Objects, Events, Mediator, Validações)
    - PlataformaEducacao.MessageBus/                        - Abstração do RabbitMQ com EasyNetQ
    - PlataformaEducacao.WebApi.Core/                       - Configurações compartilhadas (JWT, Controllers base, Identity)
  - services/
    - GestaoIdentidade/
      - PlataformaEducacao.GestaoIdentidade.Api/            - API de Identidade (registro, login, JWT)
    - GestaoConteudo/
      - PlataformaEducacao.GestaoConteudo.Api/              - API de Gestão de Conteúdo
      - PlataformaEducacao.GestaoConteudo.Application/      - Commands, Queries e Handlers
      - PlataformaEducacao.GestaoConteudo.Domain/           - Entidades e regras de domínio (Curso, Aula, ConteudoProgramatico)
      - PlataformaEducacao.GestaoConteudo.Data/             - Persistência e repositórios
    - GestaoAluno/
      - PlataformaEducacao.GestaoAluno.Api/                 - API de Gestão de Alunos
      - PlataformaEducacao.GestaoAluno.Application/         - Commands, Queries, Handlers e Integration Handlers
      - PlataformaEducacao.GestaoAluno.Domain/              - Entidades e regras de domínio (Aluno, Matricula, Certificado, ProgressoAula)
      - PlataformaEducacao.GestaoAluno.Data/                - Persistência e repositórios
    - GestaoFinanceira/
      - PlataformaEducacao.GestaoFinanceira.Api/            - API de Gestão Financeira
      - PlataformaEducacao.GestaoFinanceira.Business/       - Regras de negócio e modelos de pagamento
      - PlataformaEducacao.GestaoFinanceira.EduPag/         - Gateway de pagamento simulado
  - tests/
      - PlataformaEducacao.Bff.Api.Tests/                   - Testes do BFF - API Gateway
      - PlataformaEducacao.GestaoAluno.Api.Tests/           - Testes da API de Gestão de Alunos
      - PlataformaEducacao.GestaoAluno.Application.Tests    - Testes da aplicação da Gestão de Alunos
      - PlataformaEducacao.GestaoAluno.Domain.Tests         - Testes do domínio da Gestão de Alunos
      - PlataformaEducacao.GestaoConteudo.Api.Tests         - Testes da API de Gestão de Conteúdo
      - PlataformaEducacao.GestaoConteudo.Application.Tests - Testes da aplicação da Gestão de Conteúdo
      - PlataformaEducacao.GestaoConteudo.Domain.Tests      - Testes do domínio da Gestão de Conteúdo
      - PlataformaEducacao.GestaoFinanceira.Api.Tests       - Testes da API de Gestão Financeira
      - PlataformaEducacao.GestaoFinanceira.Business.Tests  - Testes das regras de negócio e modelos da Gestão Financeira
      - PlataformaEducacao.GestaoIdentidade.Api.Tests       - Testes da API de Gestão de Identidade 

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

## **6. Como Executar o Projeto**
### **Pré-requisitos**

- .NET SDK 8.0 ou superior
- RabbitMQ (pode ser executado via Docker)
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   - `git clone https://github.com/rinaldoserra-dev/plataforma-educacao-distribuida.git`
   - `cd plataforma-educacao-distribuida`

2. **Inicie o RabbitMQ** (caso não esteja rodando):
   - `docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
   - Painel de gerenciamento: http://localhost:15672/ (usuário: `guest`, senha: `guest`)

3. **Configuração do Banco de Dados:**
   - No arquivo `appsettings.json` de cada API, em `ConnectionStrings:DefaultConnection`, configure a string de conexão conforme necessário.
   - Em ambiente de desenvolvimento (Development), o banco SQLite é criado e populado automaticamente ao executar os serviços.

4. **Executar a API de Gestão de Identidade:**
   - `dotnet run --project src/services/GestaoIdentidade/PlataformaEducacao.GestaoIdentidade.Api`
   - Acesse a documentação em https://localhost:5431/swagger/

5. **Executar a API de Gestão de Conteúdo:**
   - `dotnet run --project src/services/GestaoConteudo/PlataformaEducacao.GestaoConteudo.Api`
   - Acesse a documentação em https://localhost:5441/swagger/

6. **Executar a API de Gestão de Alunos:**
   - `dotnet run --project src/services/GestaoAluno/PlataformaEducacao.GestaoAluno.Api`
   - Acesse a documentação em https://localhost:5461/swagger/

7. **Executar a API de Gestão Financeira:**
   - `dotnet run --project src/services/GestaoFinanceira/PlataformaEducacao.GestaoFinanceira.Api`
   - Acesse em https://localhost:7083/swagger/

8. **Executar o BFF (API Gateway):**
   - Certifique-se de que os demais serviços estejam em execução.
   - `dotnet run --project "src/api gateways/PlataformaEducacao.Bff.Api"`
   - Acesse a documentação em https://localhost:5451/swagger/

9. **Usuários registrados na carga inicial:**
   - admin@teste.com (Role: ADMIN)
   - aluno@teste.com (Role: ALUNO)
   - A senha para ambos os usuários é: **Teste@123**

## **7. Instruções de Execução e Cobertura dos Testes**
O projeto adota o **Test-Driven Development (TDD)**, e todos os testes podem ser executados com o comando `dotnet test`.
### **7.1. Executar Todos os Testes**
Para rodar todos os testes unitários e de integração na solução, execute na raiz do projeto:
   ```bash
dotnet test PlataformaEducacao.sln
   ```
   
### **7.2.Geração e Visualização do Relatório de Cobertura**
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

     
## **8. Instruções de Configuração**

- **JWT para as APIs:** As chaves de configuração do JWT estão no `appsettings.json` de cada serviço, na seção `AppSettings` (Secret, Emissor, ValidoEm, ExpiracaoHoras).
- **RabbitMQ:** A string de conexão do RabbitMQ está configurada na seção `MessageBus` do `appsettings.json` dos serviços que utilizam mensageria (Identidade, Aluno, Financeira e BFF).
- **Migrações do Banco de Dados:** As migrações são gerenciadas pelo Entity Framework Core e aplicadas automaticamente em ambiente de desenvolvimento. Não é necessário executar manualmente.

## **9. Documentação das APIs**

A documentação de cada API está disponível através do Swagger. Após iniciar os serviços, acesse:

| Serviço | URL do Swagger |
|---|---|
| Gestão Identidade API | https://localhost:5431/swagger/ |
| Gestão Conteúdo API | https://localhost:5441/swagger/ |
| Gestão Aluno API | https://localhost:5461/swagger/ |
| Gestão Financeira API | https://localhost:7083/swagger/ |
| BFF API Gateway | https://localhost:5451/swagger/ |

### **Autenticação nas APIs**

1. Faça login ou registre um novo aluno via **Gestão Identidade API** (`POST /api/identidade/autenticar` ou `POST /api/identidade/novo-aluno`)
2. Copie o `accessToken` retornado na resposta
3. No Swagger de qualquer API, clique em **Authorize** e insira: `Bearer {seu_token}`

## **10. Avaliação**

- Este projeto é parte de um curso acadêmico e não aceita contribuições externas.
- Para feedbacks ou dúvidas utilize o recurso de Issues.
- O arquivo `FEEDBACK.md` é um resumo das avaliações do instrutor e deverá ser modificado apenas por ele.
