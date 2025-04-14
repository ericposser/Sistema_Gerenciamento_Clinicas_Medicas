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
    public class ConsultaController : Controller
    {
        private readonly Context _context;

        public ConsultaController(Context context)
        {
            _context = context;
        }

        // GET: Consulta
        public async Task<IActionResult> Index(DateTime? dataBusca)
        {
            ViewData["DataBusca"] = dataBusca?.ToString("yyyy-MM-dd");

            var consultas = _context.Consulta
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .AsQueryable();

            if (dataBusca.HasValue)
            {
                consultas = consultas.Where(c => c.DataHora.Date == dataBusca.Value.Date);
            }

            return View(await consultas.ToListAsync());
        }

        // GET: Consulta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // GET: Consulta/Create
        // GET: Consulta/Create
        public IActionResult Create()
        {
            ViewBag.PacienteId = new SelectList(_context.Paciente, "Id", "Nome");
            ViewBag.MedicoId = new SelectList(_context.Medico, "Id", "Nome");
            return View();
        }

        // POST: Consulta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,Observacoes,ValorConsulta,PacienteId,MedicoId")] Consulta consulta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consulta);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Consulta cadastrada com sucesso!";
                TempData["TipoMensagem"] = "success";
                return RedirectToAction(nameof(Index));
            }

            // Recarrega selects em caso de erro no ModelState
            ViewBag.PacienteId = new SelectList(_context.Paciente, "Id", "Nome", consulta.PacienteId);
            ViewBag.MedicoId = new SelectList(_context.Medico, "Id", "Nome", consulta.MedicoId);

            return View(consulta);
        }

        // GET: Consulta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (consulta == null)
            {
                return NotFound();
            }
            ViewData["PacienteId"] = new SelectList(_context.Paciente, "Id", "Nome", consulta.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medico, "Id", "Nome", consulta.MedicoId);
            return View(consulta);
        }

        // POST: Consulta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,Observacoes,ValorConsulta,PacienteId,MedicoId")] Consulta consulta)
        {
            if (id != consulta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaExists(consulta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Mensagem"] = "Consulta atualizada com sucesso!";
                TempData["TipoMensagem"] = "info";

                return RedirectToAction(nameof(Index));
            }

            // ⚠️ Adicione isso para popular os selects novamente em caso de erro:
            ViewBag.PacienteId = new SelectList(_context.Paciente, "Id", "Nome", consulta.PacienteId);
            ViewBag.MedicoId = new SelectList(_context.Medico, "Id", "Nome", consulta.MedicoId);

            return View(consulta);
        }

        // GET: Consulta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Consulta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consulta.FindAsync(id);
            if (consulta != null)
            {
                _context.Consulta.Remove(consulta);
            }

            await _context.SaveChangesAsync();
            TempData["Mensagem"] = "Consulta excluída com sucesso!";
            TempData["TipoMensagem"] = "warning";
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultaExists(int id)
        {
            return _context.Consulta.Any(e => e.Id == id);
        }
    }
}
