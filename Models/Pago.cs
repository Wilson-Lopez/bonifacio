using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademiaApi.Models;

public class Pago
{
    [Key]
    public int IdPago { get; set; }

    // FK -> Matricula
    public int IdMatricula { get; set; }

    [ForeignKey(nameof(IdMatricula))]
    public Matricula? Matricula { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; }

    [Required, MaxLength(30)]
    public string MetodoPago { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string EstadoPago { get; set; } = string.Empty;
}
