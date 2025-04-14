using System.ComponentModel.DataAnnotations;

namespace GestaoClinica.Models;

public class Medico
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; }

    [Required]
    public string CRM { get; set; }

    [Required]
    public string Telefone { get; set; }
}