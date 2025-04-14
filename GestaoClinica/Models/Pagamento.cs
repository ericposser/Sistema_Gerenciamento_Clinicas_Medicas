using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestaoClinica.Models;

public class Pagamento
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Valor Pago")] 
    public decimal ValorPago { get; set; }

    [Required]
    [Display(Name = "Data de pagamento")] 
    public DateTime DataPagamento { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Método de pagamento")] 
    public string MetodoPagamento { get; set; }

    // FK para Consulta
    [ForeignKey("Consulta")]
    [Display(Name = "Consulta Referente")] 
    public int ConsultaId { get; set; }
    
    [ValidateNever]
    public Consulta Consulta{ get; set; }
}