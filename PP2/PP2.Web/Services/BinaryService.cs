using System;
using System.Text;

namespace PP2.Web.Services
{
    public static class BinaryService
    {
        /// <summary>Elimina ceros a la izquierda, dejando "0" si se queda vacío.</summary>
        public static string NormalizeBin(string bin)
        {
            var t = (bin ?? "").Trim();
            if (t.Length == 0) return "0";
            int i = 0;
            while (i < t.Length - 1 && t[i] == '0') i++;
            return t.Substring(i);
        }

        /// <summary>Rellena por la izquierda con '0' hasta alcanzar 'len'.</summary>
        public static string LeftPad(string s, int len)
            => (s ?? "").PadLeft(len, '0');

        /// <summary>Alinea a y b por la derecha (completando con '0') para operar bit a bit.</summary>
        private static (string A, string B) Align(string a, string b)
        {
            var aN = NormalizeBin(a);
            var bN = NormalizeBin(b);
            int n = Math.Max(aN.Length, bN.Length);
            return (LeftPad(aN, n), LeftPad(bN, n));
        }

        /// <summary>Operación binaria AND sobre strings iterando caracteres.</summary>
        public static string AndStrings(string a, string b)
        {
            (a, b) = Align(a, b);
            var sb = new StringBuilder(a.Length);
            for (int i = 0; i < a.Length; i++)
                sb.Append((a[i] == '1' && b[i] == '1') ? '1' : '0');
            return NormalizeBin(sb.ToString());
        }

        /// <summary>Operación binaria OR sobre strings iterando caracteres.</summary>
        public static string OrStrings(string a, string b)
        {
            (a, b) = Align(a, b);
            var sb = new StringBuilder(a.Length);
            for (int i = 0; i < a.Length; i++)
                sb.Append((a[i] == '1' || b[i] == '1') ? '1' : '0');
            return NormalizeBin(sb.ToString());
        }

        /// <summary>Operación binaria XOR sobre strings iterando caracteres.</summary>
        public static string XorStrings(string a, string b)
        {
            (a, b) = Align(a, b);
            var sb = new StringBuilder(a.Length);
            for (int i = 0; i < a.Length; i++)
                sb.Append((a[i] != b[i]) ? '1' : '0');
            return NormalizeBin(sb.ToString());
        }

        public static int BinToInt(string bin) => Convert.ToInt32(NormalizeBin(bin), 2);
        public static string IntToBin(int n) => Convert.ToString(n, 2);

        /// <summary>Devuelve (bin, oct, dec, hex) como strings. Si pad8=true, bin se muestra a 8 bits.</summary>
        public static (string Bin, string Oct, string Dec, string Hex) AllBasesFromBin(string bin, bool pad8 = false)
        {
            string binNorm = NormalizeBin(bin);
            int dec = BinToInt(binNorm);
            string binOut = pad8 ? LeftPad(binNorm, 8) : binNorm;
            string oct = Convert.ToString(dec, 8);
            string hex = Convert.ToString(dec, 16).ToUpperInvariant();
            return (binOut, oct, dec.ToString(), hex);
        }
    }
}