using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoFinal.Data;
using ProyectoFinal.Helpers;
using ProyectoFinal.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Cargar configuración de JWT desde appsettings.json
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? throw new Exception("Sección JwtSettings no encontrada.");

// 2. Convertir la clave secreta a bytes
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

// 3. Configurar autenticación JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // Sin margen de error de tiempo
        };
    });

// 4. Autorizar rutas protegidas
builder.Services.AddAuthorization();

// 5. Registrar servicios y controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<JwtService>();

// 6. Configurar conexión a base de datos MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))
    ));

// 7. Habilitar CORS para permitir peticiones externas (útil para frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 8. Mostrar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 9. Middleware en orden correcto
app.UseHttpsRedirection();
app.UseCors("AllowAll");         // CORS antes de autenticación
app.UseAuthentication();         // JWT
app.UseAuthorization();          // Reglas de acceso
app.MapControllers();            // API endpoints

app.Run();
