using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using BionicLibrary;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Investments.Controllers
{
    public class PenaltyController : Controller
    {
        // 
        // GET: /Penalty/

        public IActionResult Index()
        {
            return View();
        }
        static object fun = null;
        static object cond = null;
        // 
        // GET: /Penalty/Welcome/  
        // Requires using System.Text.Encodings.Web;
        /*
        public IActionResult Result(string name, int numTimes = 1)
        {
            ViewData["x1"] = 305000;
            ViewData["x2"] = 95000;
            ViewData["f"] = 37900;
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;            

            //BionicLibrary.Class1 b = new BionicLibrary.Class1();
            //Class1.Equals(fun, cond);            

            return View();
        }
        */

        public IActionResult Result()
        {
            ViewData["x1"] = 305000;
            ViewData["x2"] = 95000;
            ViewData["f"] = 37900;
            return View();
        }
    }
}
