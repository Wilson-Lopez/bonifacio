# Academia API

API REST en ASP.NET Core (.NET 10) con Entity Framework Core (migraciones), SQL Server y Swagger.
Implementa el CRUD de las tablas **Instructor** y **Estudiante** del modelo de la academia.

## Tecnologías

- ASP.NET Core Web API (.NET 10)
- Entity Framework Core 10 + SQL Server (Code First con migraciones)
- Swagger (Swashbuckle.AspNetCore)

## Estructura

```
AcademiaApi/
├── BaseDeDatos/   AcademiaDB.sql (script completo: tablas + datos de ejemplo)
├── Controllers/   InstructoresController, EstudiantesController
├── Data/          AppDbContext (relaciones y configuración)
├── DTOs/          Objetos de entrada y salida de la API
├── Migrations/    Migración InitialCreate (crea las 5 tablas)
├── Models/        Instructor, Estudiante, Curso, Matricula, Pago
├── wwwroot/       Vista (HTML + Bootstrap + JavaScript)
│   ├── index.html          Inicio con resumen
│   ├── instructores.html   CRUD de instructores
│   ├── estudiantes.html    CRUD de estudiantes
│   ├── css/site.css
│   └── js/                 api.js (compartido), instructores.js, estudiantes.js
└── Program.cs     Configuración de EF Core, Swagger y archivos estáticos
```

## Vista

La vista está hecha con HTML, Bootstrap 5 y JavaScript (`fetch`). Se sirve desde el mismo proyecto
y consume los endpoints de la API, por lo que no hace falta configurar CORS.

- **Inicio** (`/`): total de instructores y estudiantes registrados.
- **Instructores** (`/instructores.html`) y **Estudiantes** (`/estudiantes.html`):
  - Tabla con todos los registros (GET).
  - Buscador y filtro por estado (activos / inactivos).
  - Botón **Nuevo** que abre un formulario (POST).
  - Botón **Editar** que carga el registro por Id (GET /{id}) y lo actualiza (PUT).
  - Botón **Eliminar** con confirmación (DELETE).
  - Mensajes de éxito y de error de la API (email repetido, registro con datos relacionados, etc.).

## Cómo ejecutar (en otra PC)

Requisitos: .NET 10 SDK y SQL Server (Express o Developer).

1. Revisar la cadena de conexión en `appsettings.json`. Por defecto usa la instancia `.\SQLEXPRESS`:
   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AcademiaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   ```
   Si su SQL Server tiene otro nombre, cambie solo la parte `Server=`:
   | Instalación | Valor de `Server=` |
   |---|---|
   | SQL Server Express | `.\\SQLEXPRESS` |
   | SQL Server Developer / instancia por defecto | `localhost` |
   | LocalDB (viene con Visual Studio) | `(localdb)\\MSSQLLocalDB` |

2. Ejecutar el proyecto (desde la carpeta `AcademiaApi`):
   ```
   dotnet run --launch-profile http
   ```
   **La base de datos `AcademiaDB` se crea sola al iniciar**, con sus tablas y datos de ejemplo.
   No hace falta ejecutar ningún script.

3. Abrir la vista: http://localhost:5159
4. Abrir Swagger: http://localhost:5159/swagger

## Base de datos

La base de datos se puede crear de tres formas (todas dan el mismo resultado):

| Forma | Cómo |
|---|---|
| Automática | Al ejecutar el proyecto (`Program.cs` aplica las migraciones pendientes). |
| Script SQL | Abrir `BaseDeDatos/AcademiaDB.sql` en SQL Server Management Studio y ejecutar (F5). |
| Migraciones | `dotnet ef database update` |

### Migraciones

| Migración | Qué hace |
|---|---|
| `InitialCreate` | Crea las 5 tablas del diagrama: `Instructor`, `Estudiante`, `Curso`, `Matricula` y `Pago`, con llaves primarias, llaves foráneas e índices únicos en `Email`. |
| `DatosIniciales` | Inserta 3 instructores y 3 estudiantes de ejemplo. |

Comandos usados:

```
dotnet ef migrations add InitialCreate
dotnet ef migrations add DatosIniciales
dotnet ef database update
```

Si se agrega otra migración, el script se actualiza generando el SQL con el comando siguiente
y pegándolo en `AcademiaDB.sql` debajo de las líneas `CREATE DATABASE` / `USE [AcademiaDB]`
(sin esas líneas, SSMS crearía las tablas en la base que esté seleccionada):

```
dotnet ef migrations script --idempotent
```

## Enunciados de los endpoints

### Instructor — `/api/Instructores`

| Método | Ruta | Enunciado | Respuestas |
|---|---|---|---|
| GET | `/api/Instructores` | Lista todos los instructores. Acepta el filtro opcional `?activo=true/false`. | 200 |
| GET | `/api/Instructores/{id}` | Obtiene un instructor por su Id. | 200, 404 |
| POST | `/api/Instructores` | Registra un nuevo instructor. El email no se puede repetir. | 201, 400, 409 |
| PUT | `/api/Instructores/{id}` | Actualiza los datos de un instructor existente. | 200, 400, 404, 409 |
| DELETE | `/api/Instructores/{id}` | Elimina un instructor. No se permite si tiene cursos asignados. | 204, 404, 409 |

Cuerpo para POST / PUT:

```json
{
  "nombres": "Carlos",
  "apellidos": "Ramírez Torres",
  "especialidad": "Desarrollo Web",
  "email": "carlos.ramirez@academia.com",
  "activo": true
}
```

### Estudiante — `/api/Estudiantes`

| Método | Ruta | Enunciado | Respuestas |
|---|---|---|---|
| GET | `/api/Estudiantes` | Lista todos los estudiantes. Acepta el filtro opcional `?activo=true/false`. | 200 |
| GET | `/api/Estudiantes/{id}` | Obtiene un estudiante por su Id. | 200, 404 |
| POST | `/api/Estudiantes` | Registra un nuevo estudiante. La `FechaRegistro` se asigna automáticamente. El email no se puede repetir. | 201, 400, 409 |
| PUT | `/api/Estudiantes/{id}` | Actualiza los datos de un estudiante existente (la `FechaRegistro` no cambia). | 200, 400, 404, 409 |
| DELETE | `/api/Estudiantes/{id}` | Elimina un estudiante. No se permite si tiene matrículas registradas. | 204, 404, 409 |

Cuerpo para POST / PUT:

```json
{
  "nombres": "María",
  "apellidos": "López Quispe",
  "email": "maria.lopez@gmail.com",
  "telefono": "987654321",
  "activo": true
}
```

### Códigos de respuesta

- **200 OK**: consulta o actualización correcta.
- **201 Created**: registro creado.
- **204 No Content**: registro eliminado.
- **400 Bad Request**: faltan datos obligatorios o el email no es válido.
- **404 Not Found**: no existe un registro con ese Id.
- **409 Conflict**: el email ya está registrado, o el registro tiene datos relacionados y no se puede eliminar.
