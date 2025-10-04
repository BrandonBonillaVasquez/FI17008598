using System.Collections.Generic;
namespace PP2.Web.Models
{
    /// <summary>
    /// Representa una fila en la tabla (etiqueta + bin/oct/dec/hex).
    /// Todos los campos son string, conforme a lo solicitado.
    /// </summary>
    public sealed class ItemResult
    {
        public string Etiqueta { get; set; } = ""; // p.ej. "a", "b", "a AND b", etc.
        public string Bin { get; set; } = "";
        public string Oct { get; set; } = "";
        public string Dec { get; set; } = "";
        public string Hex { get; set; } = "";
    }
    /// <summary>
    /// ViewModel de la vista Index: entradas + resultados.
    /// Mantiene, además, los binarios de cada ítem como strings.
    /// </summary>
    public sealed class BinaryCalcViewModel
    {
        public BinaryInputModel Input { get; set; } = new();
        public List<ItemResult> Tabla { get; set; } = new();
        // Binarios (como string) de cada ítem, para cumplir el requerimiento de "mantener todos los valores binarios".
        public string A_Bin8 { get; set; } = "";   // a (padded a 8)
        public string B_Bin8 { get; set; } = "";   // b (padded a 8)
        public string And_Bin { get; set; } = "";
        public string Or_Bin  { get; set; } = "";
        public string Xor_Bin { get; set; } = "";
        public string Sum_Bin { get; set; } = "";
        public string Mul_Bin { get; set; } = "";
    }
}
 