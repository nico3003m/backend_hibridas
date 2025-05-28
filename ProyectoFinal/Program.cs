using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:5220");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))));

// Agregar soporte para cache en memoria (necesario para Session)
builder.Services.AddDistributedMemoryCache();

// Agregar soporte para Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;  // Para que se envíe siempre la cookie incluso si no hay consentimiento de cookies
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowAll");


app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
