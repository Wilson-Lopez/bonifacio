using System.Reflection;
using AcademiaApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Base de datos (SQL Server) con Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Academia API",
        Version = "v1",
        Description = "API REST para la gestión de una academia: CRUD de Instructores y Estudiantes."
    });

    // Usa los comentarios /// de los controladores y DTOs como documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

// Crea la base de datos y aplica las migraciones pendientes al iniciar.
// Así, quien abra el proyecto en otra PC solo necesita ejecutarlo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex,
            "No se pudo crear o actualizar la base de datos. Revise que SQL Server esté encendido " +
            "y que la cadena 'DefaultConnection' de appsettings.json apunte a su servidor.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Academia API v1");
        c.DocumentTitle = "Academia API - Swagger";
    });
}

app.UseHttpsRedirection();

// Vista: sirve los archivos de wwwroot (index.html es la página de inicio)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
