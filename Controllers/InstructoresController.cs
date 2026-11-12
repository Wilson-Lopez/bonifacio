using AcademiaApi.Data;
using AcademiaApi.DTOs;
using AcademiaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstructoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public InstructoresController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Lista todos los instructores.</summary>
    /// <param name="activo">Opcional: true = solo activos, false = solo inactivos.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InstructorResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InstructorResponseDto>>> GetAll([FromQuery] bool? activo)
    {
        var query = _context.Instructores.AsNoTracking();

        if (activo.HasValue)
            query = query.Where(i => i.Activo == activo.Value);

        var instructores = await query
            .OrderBy(i => i.IdInstructor)
            .Select(i => ToDto(i))
            .ToListAsync();

        return Ok(instructores);
    }

    /// <summary>Obtiene un instructor por su Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorResponseDto>> GetById(int id)
    {
        var instructor = await _context.Instructores.AsNoTracking()
            .FirstOrDefaultAsync(i => i.IdInstructor == id);

        if (instructor is null)
            return NotFound(new { mensaje = $"No existe el instructor con Id {id}" });

        return Ok(ToDto(instructor));
    }

    /// <summary>Registra un nuevo instructor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InstructorResponseDto>> Create([FromBody] InstructorRequestDto dto)
    {
        var email = dto.Email.Trim();

        if (await _context.Instructores.AnyAsync(i => i.Email == email))
            return Conflict(new { mensaje = $"Ya existe un instructor con el email {email}" });

        var instructor = new Instructor
        {
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Especialidad = dto.Especialidad.Trim(),
            Email = email,
            Activo = dto.Activo
        };

        _context.Instructores.Add(instructor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = instructor.IdInstructor }, ToDto(instructor));
    }

    /// <summary>Actualiza los datos de un instructor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(InstructorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InstructorResponseDto>> Update(int id, [FromBody] InstructorRequestDto dto)
    {
        var instructor = await _context.Instructores.FindAsync(id);

        if (instructor is null)
            return NotFound(new { mensaje = $"No existe el instructor con Id {id}" });

        var email = dto.Email.Trim();

        if (await _context.Instructores.AnyAsync(i => i.Email == email && i.IdInstructor != id))
            return Conflict(new { mensaje = $"Ya existe otro instructor con el email {email}" });

        instructor.Nombres = dto.Nombres.Trim();
        instructor.Apellidos = dto.Apellidos.Trim();
        instructor.Especialidad = dto.Especialidad.Trim();
        instructor.Email = email;
        instructor.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return Ok(ToDto(instructor));
    }

    /// <summary>Elimina un instructor.</summary>
    /// <remarks>No se puede eliminar si tiene cursos asignados; en ese caso desactívelo con PUT (Activo = false).</remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var instructor = await _context.Instructores.FindAsync(id);

        if (instructor is null)
            return NotFound(new { mensaje = $"No existe el instructor con Id {id}" });

        if (await _context.Cursos.AnyAsync(c => c.IdInstructor == id))
            return Conflict(new { mensaje = "No se puede eliminar: el instructor tiene cursos asignados. Desactívelo en su lugar." });

        _context.Instructores.Remove(instructor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static InstructorResponseDto ToDto(Instructor i) => new()
    {
        IdInstructor = i.IdInstructor,
        Nombres = i.Nombres,
        Apellidos = i.Apellidos,
        Especialidad = i.Especialidad,
        Email = i.Email,
        Activo = i.Activo
    };
}
