using System.ComponentModel.DataAnnotations;

namespace AcademiaApi.DTOs;

/// <summary>Datos que se envían para crear o actualizar un estudiante.</summary>
public class EstudianteRequestDto
{
    /// <example>María</example>
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    /// <example>López Quispe</example>
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    /// <example>maria.lopez@gmail.com</example>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <example>987654321</example>
    [MaxLength(20)]
    public string? Telefono { get; set; }

    /// <example>true</example>
    public bool Activo { get; set; } = true;
}

/// <summary>Datos que devuelve la API de un estudiante.</summary>
public class EstudianteResponseDto
{
    public int IdEstudiante { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
}
