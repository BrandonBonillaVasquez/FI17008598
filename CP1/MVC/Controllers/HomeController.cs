
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Linq;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new TheModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TheModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Eliminar SOLO espacios ' ' usando LINQ a nivel de char
            var charsNoSpace = model.Phrase!.Where(c => c != ' ');

            // Conteo por carácter (Dictionary<char,int>)
            model.Counts = charsNoSpace
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            // Versiones sin espacios (minúsculas / mayúsculas)
            var arr = charsNoSpace.ToArray();
            model.Lower = new string(arr).ToLowerInvariant();
            model.Upper = new string(arr).ToUpperInvariant();

            return View(model);
        }
    }
}