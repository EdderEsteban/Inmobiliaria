using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Pago
{
    public int Id_Pago { get; set; }

    public int Id_Contrato { get; set; }

    public Inquilinos? Inquilino { get; set; }
    public int Id_Inquilino { get; set; }
    public Inmuebles? Inmueble { get; set; }

    public int Id_Inmueble { get; set; }

    [Required(ErrorMessage = "La fecha de es obligatoria.")]
    [Display(Name = "Fecha de Pago")]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime Fecha_Pago { get; set; }
    public decimal Monto { get; set; }

    public int Id_Usuario { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}