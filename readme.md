# Controle de Gastos 💰

Sistema de controle financeiro pessoal desenvolvido com **.NET 8** seguindo os princípios de **Clean Architecture** e **DDD (Domain Driven Design)**.

## 🏗️ Arquitetura do Projeto

O projeto é estruturado em camadas para garantir a separação de responsabilidades e facilitar a manutenção:

- **api/ControleGastos.API**: Ponto de entrada da aplicação (Controllers, Swagger, Injeção de Dependência).
- **api/ControleGastos.Application**: Camada de serviço (Interfaces de serviço, DTOs, Mapeamentos).
- **api/ControleGastos.Domain**: Core do negócio (Entidades, Interfaces de Repositório, Regras de Negócio).
- **api/ControleGastos.Infrastructure**: Implementações externas (EF Core, Configuração do SQLite, Repositórios, Unit of Work).
- **web/**: Frontend desenvolvido em React com TypeScript (Vite).

---

## 🐳 Via Docker (Recomendado)

Na raiz do projeto (onde está o arquivo `docker-compose.yml`), abra o terminal e execute:

```cmd
docker-compose up --build
```

### 🚀 Acesso aos Serviços
Após o carregamento, os serviços estarão disponíveis em:

* **Swagger (Documentação API):** [http://localhost:5000/swagger](http://localhost:5000/swagger)
* **Frontend (React):** [http://localhost:5173](http://localhost:5173)

---

## 💻 Desenvolvimento Local (Sem Docker)

### 🔹 Backend (.NET 8)
Abra o terminal na pasta da API e execute:

```cmd
cd api/ControleGastos.API
dotnet run
```

* **Endpoint local definido em launchSettings.json:** [http://localhost:5018/swagger](http://localhost:5018/swagger)
* **Banco de Dados:** O SQLite será gerado automaticamente em `api/ControleGastos.Infrastructure/ControGastos.db`.

### 🔹 Frontend (React)
Abra o terminal na pasta `web`, instale as dependências e inicie o projeto:

```cmd
cd web
npm install
npm run dev
```

---

## 📁 Pastas Principais

* `/api`: Código fonte do Backend (C#).
* `/web`: Código fonte do Frontend (React/TS).
* `/database`: Volume de persistência do SQLite (utilizado pelo Docker).