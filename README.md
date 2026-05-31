# CidadeAtivaApi

```
            |   _   _
      . | . x .|.|-|.|
   |\ ./.\-/.\-|.|.|.|
~~~|.|_|.|_|.|.|.|_|.|~~~
```

**CidadeAtivaApi** é uma Web API REST para gestão e relato de problemas urbanos por cidadãos — como buracos, iluminação e saneamento. O projeto foi desenvolvido com foco no ODS 11 (Cidades e Comunidades Sustentáveis) e conta com autenticação JWT, controle de acesso por perfil e persistência em SQLite.

---

## Tecnologias

| Tecnologia | Uso |
|---|---|
| .NET 10 (ASP.NET Core) | Framework principal da API |
| Entity Framework Core 10 | ORM para acesso ao banco |
| SQLite | Banco de dados persistente |
| JWT (Bearer) | Autenticação e autorização |
| BCrypt.Net | Hash de senhas |
| Swagger (Swashbuckle) | Documentação e testes interativos |

---

## Arquitetura

```
CidadeAtivaAPI/
├── src/
│   ├── API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs          # Registro e login
│   │   │   ├── AdminController.cs         # Rotas exclusivas do admin
│   │   │   └── ProblemasUrbanosController.cs  # Rotas do cidadão
│   │   └── Program.cs                     # Configuração da aplicação e seed
│   ├── Data/
│   │   └── AppDB.cs                       # Contexto do EF Core
│   ├── DTOs/                              # Objetos de entrada e saída
│   ├── Models/
│   │   ├── Enum/
│   │   │   ├── TipoProblema.cs
│   │   │   ├── StatusProblema.cs
│   │   │   └── UserRole.cs
│   │   ├── ProblamasUrbano.cs
│   │   └── User.cs
│   └── Services/
│       ├── AuthService.cs
│       └── ProblemasService.cs
└── CidadeAtivaApi.csproj
```

---

## Enums

### TipoProblema
| Valor | Nome |
|---|---|
| 0 | Buraco |
| 1 | Iluminacao |
| 2 | Enchente |
| 3 | Calcada |
| 4 | Lixo |
| 5 | Outro |

### StatusProblema
| Valor | Nome |
|---|---|
| 0 | Aberto |
| 1 | EmAndamento |
| 2 | Resolvido |

### UserRole
| Valor | Nome |
|---|---|
| 0 | User |
| 1 | Admin |

---

## Autenticação

A API usa **JWT Bearer Token**. Toda rota (exceto `/api/auth/registrar` e `/api/auth/login`) exige o header:

```
Authorization: Bearer <token>
```

O token é retornado no login/registro e expira em **8 horas**.

### Usuário Admin padrão (seed automático)

Na primeira execução, a API cria automaticamente um usuário administrador:

| Campo | Valor |
|---|---|
| Email | admin@cidadeativa.com |
| Senha | Admin@123 |

---

## Endpoints

### Auth — `/api/auth`

