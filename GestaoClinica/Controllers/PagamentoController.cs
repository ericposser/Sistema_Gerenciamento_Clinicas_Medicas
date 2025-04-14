using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestaoClinica.Models;

namespace GestaoClinica.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly Context _context;

        public PagamentoController(Context context)
        {
            _context = context;
        }

        // GET: Pagamento
        public async Task<IActionResult> Index(DateTime? dataBusca)
        {
            ViewData["DataBusca"] = dataBusca?.ToString("yyyy-MM-dd");

            var pagamentos = _context.Pagamento
                .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
                .AsQueryable();

            if (dataBusca.HasValue)
            {
                pagamentos = pagamentos.Where(p => p.DataPagamento.Date == dataBusca.Value.Date);
            }

            return View(await pagamentos.ToListAsync());
        }
        

        // GET: Pagamento/Create
        public IActionResult Create()
        {
            ViewBag.ConsultaId = new SelectList(
                _context.Consulta
                    .Include(c => c.Paciente)
                    .Include(c => c.Medico)
                    .OrderByDescending(c => c.DataHora)
                    .ToList()
                    .Select(c => new
                    {
                        c.Id,
                        Descricao = $"{c.DataHora:dd/MM/yyyy HH:mm} - {c.Paciente.Nome} / {c.Medico.Nome}"
                    }),
                "Id", "Descricao");
            return View();
        }

        // POST: Pagamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ValorPago,DataPagamento,MetodoPagamento,ConsultaId")] Pagamento pagamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pagamento);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Pagamento cadastrado com sucesso!";
                TempData["TipoMensagem"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ConsultaId = new SelectList(_context.Consulta, "Id", "DataHora", pagamento.ConsultaId);
            return View(pagamento);
        }

        // GET: Pagamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pagamento = await _context.Pagamento
                .Include(p => p.Consulta)
                .ThenInclude(c => c.Paciente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null) return NotFound();

            // Mostra lista de consultas com nome do paciente
            ViewBag.ConsultaId = new SelectList(
                _context.Consulta.Include(c => c.Paciente)
                    .Select(c => new {
                        c.Id,
                        NomeExibicao = c.DataHora.ToString("dd/MM/yyyy HH:mm") + " - " + c.Paciente.Nome
                    }),
                "Id",
                "NomeExibicao",
                pagamento.ConsultaId
            );

            return View(pagamento);
        }

        // POST: Pagamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorPago,DataPagamento,MetodoPagamento,ConsultaId")] Pagamento pagamento)
        {
            if (id != pagamento.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ConsultaId = new SelectList(
                    _context.Consulta.Include(c => c.Paciente)
                        .Select(c => new {
                            c.Id,
                            NomeExibicao = c.DataHora.ToString("dd/MM/yyyy HH:mm") + " - " + c.Paciente.Nome
                        }),
                    "Id",
                    "NomeExibicao",
                    pagamento.ConsultaId
                );

                TempData["Mensagem"] = "Pagamento atualizado com sucesso!";
                TempData["TipoMensagem"] = "info";
                return View(pagamento);
            }
            return View(pagamento);
        }

        // GET: Pagamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagamento = await _context.Pagamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pagamento == null)
            {
                return NotFound();
            }

            return View(pagamento);
        }

        // POST: Pagamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pagamento = await _context.Pagamento.FindAsync(id);
            if (pagamento != null)
            {
                _context.Pagamento.Remove(pagamento);
            }

            await _context.SaveChangesAsync();
            TempData["Mensagem"] = "Pagamento removido com sucesso!";
            TempData["TipoMensagem"] = "error";
            return RedirectToAction(nameof(Index));
        }

        private bool PagamentoExists(int id)
        {
            return _context.Pagamento.Any(e => e.Id == id);
        }
    }
}
