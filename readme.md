# Controle de Gastos 💰

Sistema de controle financeiro pessoal desenvolvido com **.NET 8** seguindo os princípios de **Clean Architecture** e **DDD (Domain Driven Design)**.

## 🏗️ Arquitetura do Projeto

O projeto é estruturado em micro serviços para garantir a separação de responsabilidades e facilitar a manutenção:

- **api/ControleGastos.API**: Ponto de entrada da aplicação (Controllers, Swagger, Injeção de Dependência).
- **api/ControleGastos.Application**: Camada de serviço (Interfaces de serviço, DTOs, Mapeamentos).
- **api/ControleGastos.Domain**: Core do negócio (Entidades, Interfaces de Repositório, Regras de Negócio).
- **api/ControleGastos.Infrastructure**: Implementações externas (EF Core, Configuração do SQLite, Repositórios, Unit of Work).
- **web/**: Frontend desenvolvido em React com TypeScript (Vite).

---

## 🚀 Rodar projeto completo (Comando Único) via Docker (Recomendado)

Na raiz do projeto, execute:

```cmd
cd ControleGastos
docker-compose up --build
```

---

### 🚀 Acesso aos Serviços
Após o carregamento, os serviços estarão disponíveis em:

* **Swagger (Documentação API):** [http://localhost:5000/swagger](http://localhost:5000/swagger)
* **Frontend (React):** [http://localhost:5173](http://localhost:5173)

---

## 💻 Execução Local (Sem Docker)

### 🔹 Backend (.NET 8)
Abra o terminal na pasta da API e execute:

```cmd
cd api/ControleGastos.API
dotnet run
```

* **Swagger Local:** [http://localhost:5018/swagger](http://localhost:5018/swagger)
* **Banco de Dados:** O SQLite será gerado automaticamente em `api/ControleGastos.Infrastructure/ControleGastos.db`.

### 🔹 Frontend (React)
Abra o terminal na pasta `web`, instale as dependências e inicie o projeto:

```cmd
cd web
npm install
npm run dev
```

* **Acesso Local: http://localhost:5173
* **Configuração: O frontend consome a API através do Axios, configurado em src/services/api.ts.

---

## 🛠️ Tecnologias Utilizadas

* **Backend: .NET 8, Entity Framework Core, SQLite, AutoMapper, Swagger/OpenAPI.
* **Frontend: React 18, TypeScript, Vite, Tailwind CSS v4, Axios, React Router Dom.

---

## 📁 Pastas Principais

* `/api`: Código fonte do Backend (C#).
* `/web`: Código fonte do Frontend (React/TS).
* `/database`: Volume de persistência do SQLite (utilizado pelo Docker).

---

# 🧪 Testes Unitários e Cobertura

## Backend

### Utilizamos o Fine Code Coverage para visualização de métricas de testes.

A Extensão: A pessoa que baixar o repositório precisa ter a extensão "Fine Code Coverage" instalada no Visual Studio para ver os relatórios gráficos em tempo real dentro da IDE.

## 🛠️ Como rodar os testes

1. Abra o Test Explorer no Visual Studio.

2. Execute todos os testes (Run All Tests).

3. Abra a janela Fine Code Coverage (View > Other Windows > Fine Code Coverage) para ver a porcentagem de cobertura.

---
## Frontend
 
## Instalação das Dependências
 
Para rodar os testes e gerar os relatórios de cobertura, é necessário instalar o motor de coverage:
 
```cmd
npm install -D @vitest/coverage-v8
```
 
## Configuração de Scripts
 
Adicione ou atualize os scripts no seu `package.json`:
 
```json
"scripts": {
  "test": "vitest",
  "test:coverage": "vitest run --coverage"
}
```
 
## Como Executar
 
* ### Via Terminal (CLI)
 
**Rodar testes:**
 
```cmd
npm test
```
 
**Gerar relatório de cobertura:**
 
```cmd
npm run test:coverage
```
 
* ### Via VS Code (Interface Gráfica)
 
1. Instale a extensão oficial **Vitest**.
2. Abra a aba **Testing** (ícone de um "bequer") na barra lateral esquerda do VS Code.
3. Clique no ícone de **Play** para rodar os testes ou no ícone de **Coverage** para visualizar as linhas cobertas diretamente no código.
 
## O que cada comando faz
 
| Comando | Descrição |
|---|---|
| `npm test` | Executa os testes normalmente. |
| `npm run test:coverage` | Executa todos os testes e gera estatísticas de cobertura de cada classe testada. |
