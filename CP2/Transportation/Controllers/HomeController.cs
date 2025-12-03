
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection; // necesario para FromKeyedServices
using Transportation.Interfaces;
using Transportation.Models;
using System.Linq;

namespace Transportation.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Inyección por clave (cada parámetro recibe su implementación correcta)
    public IActionResult Index(
        [FromKeyedServices("airbus")] IAirplanes airbus,
        [FromKeyedServices("boeing")] IAirplanes boeing)
    {
        using var db = new CarsContext();

        var minnieOwnership = db.CustomerOwnerships
            .Where(o => o.Customer.FirstName == "Minnie" && o.Customer.LastName == "Mouse")
            .Select(o => new
            {
                BrandName     = o.VinNavigation.Model.Brand.BrandName,
                ModelName     = o.VinNavigation.Model.ModelName,
                DealerName    = o.Dealer.DealerName,
                DealerAddress = o.Dealer.DealerAddress
            })
            .FirstOrDefault();

        if (minnieOwnership != null)
        {
            ViewData["BrandModel"] = $"{minnieOwnership.BrandName} - {minnieOwnership.ModelName}";
            ViewData["Dealer"]     = $"{minnieOwnership.DealerName} - {minnieOwnership.DealerAddress}";
        }
        else
        {
            ViewData["BrandModel"] = "Car not found";
            ViewData["Dealer"]     = "";
        }

        // Cada parámetro corresponde a su implementación
        ViewData["Airbus"] = $"{airbus.GetBrand}: {string.Join(" - ", airbus.GetModels)}";
        ViewData["Boeing"] = $"{boeing.GetBrand}: {string.Join(" - ", boeing.GetModels)}";

        return View();
    }
}
//Fuente: Copilot