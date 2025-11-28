namespace QuixoWeb.Models
{
    public class TableroDataSerializable
    {
        public List<List<string>> Simbolos { get; set; } = new();
        public List<List<int?>> Orientaciones { get; set; } = new();

        public static TableroDataSerializable FromTableroData(TableroData data)
        {
            var result = new TableroDataSerializable();

            for (int i = 0; i < 5; i++)
            {
                var filaSimbolos = new List<string>();
                var filaOrientaciones = new List<int?>();

                for (int j = 0; j < 5; j++)
                {
                    filaSimbolos.Add(data.Simbolos[i, j]);
                    filaOrientaciones.Add(data.Orientaciones[i, j]);
                }

                result.Simbolos.Add(filaSimbolos);
                result.Orientaciones.Add(filaOrientaciones);
            }

            return result;
        }

        public TableroData ToTableroData()
        {
            var tablero = new string[5, 5];
            var orientaciones = new byte?[5, 5];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    tablero[i, j] = Simbolos[i][j];
                    orientaciones[i, j] = Orientaciones[i][j].HasValue
                        ? (byte?)Orientaciones[i][j].Value
                        : null;
                }
            }

            return new TableroData
            {
                Simbolos = tablero,
                Orientaciones = orientaciones
            };
        }
    }

    public class TableroData
    {
        public string[,] Simbolos { get; set; } = new string[5, 5];
        public byte?[,] Orientaciones { get; set; } = new byte?[5, 5];
    }
}