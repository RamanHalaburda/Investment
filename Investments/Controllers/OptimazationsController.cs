using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Investments.Models;

namespace Investments.Controllers
{
    public class OptimazationsController : Controller
    {
        private readonly InvestmentsContext _context;

        public OptimazationsController(InvestmentsContext context)
        {
            _context = context;
        }

        // GET: Optimazations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Optimazation.ToListAsync());
        }

        // GET: Optimazations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optimazation = await _context.Optimazation
                .SingleOrDefaultAsync(m => m.ID == id);
            if (optimazation == null)
            {
                return NotFound();
            }

            return View(optimazation);
        }

        // GET: Optimazations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Optimazations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,date,dividentsA,dividentsB,sum,limitA,limitB,investmentA,investmentB,result")] Optimazation optimazation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(optimazation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(optimazation);
        }

        // GET: Optimazations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optimazation = await _context.Optimazation.SingleOrDefaultAsync(m => m.ID == id);
            if (optimazation == null)
            {
                return NotFound();
            }
            return View(optimazation);
        }

        // POST: Optimazations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,date,dividentsA,dividentsB,sum,limitA,limitB,investmentA,investmentB,result")] Optimazation optimazation)
        {
            if (id != optimazation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(optimazation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptimazationExists(optimazation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(optimazation);
        }

        // GET: Optimazations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optimazation = await _context.Optimazation
                .SingleOrDefaultAsync(m => m.ID == id);
            if (optimazation == null)
            {
                return NotFound();
            }

            return View(optimazation);
        }

        // POST: Optimazations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var optimazation = await _context.Optimazation.SingleOrDefaultAsync(m => m.ID == id);
            _context.Optimazation.Remove(optimazation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OptimazationExists(int id)
        {
            return _context.Optimazation.Any(e => e.ID == id);
        }
    }
}
