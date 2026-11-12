/* =====================================================================
   Base de datos: AcademiaDB
   Proyecto:      AcademiaApi (ASP.NET Core + Entity Framework Core)

   Script generado a partir de las migraciones de EF Core:
     - InitialCreate   -> crea las tablas Instructor, Estudiante, Curso,
                          Matricula y Pago con sus llaves e índices
     - DatosIniciales  -> inserta instructores y estudiantes de ejemplo

   Uso: abrir en SQL Server Management Studio y ejecutar (F5).
   Se puede ejecutar varias veces: solo aplica lo que falte.

   Para regenerarlo (desde la carpeta AcademiaApi), ejecutar el comando
   y pegar su resultado debajo de las líneas CREATE DATABASE / USE:
     dotnet ef migrations script --idempotent
   ===================================================================== */

IF DB_ID(N'AcademiaDB') IS NULL
    CREATE DATABASE [AcademiaDB];
GO

USE [AcademiaDB];
GO
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE TABLE [Estudiante] (
        [IdEstudiante] int NOT NULL IDENTITY,
        [Nombres] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Telefono] nvarchar(20) NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Estudiante] PRIMARY KEY ([IdEstudiante])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE TABLE [Instructor] (
        [IdInstructor] int NOT NULL IDENTITY,
        [Nombres] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [Especialidad] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Instructor] PRIMARY KEY ([IdInstructor])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE TABLE [Curso] (
        [IdCurso] int NOT NULL IDENTITY,
        [Nombre] nvarchar(150) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Precio] decimal(10,2) NOT NULL,
        [DuracionHoras] int NOT NULL,
        [IdInstructor] int NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Curso] PRIMARY KEY ([IdCurso]),
        CONSTRAINT [FK_Curso_Instructor_IdInstructor] FOREIGN KEY ([IdInstructor]) REFERENCES [Instructor] ([IdInstructor]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE TABLE [Matricula] (
        [IdMatricula] int NOT NULL IDENTITY,
        [IdEstudiante] int NOT NULL,
        [IdCurso] int NOT NULL,
        [FechaMatricula] datetime2 NOT NULL,
        [Estado] nvarchar(30) NOT NULL,
        [MontoTotal] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_Matricula] PRIMARY KEY ([IdMatricula]),
        CONSTRAINT [FK_Matricula_Curso_IdCurso] FOREIGN KEY ([IdCurso]) REFERENCES [Curso] ([IdCurso]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Matricula_Estudiante_IdEstudiante] FOREIGN KEY ([IdEstudiante]) REFERENCES [Estudiante] ([IdEstudiante]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE TABLE [Pago] (
        [IdPago] int NOT NULL IDENTITY,
        [IdMatricula] int NOT NULL,
        [Monto] decimal(10,2) NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [MetodoPago] nvarchar(30) NOT NULL,
        [EstadoPago] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_Pago] PRIMARY KEY ([IdPago]),
        CONSTRAINT [FK_Pago_Matricula_IdMatricula] FOREIGN KEY ([IdMatricula]) REFERENCES [Matricula] ([IdMatricula]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Curso_IdInstructor] ON [Curso] ([IdInstructor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Estudiante_Email] ON [Estudiante] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Instructor_Email] ON [Instructor] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Matricula_IdCurso] ON [Matricula] ([IdCurso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Matricula_IdEstudiante] ON [Matricula] ([IdEstudiante]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Pago_IdMatricula] ON [Pago] ([IdMatricula]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112220204_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261112220204_InitialCreate', N'10.0.15');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112222428_DatosIniciales'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdEstudiante', N'Activo', N'Apellidos', N'Email', N'FechaRegistro', N'Nombres', N'Telefono') AND [object_id] = OBJECT_ID(N'[Estudiante]'))
        SET IDENTITY_INSERT [Estudiante] ON;
    EXEC(N'INSERT INTO [Estudiante] ([IdEstudiante], [Activo], [Apellidos], [Email], [FechaRegistro], [Nombres], [Telefono])
    VALUES (1, CAST(1 AS bit), N''López Quispe'', N''maria.lopez@alumnos.academia.com'', ''2026-03-02T09:15:00.0000000'', N''María'', N''987654321''),
    (2, CAST(1 AS bit), N''Pérez Huamán'', N''jose.perez@alumnos.academia.com'', ''2026-03-05T14:30:00.0000000'', N''José'', N''912345678''),
    (3, CAST(1 AS bit), N''Torres Mendoza'', N''lucia.torres@alumnos.academia.com'', ''2026-04-10T10:00:00.0000000'', N''Lucía'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdEstudiante', N'Activo', N'Apellidos', N'Email', N'FechaRegistro', N'Nombres', N'Telefono') AND [object_id] = OBJECT_ID(N'[Estudiante]'))
        SET IDENTITY_INSERT [Estudiante] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112222428_DatosIniciales'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdInstructor', N'Activo', N'Apellidos', N'Email', N'Especialidad', N'Nombres') AND [object_id] = OBJECT_ID(N'[Instructor]'))
        SET IDENTITY_INSERT [Instructor] ON;
    EXEC(N'INSERT INTO [Instructor] ([IdInstructor], [Activo], [Apellidos], [Email], [Especialidad], [Nombres])
    VALUES (1, CAST(1 AS bit), N''Ramírez Torres'', N''carlos.ramirez@academia.com'', N''Desarrollo Web'', N''Carlos''),
    (2, CAST(1 AS bit), N''Gómez Salazar'', N''ana.gomez@academia.com'', N''Base de Datos'', N''Ana''),
    (3, CAST(0 AS bit), N''Fernández Castro'', N''luis.fernandez@academia.com'', N''Redes y Seguridad'', N''Luis'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdInstructor', N'Activo', N'Apellidos', N'Email', N'Especialidad', N'Nombres') AND [object_id] = OBJECT_ID(N'[Instructor]'))
        SET IDENTITY_INSERT [Instructor] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261112222428_DatosIniciales'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261112222428_DatosIniciales', N'10.0.15');
END;

COMMIT;
GO

