
using System;
using Transportation.Models;
using Xunit; 

namespace Transportation.Tests
{
    public class ShipsUnitTests
    {
        [Fact]
        public void TitanicSankSpecificDay()
        {
            var ships = new Ships();
            var titanicDate = ships.EndOfTitanic();
            Assert.Equal(15, titanicDate.Day); // Día correcto
        }

        [Fact]
        public void BritannicSankSpecificMonth()
        {
            var ships = new Ships();
            var britannicDate = ships.EndOfBritannic();
            Assert.Equal(11, britannicDate.Month); // Mes correcto (noviembre)
        }

        // Nuevo método: validar el día del Olympic
        [Fact]
        public void OlympicWasOutOfServiceSpecificDay()
        {
            var ships = new Ships();
            var olympicDate = ships.EndOfOlympic();
            Assert.Equal(12, olympicDate.Day); // Día correcto (12)
        }
    }
}
//Fuente: Copilot