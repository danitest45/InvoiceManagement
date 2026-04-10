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

## Como executar o projeto

1. Restaurar pacotes
bash
dotnet restore


2. Atualizar banco de dados

dotnet ef database update --project InvoiceManagement.Infrastructure --startup-project InvoiceManagement.Api

3. Executar aplicação

dotnet run --project InvoiceManagement.Api