
using System;

namespace Transportation.Models
{
    public class Ships
    {
        // Olympic fuera de servicio
        public DateTime EndOfOlympic()
        {
            return new DateTime(1935, 4, 12); // 12 abril 1935
        }

        // Titanic hundido el 15 abril 1912
        public DateTime EndOfTitanic()
        {
            return new DateTime(1912, 4, 15);
        }

        // Britannic hundido el 21 noviembre 1916
        public DateTime EndOfBritannic()
        {
            return new DateTime(1916, 11, 21);
        }
    }
}
//Fuente: Copilot