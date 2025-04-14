using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestaoClinica.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoClinica.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Context _context;

    public HomeController(Context context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var faturamentoTotal = await _context.Pagamento.SumAsync(p => (decimal?)p.ValorPago) ?? 0;
        ViewBag.FaturamentoTotal = faturamentoTotal;
        return View();
    }
    
    // Endpoint para retornar as consultas em JSON
    [HttpGet]
    public async Task<IActionResult> ConsultasJson()
    {
        var consultas = await _context.Consulta
            .Include(c => c.Paciente)
            .Select(c => new {
                title = c.Paciente.Nome,
                start = c.DataHora.ToString("s") // formato ISO
            })
            .ToListAsync();

        return Json(consultas);
    }
    
    [HttpGet]
    public async Task<IActionResult> FaturamentoPorMes()
    {
        // 1. Gera os meses fixos de 1 a 12
        var mesesFixos = Enumerable.Range(1, 12).Select(mes => new
        {
            Mes = mes,
            Nome = new DateTime(2000, mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")),
            Valor = 0m
        }).ToList();

        // 2. Busca os valores reais do banco
        var dadosDb = await _context.Pagamento
            .Where(p => p.DataPagamento != null)
            .GroupBy(p => p.DataPagamento.Month)
            .Select(g => new
            {
                Mes = g.Key,
                Valor = g.Sum(p => p.ValorPago)
            })
            .ToListAsync();

        // 3. Mescla os dados reais com os meses fixos
        var resultado = mesesFixos
            .GroupJoin(dadosDb,
                fixo => fixo.Mes,
                db => db.Mes,
                (fixo, agrupado) => new
                {
                    Mes = fixo.Mes,
                    Nome = fixo.Nome,
                    Valor = agrupado.FirstOrDefault()?.Valor ?? 0m
                })
            .OrderBy(m => m.Mes)
            .Select(m => new
            {
                MesNome = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Nome),
                Valor = m.Valor
            });

        return Json(resultado);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}