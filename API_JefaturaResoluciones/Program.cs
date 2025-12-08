using API_JefaturaResoluciones.Repository;
using API_JefaturaResoluciones.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<JefaturaRepository>();

var app = builder.Build();

      app.UseSwagger();
      app.UseSwaggerUI();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.MapJefaturaPendientesEndpoints();
app.MapJefaturaResolucionesEndpoints();

app.Run();
