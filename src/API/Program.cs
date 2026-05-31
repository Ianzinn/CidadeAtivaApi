using System.Text;
using CidadeAtivaApi.Data;
using CidadeAtivaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados SQLite
builder.Services.AddDbContext<AppDB>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<ProblemasService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Cidade Ativa API",
        Description = "API para registro de problemas urbanos — ODS 11",
        Version     = "v1"
    });
});


var app = builder.Build();

// Cria o banco de dados SQLite e seed do admin padrão
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDB>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Role == CidadeAtivaApi.Models.Enum.UserRole.Admin))
    {
        db.Users.Add(new CidadeAtivaApi.Models.User
        {
            Name         = "Administrador",
            Email        = "admin@cidadeativa.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role         = CidadeAtivaApi.Models.Enum.UserRole.Admin
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