#### `POST /api/auth/registrar`
Cria uma nova conta de usuário (role padrão: `User`).

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "senha123"
}
```

**Resposta `201`:**
```json
{
  "token": "<jwt>",
  "usuario": {
    "id": 1,
    "name": "João Silva",
    "email": "joao@email.com",
    "role": "User",
    "createdAt": "2026-05-31T00:00:00"
  }
}
```

---

#### `POST /api/auth/login`
Autentica um usuário e retorna o token JWT.

**Body:**
```json
{
  "email": "joao@email.com",
  "password": "senha123"
}
```

**Resposta `200`:** mesma estrutura do registro.

---

#### `POST /api/auth/logout` `[Authorize]`
Logout simbólico (JWT é stateless — o cliente descarta o token).

---

### Problemas Urbanos — `/api/problemasurbanos` `[Authorize]`

Rotas acessíveis por qualquer usuário autenticado. Cidadãos veem apenas seus próprios chamados; admins veem todos.

#### `GET /api/problemasurbanos`
Lista os chamados. Suporta filtros opcionais por query string.

| Parâmetro | Tipo | Exemplo |
|---|---|---|
| `tipo` | int | `?tipo=0` |
| `status` | int | `?status=1` |

**Resposta `200`:**
```json
[
  {
    "id": "c1fd0814-e6c1-47fb-96d4-f0d329530888",
    "titulo": "Buraco na calçada",
    "descricao": "Buraco grande na rua X",
    "tipo": "Calcada",
    "status": "Aberto",
    "bairro": "Centro",
    "criadoEm": "2026-05-31T16:48:16",
    "atualizadoEm": null,
    "userId": 2
  }
]
```

---

#### `GET /api/problemasurbanos/{id}`
Retorna um chamado pelo ID.

---

#### `POST /api/problemasurbanos`
Cria um novo chamado vinculado ao usuário autenticado.

**Body:**
```json
{
  "titulo": "Buraco na calçada",
  "descricao": "Buraco grande perto da padaria",
  "tipo": 3,
  "bairro": "Centro"
}
```

**Resposta `201`:** objeto do chamado criado.

---

#### `PUT /api/problemasurbanos/{id}` `[Admin]`
Atualiza os dados de um chamado.

**Body:**
```json
{
  "titulo": "Título atualizado",
  "descricao": "Nova descrição",
  "tipo": 0,
  "bairro": "Novo Bairro"
}
```

---

#### `DELETE /api/problemasurbanos/{id}` `[Admin]`
Remove um chamado. Retorna `204 No Content`.

---

### Admin — `/api/admin` `[Authorize(Roles = "Admin")]`

Todas as rotas abaixo exigem token de usuário com role `Admin`.

#### `GET /api/admin/chamados`
Lista todos os chamados de todos os usuários. Filtro opcional por status.

| Parâmetro | Tipo | Exemplo |
|---|---|---|
| `status` | int | `?status=0` |

---

#### `GET /api/admin/chamados/{id}`
Retorna um chamado específico pelo ID (GUID).

---

#### `PATCH /api/admin/chamados/{id}/status`
Altera o status de um chamado.

**Body:**
```json
{
  "status": 1
}
```

| Valor | Status |
|---|---|
| 0 | Aberto |
| 1 | EmAndamento |
| 2 | Resolvido |

---

#### `PATCH /api/admin/chamados/{id}/finalizar`
Atalho para marcar um chamado diretamente como `Resolvido`. Sem body.

---

#### `DELETE /api/admin/chamados/{id}`
Remove um chamado pelo ID. Retorna `204 No Content`.

---

## Como rodar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Passos

```bash
# 1. Clone o repositório
git clone https://github.com/Ianzinn/CidadeAtivaApi.git
cd CidadeAtivaApi

# 2. Execute
dotnet run --urls "http://localhost:5164"
```

O banco SQLite (`CidadeAtiva.db`) é criado automaticamente na primeira execução, junto com o usuário admin padrão.

### Swagger

Com a API rodando, acesse:

```
http://localhost:5164/swagger
```

Para testar rotas protegidas no Swagger:
1. Faça login em `POST /api/auth/login`
2. Copie o token retornado
3. Clique em **Authorize** (canto superior direito)
4. Cole no formato: `Bearer <token>`

---

## Testando com curl

### Login e captura do token
```bash
TOKEN=$(curl -s -X POST http://localhost:5164/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@cidadeativa.com","password":"Admin@123"}' \
  | grep -o '"token":"[^"]*' | cut -d'"' -f4)
```

### Listar chamados como admin
```bash
curl -s http://localhost:5164/api/admin/chamados \
  -H "Authorization: Bearer $TOKEN"
```

### Alterar status de um chamado
```bash
curl -s -X PATCH http://localhost:5164/api/admin/chamados/<id>/status \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"status": 1}'
```

### Finalizar um chamado
```bash
curl -s -X PATCH http://localhost:5164/api/admin/chamados/<id>/finalizar \
  -H "Authorization: Bearer $TOKEN"
```
