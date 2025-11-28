using Microsoft.AspNetCore.Mvc;
using PP2.Web.Models;
using PP2.Web.Services;

namespace PP2.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Página por defecto con modelo vacío
            return View(new BinaryCalcViewModel());
        }

        [HttpPost]
        public IActionResult Index(BinaryInputModel input)
        {
            // Si hay errores de validación por DataAnnotations, regresar la vista con mensajes.
            if (!ModelState.IsValid)
            {
                return View(new BinaryCalcViewModel { Input = input });
            }

            // 1) Normalizar a y b, y preparar sus binarios a 8 bits para mostrar en la tabla
            var aNorm = BinaryService.NormalizeBin(input.A);
            var bNorm = BinaryService.NormalizeBin(input.B);

            var aBin8 = BinaryService.LeftPad(aNorm, 8);
            var bBin8 = BinaryService.LeftPad(bNorm, 8);

            // 2) Operaciones binarias (iterando chars)
            string andBin = BinaryService.AndStrings(aNorm, bNorm);
            string orBin  = BinaryService.OrStrings(aNorm, bNorm);
            string xorBin = BinaryService.XorStrings(aNorm, bNorm);

            // 3) Operaciones aritméticas (conversión a int)
            int aDec = BinaryService.BinToInt(aNorm);
            int bDec = BinaryService.BinToInt(bNorm);

            string sumBin = BinaryService.IntToBin(aDec + bDec);
            string mulBin = BinaryService.IntToBin(aDec * bDec);

            // 4) Construir la tabla (cada fila: bin/oct/dec/hex) TODO string
            var (aBin, aOct, aDecStr, aHex) = BinaryService.AllBasesFromBin(aNorm, pad8: true);
            var (bBin, bOct, bDecStr, bHex) = BinaryService.AllBasesFromBin(bNorm, pad8: true);

            var (andB, andO, andD, andH) = BinaryService.AllBasesFromBin(andBin);
            var (orB , orO , orD , orH ) = BinaryService.AllBasesFromBin(orBin);
            var (xorB, xorO, xorD, xorH) = BinaryService.AllBasesFromBin(xorBin);
            var (sumB, sumO, sumD, sumH) = BinaryService.AllBasesFromBin(sumBin);
            var (mulB, mulO, mulD, mulH) = BinaryService.AllBasesFromBin(mulBin);

            var vm = new BinaryCalcViewModel
            {
                Input = input,
                A_Bin8 = aBin8,
                B_Bin8 = bBin8,
                And_Bin = andBin,
                Or_Bin  = orBin,
                Xor_Bin = xorBin,
                Sum_Bin = sumBin,
                Mul_Bin = mulBin,
                Tabla = new()
                {
                    new ItemResult { Etiqueta = "a",         Bin = aBin,  Oct = aOct,  Dec = aDecStr, Hex = aHex },
                    new ItemResult { Etiqueta = "b",         Bin = bBin,  Oct = bOct,  Dec = bDecStr, Hex = bHex },
                    new ItemResult { Etiqueta = "a AND b",   Bin = andB,  Oct = andO,  Dec = andD,    Hex = andH },
                    new ItemResult { Etiqueta = "a OR b",    Bin = orB,   Oct = orO,   Dec = orD,     Hex = orH  },
                    new ItemResult { Etiqueta = "a XOR b",   Bin = xorB,  Oct = xorO,  Dec = xorD,    Hex = xorH },
                    new ItemResult { Etiqueta = "a + b",     Bin = sumB,  Oct = sumO,  Dec = sumD,    Hex = sumH },
                    new ItemResult { Etiqueta = "a • b",     Bin = mulB,  Oct = mulO,  Dec = mulD,    Hex = mulH },
                }
            };

            return View(vm);
        }
    }
}