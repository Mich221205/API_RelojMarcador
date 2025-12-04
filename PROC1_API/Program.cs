using PROC1_API;
using PROC1_API.Repository;
using PROC1_API.Services;

var builder = WebApplication.CreateBuilder(args);


// ==============================================================
// Swagger
// ==============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==============================================================
// Base de datos (MySQL)
// ==============================================================
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// ==============================================================
// Repositorios y Servicios
// ==============================================================
builder.Services.AddScoped<Proceso_Generar_Inconsistencias_Marcas>();
builder.Services.AddScoped<IProceso_Generar_Inconsistencias_MarcasService, Proceso_Generar_Inconsistencias_MarcasService>();

var app = builder.Build();

// ==============================================================
// Swagger UI
// ==============================================================

    app.UseSwagger();
    app.UseSwaggerUI();

// ==============================================================
// Endpoints
// ==============================================================
app.MapPROC1Endpoints();

// ==============================================================
// Inicio app
// ==============================================================
app.Run();
