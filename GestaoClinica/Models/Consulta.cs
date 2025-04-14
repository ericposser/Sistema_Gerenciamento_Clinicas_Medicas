using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestaoClinica.Models;

public class Consulta
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Data e Hora")] 
    public DateTime DataHora { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações")] 
    public string Observacoes { get; set; }

    [Required]
    [Display(Name = "Valor da consulta")] 
    public decimal ValorConsulta { get; set; }

    // FK para Paciente
    [ForeignKey("Paciente")]
    [Display(Name = "Nome do Paciente")] 
    public int PacienteId { get; set; }

    // FK para Medico
    [ForeignKey("Medico")]
    [Display(Name = "Nome do Médico")]
    public int MedicoId { get; set; }
    
    [ValidateNever]
    public Paciente Paciente { get; set; }
    [ValidateNever]
    public Medico Medico { get; set; }
}