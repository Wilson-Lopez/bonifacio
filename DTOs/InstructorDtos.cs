using System.ComponentModel.DataAnnotations;

namespace AcademiaApi.DTOs;

/// <summary>Datos que se envían para crear o actualizar un instructor.</summary>
public class InstructorRequestDto
{
    /// <example>Carlos</example>
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    /// <example>Ramírez Torres</example>
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    /// <example>Desarrollo Web</example>
    [Required(ErrorMessage = "La especialidad es obligatoria")]
    [MaxLength(100)]
    public string Especialidad { get; set; } = string.Empty;

    /// <example>carlos.ramirez@academia.com</example>
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <example>true</example>
    public bool Activo { get; set; } = true;
}

/// <summary>Datos que devuelve la API de un instructor.</summary>
public class InstructorResponseDto
{
    public int IdInstructor { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
