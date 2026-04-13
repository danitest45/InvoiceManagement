# Invoice Management API

Projeto desenvolvido como solução para o desafio técnico de Desenvolvedor de Software Pleno (.NET / C#).

A aplicação permite o gerenciamento de faturas e itens de fatura, contemplando regras de negócio, persistência em banco de dados, API REST e testes automatizados.

---

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server LocalDB
- xUnit
- FluentAssertions
- Swagger / OpenAPI

---

## Estrutura do projeto

A solução foi organizada em camadas para garantir separação de responsabilidades e melhor manutenção do código:

- **InvoiceManagement.Api** → controllers, middleware e configuração da API
- **InvoiceManagement.Application** → services e DTOs
- **InvoiceManagement.Domain** → entidades, enums e regras de domínio
- **InvoiceManagement.Infrastructure** → DbContext e persistência
- **InvoiceManagement.Tests** → testes automatizados

---

## Funcionalidades implementadas

- criação de fatura
- inclusão de itens
- fechamento de fatura
- consulta por id
- listagem com filtros
- recálculo automático do valor total
- bloqueio de alteração em fatura fechada
- validação de justificativa para itens acima de R$ 1.000,00
- tratamento global de exceções

---

## Regras de negócio atendidas

- fatura criada com status inicial aberta
- nome do cliente obrigatório
- descrição do item obrigatória
- item acima de R$ 1.000,00 exige justificativa
- fatura fechada não pode ser alterada
- valor total recalculado automaticamente

---

##  Decisões técnicas relevantes

Algumas decisões técnicas foram tomadas para garantir clareza, manutenibilidade e aderência a boas práticas:

arquitetura em camadas para separação de responsabilidades
uso de DTOs para evitar exposição direta das entidades de domínio
uso de interface na camada de serviço para melhor testabilidade e inversão de dependência
middleware global para tratamento padronizado de erros
testes automatizados cobrindo cenários positivos e regras de negócio
aplicação automática das migrations na inicialização da API para simplificar a execução
uso de filtros com tratamento de data final inclusiva para melhorar a experiência de consulta

##  Melhorias que seriam realizadas com mais tempo

Com mais tempo disponível, algumas melhorias que poderiam ser implementadas:

* paginação na listagem de faturas
* autenticação e autorização com JWT
* deploy em ambiente cloud para disponibilização pública da API e frontend
* logs estruturados com rastreamento por request
* versionamento da API
* containerização com Docker
* monitoramento e health checks

---

## Como executar o projeto

O projeto foi desenvolvido para ter uma execução simples e rápida.

### Pré-requisitos

Antes de executar, é necessário ter instalado:

* **.NET 8 SDK**
* **Visual Studio 2022** (recomendado) com workload de ASP.NET e desenvolvimento web
* **SQL Server LocalDB** (normalmente já vem com o Visual Studio)

---

### Executando a API

1. Clone o repositório

```bash id="rdmpt01"
git clone https://github.com/danitest45/InvoiceManagement.git
```

2. Abra a solution no **Visual Studio**

3. Certifique-se de que o projeto `InvoiceManagement.Api` está como projeto de inicialização

4. Execute a aplicação utilizando o perfil **HTTPS padrão** (`F5`)

> Não é necessário configurar o banco de dados manualmente.
> A base e as migrations são criadas automaticamente ao iniciar a aplicação.

5. Acesse o Swagger pelo endereço exibido ao executar o projeto, por exemplo:

```text id="rdmpt02"
https://localhost:xxxx/swagger
```

---

### Executando os testes

Os testes podem ser executados pelo **Test Explorer** do Visual Studio.

Ou pelo terminal:

```bash id="rdmpt03"
dotnet test
```

## Extensão do desafio — Frontend

Como diferencial adicional ao desafio técnico, foi desenvolvido também um **frontend web integrado à API**, permitindo a visualização e gerenciamento das faturas de forma visual.

O frontend consome todos os endpoints principais da API, incluindo:

* listagem de faturas
* filtros por cliente, data e status
* criação de novas faturas
* adição de itens
* fechamento de faturas

Este frontend foi desenvolvido como **extensão do teste para demonstrar uma solução full stack e integração ponta a ponta**.

🔗 Repositório do frontend:
https://github.com/danitest45/invoice-management-front

