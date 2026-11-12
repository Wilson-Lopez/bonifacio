using AcademiaApi.Data;
using AcademiaApi.DTOs;
using AcademiaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EstudiantesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstudiantesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Lista todos los estudiantes.</summary>
    /// <param name="activo">Opcional: true = solo activos, false = solo inactivos.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstudianteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EstudianteResponseDto>>> GetAll([FromQuery] bool? activo)
    {
        var query = _context.Estudiantes.AsNoTracking();

        if (activo.HasValue)
            query = query.Where(e => e.Activo == activo.Value);

        var estudiantes = await query
            .OrderBy(e => e.IdEstudiante)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(estudiantes);
    }

    /// <summary>Obtiene un estudiante por su Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteResponseDto>> GetById(int id)
    {
        var estudiante = await _context.Estudiantes.AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdEstudiante == id);

        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe el estudiante con Id {id}" });

        return Ok(ToDto(estudiante));
    }

    /// <summary>Registra un nuevo estudiante.</summary>
    /// <remarks>La FechaRegistro se asigna automáticamente con la fecha y hora actual.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudianteResponseDto>> Create([FromBody] EstudianteRequestDto dto)
    {
        var email = dto.Email.Trim();

        if (await _context.Estudiantes.AnyAsync(e => e.Email == email))
            return Conflict(new { mensaje = $"Ya existe un estudiante con el email {email}" });

        var estudiante = new Estudiante
        {
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Email = email,
            Telefono = dto.Telefono?.Trim(),
            // FechaRegistro no se envía: SQL Server la asigna con GETDATE()
            Activo = dto.Activo
        };

        _context.Estudiantes.Add(estudiante);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = estudiante.IdEstudiante }, ToDto(estudiante));
    }

    /// <summary>Actualiza los datos de un estudiante.</summary>
    /// <remarks>La FechaRegistro no se modifica.</remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EstudianteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudianteResponseDto>> Update(int id, [FromBody] EstudianteRequestDto dto)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);

        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe el estudiante con Id {id}" });

        var email = dto.Email.Trim();

        if (await _context.Estudiantes.AnyAsync(e => e.Email == email && e.IdEstudiante != id))
            return Conflict(new { mensaje = $"Ya existe otro estudiante con el email {email}" });

        estudiante.Nombres = dto.Nombres.Trim();
        estudiante.Apellidos = dto.Apellidos.Trim();
        estudiante.Email = email;
        estudiante.Telefono = dto.Telefono?.Trim();
        estudiante.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return Ok(ToDto(estudiante));
    }

    /// <summary>Elimina un estudiante.</summary>
    /// <remarks>No se puede eliminar si tiene matrículas; en ese caso desactívelo con PUT (Activo = false).</remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await _context.Estudiantes.FindAsync(id);

        if (estudiante is null)
            return NotFound(new { mensaje = $"No existe el estudiante con Id {id}" });

        if (await _context.Matriculas.AnyAsync(m => m.IdEstudiante == id))
            return Conflict(new { mensaje = "No se puede eliminar: el estudiante tiene matrículas registradas. Desactívelo en su lugar." });

        _context.Estudiantes.Remove(estudiante);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static EstudianteResponseDto ToDto(Estudiante e) => new()
    {
        IdEstudiante = e.IdEstudiante,
        Nombres = e.Nombres,
        Apellidos = e.Apellidos,
        Email = e.Email,
        Telefono = e.Telefono,
        FechaRegistro = e.FechaRegistro,
        Activo = e.Activo
    };
}
