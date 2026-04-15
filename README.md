# 📦 Pedidos-App

API REST em .NET 9 para cadastro e consulta de pedidos, com frontend em Razor (MVC), integração com mensageria via Redpanda (Kafka-compatible) e persistência em SQL Server.

---

## 🧱 Arquitetura

```
┌─────────────────────┐        ┌──────────────┐        ┌─────────────┐
│   Frontend (Razor)  │──────▶ │  API (.NET)  │──────▶ │  Redpanda   │
└─────────────────────┘        └──────┬───────┘        └─────────────┘
                                      │
                                      ▼
                               ┌──────────────┐
                               │  SQL Server  │
                               └──────────────┘
```

- **POST /orders** → Recebe o pedido e publica no tópico `orders` do Redpanda
- **GET /orders/{id}** → Consulta pedido no SQL Server
- **GET /orders** → Lista todos os pedidos do SQL Server
- O **PedidosWorker** (repositório separado) consome o tópico e persiste no banco

---

## 🛠️ Tecnologias

- .NET 9 / ASP.NET Core MVC
- Entity Framework Core + SQL Server
- Confluent.Kafka (Redpanda)
- xUnit + Moq (testes unitários)
- Docker / Docker Compose

---

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop/)
- SQL Server (local ou via Docker)
- Redpanda rodando na porta `9092`

---

## 🐳 Subindo tudo com Docker Compose (Recomendado)

A forma mais simples de rodar o sistema completo — API, Worker, Redpanda e SQL Server sobem juntos com um único comando.

### 1. Clone os dois repositórios

```bash
git clone https://github.com/cadugomes06/Pedidos-app.git
git clone https://github.com/cadugomes06/Pedidos-worker.git
```

### 2. Build das imagens

```bash
# Imagem da API
cd Pedidos-app
docker build -t pedidosapp .

# Imagem do Worker
cd ../Pedidos-worker/PedidosWorker
docker build -t pedidosworker .
```

### 3. Suba todos os serviços

```bash
cd ../../Pedidos-app
docker-compose up
```

| Serviço | URL |
|---------|-----|
| API + Frontend | http://localhost:8080 |
| Redpanda | localhost:9092 |
| SQL Server | localhost:1433 |

> Para rodar em background: `docker-compose up -d`
> Para parar tudo: `docker-compose down`

---

## ⚙️ Rodando localmente (sem Docker)

### 1. Clone o repositório

```bash
git clone https://github.com/cadugomes06/Pedidos-app.git
cd Pedidos-app
```

### 2. Configure o `appsettings.Development.json`

Dentro de `PedidosApp/`, crie o arquivo `appsettings.Development.json` (não versionado):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=PedidoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "orders"
  }
}
```

### 3. Suba o Redpanda via Docker

```bash
docker run -d --name redpanda \
  -p 9092:9092 \
  -p 9644:9644 \
  docker.redpanda.com/redpandadata/redpanda:latest \
  redpanda start --overprovisioned \
  --smp 1 --memory 1G \
  --reserve-memory 0M \
  --node-id 0 \
  --check=false \
  --kafka-addr 0.0.0.0:9092 \
  --advertise-kafka-addr localhost:9092
```

### 4. Crie o banco de dados

```bash
cd PedidosApp
dotnet ef database update
```

### 5. Rode a aplicação

```bash
dotnet run
```

A aplicação estará disponível em `http://localhost:5000`.

---

## 📡 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/orders` | Cadastra um novo pedido e publica no Redpanda |
| `GET` | `/orders/{id}` | Retorna um pedido pelo ID |
| `GET` | `/orders` | Lista todos os pedidos |

### Exemplo de payload — POST /orders

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cliente": "João Silva",
  "valor": 150.00,
  "dataPedido": "2026-04-14T10:00:00"
}
```

---

## 🧪 Testes

```bash
cd PedidosApp.Test
dotnet test
```

---

## 📁 Estrutura do Projeto

```
Pedidos-app/
├── PedidosApp/
│   ├── Controllers/        # HomeController, OrderController
│   ├── Domain/             # Entidade Order
│   ├── DTO/                # CreateOrderDTO, ApiResponseDTO
│   ├── Infrastructure/     # ApplicationDbContext (EF Core)
│   ├── Service/
│   │   ├── Interface/      # IOrderService
│   │   └── Implementation/ # OrderService
│   ├── Views/              # Razor Views
│   ├── wwwroot/            # CSS, JS, imagens
│   ├── Program.cs
│   └── appsettings.json
├── PedidosApp.Test/
│   └── service/
│       └── OrderServiceTests.cs
├── docker-compose.yml
├── Dockerfile
└── PedidosApp.sln
```

---

## 🔗 Repositório relacionado

- [PedidosWorker](https://github.com/cadugomes06/Pedidos-worker) — Worker que consome o tópico Redpanda e persiste os pedidos no SQL Server
