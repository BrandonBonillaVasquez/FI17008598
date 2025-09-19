using System;

namespace SumasNaturales.App
{
    internal static class Program
    {
        static void Main()
        {
            const int Max = int.MaxValue;

            Console.WriteLine("• SumFor:");
            Console.WriteLine($"\t◦ From 1 to Max → n: {FindLastValidAsc(SumFor, 1)}");
            Console.WriteLine($"\t◦ From Max to 1 → n: {FindFirstValidDesc(SumFor, Max)}");
            Console.WriteLine();
            Console.WriteLine("• SumIte:");
            Console.WriteLine("Por favor, espere. El cálculo de SumIte puede tardar un momento..."); 
            Console.WriteLine($"\t◦ From 1 to Max → n: {FindLastValidAsc(SumIte, 1)}");
            Console.WriteLine($"\t◦ From Max to 1 → n: {FindFirstValidDesc(SumIte, int.MaxValue)}");
        }

        static int SumFor(int n)
        {
            checked
            {
                return (n * (n + 1)) / 2;
            }
        }

        static int SumIte(int n)
        {
            int acc = 0;
            checked
            {
                for (int i = 1; i <= n; i++)
                {
                    acc += i;
                }
            }
            return acc;
        }

        static string FindLastValidAsc(Func<int, int> f, int max)
        {
            int lastN = 0;
            int lastSum = 0;

            for (int n = 1; n <= max; n++)
            {
                try
                {
                    int s = f(n);
                    if (s > 0)
                    {
                        lastN = n;
                        lastSum = s;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (OverflowException)
                {
                    break;
                }
            }
            return $"{lastN} → sum: {lastSum}";
        }

        static string FindFirstValidDesc(Func<int, int> f, int max)
        {
            for (int n = max; n >= 1; n--)
            {
                try
                {
                    int s = f(n);
                    if (s > 0)
                    {
                        return $"{n} → sum: {s}";
                    }
                }
                catch (OverflowException)
                {
                    // El desbordamiento es esperado. Continúe buscando.
                }
            }
            return "No se encontró un valor válido.";
        }
    }
}