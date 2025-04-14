using Microsoft.EntityFrameworkCore;

namespace GestaoClinica.Models;

public class Context : DbContext
{
    public DbSet<Paciente> Paciente { get; set; }
    public DbSet<Medico> Medico { get; set; }
    public DbSet<Consulta> Consulta { get; set; }
    public DbSet<Pagamento> Pagamento { get; set; }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }
    
}