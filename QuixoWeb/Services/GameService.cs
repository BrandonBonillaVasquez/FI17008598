using Microsoft.EntityFrameworkCore;
using QuixoWeb.Data;
using QuixoWeb.Models;
using QuixoWeb.Models.ViewModels;
using System.Text.Json;
using System.Xml.Linq;

namespace QuixoWeb.Services
{
    public class GameService : IGameService
    {
        private readonly QuixoDbContext _context;
        private readonly ILogger<GameService> _logger;

        public GameService(QuixoDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Gestión de Partidas

        public async Task<Partida> CrearNuevaPartidaAsync(byte modoJuego)
        {
            if (modoJuego != 2 && modoJuego != 4)
            {
                throw new ArgumentException("El modo de juego debe ser 2 o 4 jugadores");
            }

            try
            {
                var partida = new Partida
                {
                    ModoJuego = modoJuego,
                    FechaHoraCreacion = DateTime.Now,
                    TiempoTranscurrido = TimeSpan.Zero,
                    Estado = "EnCurso"
                };

                _context.Partidas.Add(partida);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Partida creada con ID: {partida.PartidaId}");

                // Crear jugada inicial con tablero vacío (todos neutros)
                var tableroInicial = CrearTableroInicial();

                // Convertir a formato serializable
                var tableroSerializable = TableroDataSerializable.FromTableroData(tableroInicial);

                // Serializar correctamente el tablero
                var tableroJson = JsonSerializer.Serialize(tableroSerializable);

                var jugadaInicial = new Jugada
                {
                    PartidaId = partida.PartidaId,
                    NumeroJugada = 0,
                    JugadorActual = 1,
                    FilaOrigen = -1,
                    ColumnaOrigen = -1,
                    FilaDestino = -1,
                    ColumnaDestino = -1,
                    EstadoTablero = tableroJson,
                    TiempoTranscurrido = TimeSpan.Zero
                };

                _context.Jugadas.Add(jugadaInicial);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Jugada inicial creada para partida {partida.PartidaId}");

                return partida;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear partida en GameService");
                throw;
            }
        }

        public async Task<GameViewModel> ObtenerEstadoPartidaAsync(int partidaId)
        {
            var partida = await _context.Partidas
                .Include(p => p.Jugadas)
                .FirstOrDefaultAsync(p => p.PartidaId == partidaId);

            if (partida == null)
            {
                throw new InvalidOperationException($"No se encontró la partida con ID {partidaId}");
            }

            var ultimaJugada = partida.Jugadas
                .OrderByDescending(j => j.NumeroJugada)
                .FirstOrDefault();

            if (ultimaJugada == null)
            {
                throw new InvalidOperationException("La partida no tiene jugadas iniciales");
            }

            var tableroSerializable = JsonSerializer.Deserialize<TableroDataSerializable>(ultimaJugada.EstadoTablero);

            // Convertir a TableroData
            var tableroData = tableroSerializable.ToTableroData();

            var viewModel = new GameViewModel
            {
                PartidaId = partida.PartidaId,
                ModoJuego = partida.ModoJuego,
                Tablero = tableroData.Simbolos,
                OrientacionesPuntos = tableroData.Orientaciones,
                JugadorActual = CalcularSiguienteJugador(ultimaJugada.JugadorActual, partida.ModoJuego),
                TiempoTranscurrido = partida.TiempoTranscurrido,
                Estado = partida.Estado,
                Ganador = ObtenerNombreGanador(partida),
                EsPrimeraVuelta = partida.Jugadas.Count <= partida.ModoJuego
            };

            return viewModel;
        }

        public async Task<bool> RealizarJugadaAsync(int partidaId, int fila, int columna, int filaDestino, int columnaDestino, byte? orientacionPunto = null)
        {
            var partida = await _context.Partidas
                .Include(p => p.Jugadas)
                .FirstOrDefaultAsync(p => p.PartidaId == partidaId);

            if (partida == null || partida.Estado != "EnCurso")
            {
                return false;
            }

            var estadoActual = await ObtenerEstadoPartidaAsync(partidaId);

            // Validaciones
            if (!EsCuboJugable(estadoActual, fila, columna))
            {
                _logger.LogWarning($"Cubo no jugable en posición ({fila}, {columna})");
                return false;
            }

            if (!EsMovimientoValido(estadoActual, fila, columna, filaDestino, columnaDestino))
            {
                _logger.LogWarning($"Movimiento inválido de ({fila}, {columna}) a ({filaDestino}, {columnaDestino})");
                return false;
            }

            // Realizar el movimiento
            var nuevoTablero = RealizarMovimiento(estadoActual, fila, columna, filaDestino, columnaDestino, orientacionPunto);

            // Guardar la jugada
            var numeroJugada = partida.Jugadas.Count;
            var jugada = new Jugada
            {
                PartidaId = partidaId,
                NumeroJugada = numeroJugada,
                JugadorActual = estadoActual.JugadorActual,
                FilaOrigen = fila,
                ColumnaOrigen = columna,
                FilaDestino = filaDestino,
                ColumnaDestino = columnaDestino,
                OrientacionPunto = orientacionPunto,
                EstadoTablero = JsonSerializer.Serialize(TableroDataSerializable.FromTableroData(nuevoTablero)),
                TiempoTranscurrido = estadoActual.TiempoTranscurrido
            };

            _context.Jugadas.Add(jugada);

            // Actualizar el tiempo en la partida
            partida.TiempoTranscurrido = estadoActual.TiempoTranscurrido;

            // Actualizar el estado del tablero para verificar victoria
            estadoActual.Tablero = nuevoTablero.Simbolos;
            estadoActual.OrientacionesPuntos = nuevoTablero.Orientaciones;

            // Verificar victoria
            var ganador = await VerificarVictoriaAsync(estadoActual);
            if (ganador != null)
            {
                partida.Estado = "Finalizada";

                partida.TiempoTranscurrido = estadoActual.TiempoTranscurrido;

                if (partida.ModoJuego == 2)
                {
                    partida.GanadorId = ganador == "Jugador1" ? 1 : 2;
                }
                else
                {
                    partida.EquipoGanador = ganador == "EquipoA" ? "A" : "B";
                }

                await ActualizarEstadisticasAsync(partida.ModoJuego, ganador);

                _logger.LogInformation($"Partida {partidaId} finalizada. Ganador: {ganador}. Tiempo: {estadoActual.TiempoTranscurrido}");
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReiniciarPartidaAsync(int partidaId)
        {
            var partida = await _context.Partidas
                .Include(p => p.Jugadas)
                .FirstOrDefaultAsync(p => p.PartidaId == partidaId);

            if (partida == null)
            {
                return false;
            }

            // Eliminar todas las jugadas excepto la inicial
            var jugadasAEliminar = partida.Jugadas.Where(j => j.NumeroJugada > 0).ToList();
            _context.Jugadas.RemoveRange(jugadasAEliminar);

            // Resetear el estado de la partida
            partida.Estado = "EnCurso";
            partida.TiempoTranscurrido = TimeSpan.Zero;
            partida.GanadorId = null;
            partida.EquipoGanador = null;
            partida.FechaHoraCreacion = DateTime.Now;

            // Actualizar jugada inicial
            var jugadaInicial = partida.Jugadas.FirstOrDefault(j => j.NumeroJugada == 0);
            if (jugadaInicial != null)
            {
                var tableroInicial = CrearTableroInicial();
                var tableroSerializable = TableroDataSerializable.FromTableroData(tableroInicial);
                jugadaInicial.EstadoTablero = JsonSerializer.Serialize(tableroSerializable);
                jugadaInicial.TiempoTranscurrido = TimeSpan.Zero;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Partida {partidaId} reiniciada");

            return true;
        }

        #endregion

        #region Validaciones

        public bool EsCuboJugable(GameViewModel juego, int fila, int columna)
        {
            // Verificar que está en la periferia
            if (!EstaEnPeriferia(fila, columna))
            {
                return false;
            }

            var simbolo = juego.Tablero[fila, columna];
            var jugadorActual = juego.JugadorActual;

            // En la primera vuelta, solo se pueden tomar cubos neutros
            if (juego.EsPrimeraVuelta)
            {
                return simbolo == "N"; // Neutro
            }

            // Para modo 2 jugadores
            if (juego.ModoJuego == 2)
            {
                var simboloJugador = jugadorActual == 1 ? "O" : "X";
                return simbolo == "N" || simbolo == simboloJugador;
            }

            // Para modo 4 jugadores
            if (juego.ModoJuego == 4)
            {
                var equipo = jugadorActual == 1 || jugadorActual == 3 ? "A" : "B";
                var simboloEquipo = equipo == "A" ? "O" : "X";

                // Si es neutro, puede jugarse
                if (simbolo == "N")
                {
                    return true;
                }

                // Si tiene el símbolo del equipo, verificar orientación del punto
                if (simbolo == simboloEquipo)
                {
                    var orientacion = juego.OrientacionesPuntos[fila, columna];
                    if (!orientacion.HasValue)
                    {
                        return true;
                    }

                    // Verificar que el punto apunta hacia el jugador actual
                    return OrientacionApuntaAJugador(fila, columna, orientacion.Value, jugadorActual);
                }

                return false;
            }

            return false;
        }

        public bool EsMovimientoValido(GameViewModel juego, int fila, int columna, int filaDestino, int columnaDestino)
        {
            // No se puede colocar en la misma posición de origen
            if (fila == filaDestino && columna == columnaDestino)
            {
                return false;
            }

            // Verificar que el destino está en la periferia
            if (!EstaEnPeriferia(filaDestino, columnaDestino))
            {
                return false;
            }

            // Verificar que el movimiento es en línea recta (misma fila o columna)
            bool mismaFila = fila == filaDestino;
            bool mismaColumna = columna == columnaDestino;

            if (!mismaFila && !mismaColumna)
            {
                return false;
            }

            // Verificar que se puede empujar desde ese extremo
            if (mismaFila)
            {
                // Movimiento horizontal
                return columnaDestino == 0 || columnaDestino == 4;
            }
            else
            {
                // Movimiento vertical
                return filaDestino == 0 || filaDestino == 4;
            }
        }

        #endregion

        #region Verificación de Victoria

        public async Task<string?> VerificarVictoriaAsync(GameViewModel juego)
        {
            return await Task.Run(() =>
            {
                // Verificar filas
                for (int fila = 0; fila < 5; fila++)
                {
                    var resultado = VerificarLinea(juego.Tablero, fila, 0, 0, 1, juego.ModoJuego, juego.JugadorActual);
                    if (resultado != null) return resultado;
                }

                // Verificar columnas
                for (int columna = 0; columna < 5; columna++)
                {
                    var resultado = VerificarLinea(juego.Tablero, 0, columna, 1, 0, juego.ModoJuego, juego.JugadorActual);
                    if (resultado != null) return resultado;
                }

                // Verificar diagonal principal
                var diagPrincipal = VerificarLinea(juego.Tablero, 0, 0, 1, 1, juego.ModoJuego, juego.JugadorActual);
                if (diagPrincipal != null) return diagPrincipal;

                // Verificar diagonal secundaria
                var diagSecundaria = VerificarLinea(juego.Tablero, 0, 4, 1, -1, juego.ModoJuego, juego.JugadorActual);
                if (diagSecundaria != null) return diagSecundaria;

                return null;
            });
        }

        private string? VerificarLinea(string[,] tablero, int filaInicio, int colInicio, int deltaFila, int deltaCol, byte modoJuego, int jugadorActual)
        {
            string primerSimbolo = tablero[filaInicio, colInicio];

            if (primerSimbolo == "N") return null; // No hay línea con neutros

            bool lineaCompleta = true;
            for (int i = 1; i < 5; i++)
            {
                int fila = filaInicio + (i * deltaFila);
                int col = colInicio + (i * deltaCol);

                if (tablero[fila, col] != primerSimbolo)
                {
                    lineaCompleta = false;
                    break;
                }
            }

            if (!lineaCompleta) return null;

            // Determinar ganador
            if (modoJuego == 2)
            {
                // Jugador 1 usa O, Jugador 2 usa X
                var jugadorLinea = primerSimbolo == "O" ? 1 : 2;

                // Si el jugador actual creó una línea del oponente, pierde
                if (jugadorLinea != jugadorActual)
                {
                    return jugadorActual == 1 ? "Jugador2" : "Jugador1";
                }

                return jugadorLinea == 1 ? "Jugador1" : "Jugador2";
            }
            else // Modo 4 jugadores
            {
                // Equipo A usa O, Equipo B usa X
                var equipoLinea = primerSimbolo == "O" ? "A" : "B";
                var equipoActual = (jugadorActual == 1 || jugadorActual == 3) ? "A" : "B";

                // Si el equipo actual creó una línea del oponente, pierde
                if (equipoLinea != equipoActual)
                {
                    return equipoActual == "A" ? "EquipoB" : "EquipoA";
                }

                return equipoLinea == "A" ? "EquipoA" : "EquipoB";
            }
        }

        #endregion

        #region Historial y Exportación

        public async Task<List<Partida>> ObtenerPartidasFinalizadasAsync()
        {
            return await _context.Partidas
                .Where(p => p.Estado == "Finalizada")
                .OrderByDescending(p => p.FechaHoraCreacion)
                .ToListAsync();
        }

        public async Task<List<Jugada>> ObtenerHistorialPartidaAsync(int partidaId)
        {
            return await _context.Jugadas
                .Where(j => j.PartidaId == partidaId)
                .OrderBy(j => j.NumeroJugada)
                .ToListAsync();
        }

        public async Task<string> ExportarPartidaXmlAsync(int partidaId)
        {
            var partida = await _context.Partidas
                .Include(p => p.Jugadas)
                .FirstOrDefaultAsync(p => p.PartidaId == partidaId);

            if (partida == null)
            {
                throw new InvalidOperationException($"No se encontró la partida con ID {partidaId}");
            }

            var xml = new XElement("Partida",
                new XAttribute("Id", partida.PartidaId),
                new XElement("ModoJuego", partida.ModoJuego),
                new XElement("FechaHoraCreacion", partida.FechaHoraCreacion.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("TiempoTranscurrido", partida.TiempoTranscurrido.ToString(@"hh\:mm\:ss")),
                new XElement("Estado", partida.Estado),
                new XElement("Ganador", ObtenerNombreGanador(partida)),
                new XElement("Jugadas",
                    partida.Jugadas.OrderBy(j => j.NumeroJugada).Select(j =>
                        new XElement("Jugada",
                            new XAttribute("Numero", j.NumeroJugada),
                            new XElement("JugadorActual", j.JugadorActual),
                            new XElement("Origen",
                                new XElement("Fila", j.FilaOrigen),
                                new XElement("Columna", j.ColumnaOrigen)
                            ),
                            new XElement("Destino",
                                new XElement("Fila", j.FilaDestino),
                                new XElement("Columna", j.ColumnaDestino)
                            ),
                            j.OrientacionPunto.HasValue ? new XElement("OrientacionPunto", j.OrientacionPunto.Value) : null,
                            new XElement("TiempoTranscurrido", j.TiempoTranscurrido.ToString(@"hh\:mm\:ss")),
                            new XElement("EstadoTablero", j.EstadoTablero)
                        )
                    )
                )
            );

            return xml.ToString();
        }

        #endregion

        #region Estadísticas

        public async Task<StatsViewModel> ObtenerEstadisticasAsync()
        {
            var estadisticas = await _context.Estadisticas.ToListAsync();

            // Obtener partidas finalizadas para calcular tiempos
            var partidas = await _context.Partidas
                .Where(p => p.Estado == "Finalizada")
                .ToListAsync();

            var viewModel = new StatsViewModel
            {
                EstadisticasDosJugadores = estadisticas
                    .Where(e => e.ModoJuego == 2)
                    .Select(e => new EstadisticaDto
                    {
                        Nombre = e.JugadorEquipo == "Jugador1" ? "Primer Jugador" : "Segundo Jugador",
                        Efectividad = e.Efectividad,
                        Ganadas = e.PartidasGanadas
                    })
                    .ToList(),

                EstadisticasCuatroJugadores = estadisticas
                    .Where(e => e.ModoJuego == 4)
                    .Select(e => new EstadisticaDto
                    {
                        Nombre = e.JugadorEquipo,
                        Efectividad = e.Efectividad,
                        Ganadas = e.PartidasGanadas
                    })
                    .ToList()
            };

            // Separar partidas por modo
            var partidasDosJugadores = partidas.Where(p => p.ModoJuego == 2).ToList();
            var partidasCuatroJugadores = partidas.Where(p => p.ModoJuego == 4).ToList();

            // Calcular resumen general
            viewModel.TotalPartidas = partidas.Count;
            viewModel.PartidasDosJugadores = partidasDosJugadores.Count;
            viewModel.PartidasCuatroJugadores = partidasCuatroJugadores.Count;

            // Calcular tiempo promedio para 2 jugadores
            if (partidasDosJugadores.Any())
            {
                var tiemposValidos = partidasDosJugadores
                    .Where(p => p.TiempoTranscurrido.TotalSeconds > 0)
                    .Select(p => p.TiempoTranscurrido.TotalSeconds)
                    .ToList();

                if (tiemposValidos.Any())
                {
                    var promedioSegundos = tiemposValidos.Average();
                    viewModel.TiempoPromedioDosJugadores = TimeSpan.FromSeconds(promedioSegundos);
                }
            }

            // Calcular tiempo promedio para 4 jugadores
            if (partidasCuatroJugadores.Any())
            {
                var tiemposValidos = partidasCuatroJugadores
                    .Where(p => p.TiempoTranscurrido.TotalSeconds > 0)
                    .Select(p => p.TiempoTranscurrido.TotalSeconds)
                    .ToList();

                if (tiemposValidos.Any())
                {
                    var promedioSegundos = tiemposValidos.Average();
                    viewModel.TiempoPromedioCuatroJugadores = TimeSpan.FromSeconds(promedioSegundos);
                }
            }

            // Calcular tiempo promedio total
            var todosLosTiempos = partidas
                .Where(p => p.TiempoTranscurrido.TotalSeconds > 0)
                .Select(p => p.TiempoTranscurrido.TotalSeconds)
                .ToList();

            if (todosLosTiempos.Any())
            {
                var promedioTotal = todosLosTiempos.Average();
                viewModel.TiempoPromedioTotal = TimeSpan.FromSeconds(promedioTotal);
            }

            return viewModel;
        }

        public async Task ActualizarEstadisticasAsync(byte modoJuego, string ganador)
        {
            if (modoJuego == 2)
            {
                // Actualizar estadísticas para modo 2 jugadores
                var stats1 = await _context.Estadisticas
                    .FirstOrDefaultAsync(e => e.ModoJuego == 2 && e.JugadorEquipo == "Jugador1");
                var stats2 = await _context.Estadisticas
                    .FirstOrDefaultAsync(e => e.ModoJuego == 2 && e.JugadorEquipo == "Jugador2");

                if (stats1 != null && stats2 != null)
                {
                    stats1.PartidasJugadas++;
                    stats2.PartidasJugadas++;

                    if (ganador == "Jugador1")
                    {
                        stats1.PartidasGanadas++;
                    }
                    else
                    {
                        stats2.PartidasGanadas++;
                    }
                }
            }
            else // Modo 4 jugadores
            {
                var statsA = await _context.Estadisticas
                    .FirstOrDefaultAsync(e => e.ModoJuego == 4 && e.JugadorEquipo == "EquipoA");
                var statsB = await _context.Estadisticas
                    .FirstOrDefaultAsync(e => e.ModoJuego == 4 && e.JugadorEquipo == "EquipoB");

                if (statsA != null && statsB != null)
                {
                    statsA.PartidasJugadas++;
                    statsB.PartidasJugadas++;

                    if (ganador == "EquipoA")
                    {
                        statsA.PartidasGanadas++;
                    }
                    else
                    {
                        statsB.PartidasGanadas++;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Métodos Auxiliares

        private TableroData CrearTableroInicial()
        {
            var tablero = new string[5, 5];
            var orientaciones = new byte?[5, 5];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    tablero[i, j] = "N"; // Todos neutros
                    orientaciones[i, j] = null;
                }
            }

            return new TableroData
            {
                Simbolos = tablero,
                Orientaciones = orientaciones
            };
        }

        private TableroData RealizarMovimiento(GameViewModel juego,
    int fila, int columna, int filaDestino, int columnaDestino, byte? orientacionPunto)
{
    var nuevoTablero = (string[,])juego.Tablero.Clone();
    var nuevasOrientaciones = (byte?[,])juego.OrientacionesPuntos.Clone();

    // 1) Tomar y vaciar origen
    var tomadoSimbolo = nuevoTablero[fila, columna];
    var tomadoOrientacion = nuevasOrientaciones[fila, columna];
    nuevoTablero[fila, columna] = "N";
    nuevasOrientaciones[fila, columna] = null;

    // 2) Desplazar en línea (fila fija o columna fija)
    if (fila == filaDestino)
    {
        if (columnaDestino < columna)
        {
            // izquierda: desplazamos [dest..orig-1] -> [dest+1..orig]
            for (int c = columna; c > columnaDestino; c--)
            {
                nuevoTablero[fila, c] = nuevoTablero[fila, c - 1];
                nuevasOrientaciones[fila, c] = nuevasOrientaciones[fila, c - 1];
            }
        }
        else
        {
            // derecha
            for (int c = columna; c < columnaDestino; c++)
            {
                nuevoTablero[fila, c] = nuevoTablero[fila, c + 1];
                nuevasOrientaciones[fila, c] = nuevasOrientaciones[fila, c + 1];
            }
        }
    }
    else
    {
        if (filaDestino < fila)
        {
            // arriba
            for (int f = fila; f > filaDestino; f--)
            {
                nuevoTablero[f, columna] = nuevoTablero[f - 1, columna];
                nuevasOrientaciones[f, columna] = nuevasOrientaciones[f - 1, columna];
            }
        }
        else
        {
            // abajo
            for (int f = fila; f < filaDestino; f++)
            {
                nuevoTablero[f, columna] = nuevoTablero[f + 1, columna];
                nuevasOrientaciones[f, columna] = nuevasOrientaciones[f + 1, columna];
            }
        }
    }

    // 3) Insertar pieza del jugador en destino
    var simboloJugador = (juego.ModoJuego == 2)
        ? (juego.JugadorActual == 1 ? "O" : "X")
        : ((juego.JugadorActual == 1 || juego.JugadorActual == 3) ? "O" : "X");

    nuevoTablero[filaDestino, columnaDestino] = simboloJugador;
    nuevasOrientaciones[filaDestino, columnaDestino] = orientacionPunto;

    return new TableroData { Simbolos = nuevoTablero, Orientaciones = nuevasOrientaciones };
}

        private bool EstaEnPeriferia(int fila, int columna)
        {
            return fila == 0 || fila == 4 || columna == 0 || columna == 4;
        }

        private bool OrientacionApuntaAJugador(int fila, int columna, byte orientacion, int jugador)
        {
            // 0=arriba, 1=derecha, 2=abajo, 3=izquierda
            // Jugador 1 = arriba, Jugador 2 = derecha, Jugador 3 = abajo, Jugador 4 = izquierda

            switch (jugador)
            {
                case 1: return orientacion == 0 && fila == 0;
                case 2: return orientacion == 1 && columna == 4;
                case 3: return orientacion == 2 && fila == 4;
                case 4: return orientacion == 3 && columna == 0;
                default: return false;
            }
        }

        private int CalcularSiguienteJugador(int jugadorActual, byte modoJuego)
        {
            if (modoJuego == 2)
            {
                return jugadorActual == 1 ? 2 : 1;
            }
            else
            {
                return (jugadorActual % 4) + 1;
            }
        }

        private string? ObtenerNombreGanador(Partida partida)
        {
            if (partida.Estado != "Finalizada") return null;

            if (partida.ModoJuego == 2)
            {
                return partida.GanadorId == 1 ? "Jugador1" : "Jugador2";
            }
            else
            {
                return partida.EquipoGanador == "A" ? "EquipoA" : "EquipoB";
            }
        }

        public async Task<bool> ActualizarTiempoPartidaAsync(int partidaId, TimeSpan tiempoTranscurrido)
        {
            var partida = await _context.Partidas.FindAsync(partidaId);

            if (partida == null || partida.Estado != "EnCurso")
            {
                return false;
            }

            partida.TiempoTranscurrido = tiempoTranscurrido;
            await _context.SaveChangesAsync();

            return true;

        }
        #endregion
    }
}