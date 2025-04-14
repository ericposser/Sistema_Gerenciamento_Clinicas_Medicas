namespace GestaoClinica.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Paciente
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; }

    [Required]
    public string CPF { get; set; }

    [Required]
    public string Telefone { get; set; }
}