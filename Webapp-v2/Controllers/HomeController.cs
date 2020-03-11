using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp_v2.Data;
using Webapp_v2.Models;
using Webapp_v2.Models.SchoolViewModels;

namespace Webapp_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;
        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> About()
        {
            IQueryable<EnrollmentDateGroup> data = from student in _context.Students
                                                   group student by student.EnrollmentDate into dateGroup
                                                   select new EnrollmentDateGroup()
                                                   {
                                                       EnrollmentDate = dateGroup.Key,
                                                       StudentCount = dateGroup.Count()
                                                   };
            return View(await data.AsNoTracking().ToListAsync());
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
