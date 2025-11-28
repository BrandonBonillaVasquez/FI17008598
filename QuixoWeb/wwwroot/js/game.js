// ========================================
// QUIXO - LÓGICA DEL JUEGO
// ========================================

class QuixoGame {
    constructor(partidaId, modoJuego) {
        this.partidaId = partidaId;
        this.modoJuego = modoJuego;
        this.selectedCube = null;
        this.gameState = null;
        this.isGameActive = true;

        this.init();
    }

    // Inicializar el juego
    init() {
        this.attachEventListeners();
        this.loadGameState();
    }

    // Adjuntar event listeners a los cubos
    attachEventListeners() {
        const cubes = document.querySelectorAll('.cube');

        cubes.forEach(cube => {
            cube.addEventListener('click', (e) => this.handleCubeClick(e));

            // Efecto hover para cubos jugables
            cube.addEventListener('mouseenter', (e) => this.handleCubeHover(e));
            cube.addEventListener('mouseleave', (e) => this.handleCubeLeave(e));
        });
    }

    // Manejar clic en cubo
    handleCubeClick(event) {
        if (!this.isGameActive) {
            this.showMessage('La partida ha finalizado', 'warning');
            return;
        }

        const cube = event.currentTarget;
        const row = parseInt(cube.dataset.row);
        const col = parseInt(cube.dataset.col);

        // Si no hay cubo seleccionado, intentar seleccionar este
        if (!this.selectedCube) {
            this.selectCube(cube, row, col);
        } else {
            // Si ya hay un cubo seleccionado, intentar mover a esta posición
            this.moveToPosition(row, col);
        }
    }

    // Seleccionar un cubo
    async selectCube(cube, row, col) {
        // Verificar si el cubo es jugable
        const isPlayable = await this.isCubePlayable(row, col);

        if (!isPlayable) {
            this.showMessage('Este cubo no se puede jugar', 'danger');
            return;
        }

        // Verificar que está en la periferia
        if (!this.isPeriphery(row, col)) {
            this.showMessage('Solo puedes seleccionar cubos de la periferia', 'danger');
            return;
        }

        // Deseleccionar cubo anterior si existe
        if (this.selectedCube) {
            this.selectedCube.element.classList.remove('selected');
        }

        // Seleccionar nuevo cubo
        this.selectedCube = { element: cube, row, col };
        cube.classList.add('selected');

        // Resaltar posibles destinos
        this.highlightValidDestinations(row, col);

        this.showMessage('Selecciona el destino en la periferia', 'info');
    }

    // Mover a una posición
    async moveToPosition(destRow, destCol) {
        if (!this.selectedCube) return;

        const { row: originRow, col: originCol } = this.selectedCube;

        // Verificar que el destino es válido
        if (!this.isValidDestination(originRow, originCol, destRow, destCol)) {
            this.showMessage('Destino inválido. Debe ser un extremo de la fila/columna', 'danger');
            return;
        }

        // Si es modo 4 jugadores, mostrar modal para orientación
        if (this.modoJuego === 4) {
            this.showOrientationModal(originRow, originCol, destRow, destCol);
        } else {
            // Modo 2 jugadores - realizar movimiento directamente
            await this.executeMove(originRow, originCol, destRow, destCol, null);
        }
    }

    // Ejecutar el movimiento
    async executeMove(originRow, originCol, destRow, destCol, orientation) {
        try {
            const response = await fetch('/Game/MakeMove', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    partidaId: this.partidaId,
                    filaOrigen: originRow,
                    columnaOrigen: originCol,
                    filaDestino: destRow,
                    columnaDestino: destCol,
                    orientacionPunto: orientation
                })
            });

            // Verificar si la respuesta es exitosa
            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error del servidor:', errorText);
                this.showMessage('Error en el servidor al procesar el movimiento', 'danger');
                this.clearSelection();
                return;
            }

            const result = await response.json();

            if (result.success) {
                // Actualizar el tablero con el nuevo estado
                this.updateBoard(result.gameState);

                // Limpiar selección
                this.clearSelection();

                // Verificar si hay ganador
                if (result.gameState.estado === 'Finalizada') {
                    this.handleGameEnd(result.gameState.ganador);
                } else {
                    this.showMessage(`Turno del Jugador ${result.gameState.jugadorActual}`, 'success');
                }
            } else {
                this.showMessage(result.message || 'Movimiento inválido', 'danger');
                this.clearSelection();
            }
        } catch (error) {
            console.error('Error al realizar movimiento:', error);
            this.showMessage('Error de conexión al procesar el movimiento', 'danger');
            this.clearSelection();
        }
    }

    // Actualizar el tablero visual
    updateBoard(gameState) {
        const tablero = gameState.tablero;
        const orientaciones = gameState.orientaciones;

        for (let i = 0; i < 5; i++) {
            for (let j = 0; j < 5; j++) {
                const cube = document.querySelector(`[data-row="${i}"][data-col="${j}"]`);
                if (!cube) continue;

                // Acceder como lista en lugar de array multidimensional
                const symbol = tablero[i][j];
                const orientation = orientaciones ? orientaciones[i][j] : null;

                // Actualizar símbolo
                this.updateCubeSymbol(cube, symbol, orientation);
            }
        }

        // Actualizar indicador de jugador actual
        this.updateCurrentPlayer(gameState.jugadorActual);

        // Actualizar primera vuelta
        const esPrimeraVueltaInput = document.getElementById('esPrimeraVuelta');
        if (esPrimeraVueltaInput) {
            esPrimeraVueltaInput.value = gameState.esPrimeraVuelta.toString().toLowerCase();
        }
    }

    // Actualizar símbolo del cubo
    updateCubeSymbol(cube, symbol, orientation) {
        const content = cube.querySelector('.cube-content');
        content.innerHTML = '';

        if (symbol === 'O') {
            const svg = this.createCircleSVG(orientation);
            const div = document.createElement('div');
            div.className = 'symbol circle';
            div.innerHTML = svg;
            content.appendChild(div);
            cube.classList.add('has-symbol');
            cube.dataset.symbol = 'O';
        } else if (symbol === 'X') {
            const svg = this.createCrossSVG(orientation);
            const div = document.createElement('div');
            div.className = 'symbol cross';
            div.innerHTML = svg;
            content.appendChild(div);
            cube.classList.add('has-symbol');
            cube.dataset.symbol = 'X';
        } else {
            cube.classList.remove('has-symbol');
            cube.dataset.symbol = 'N';
        }

        if (orientation !== null && orientation !== undefined) {
            cube.dataset.orientation = orientation;
        }
    }

    // Crear SVG de círculo
    createCircleSVG(orientation) {
        let dotY = '50';
        if (orientation === 0) dotY = '20';
        else if (orientation === 2) dotY = '80';

        const dot = this.modoJuego === 4 && orientation !== null && orientation !== undefined
            ? `<circle cx="50" cy="${dotY}" r="5" fill="currentColor"/>`
            : '';

        return `
            <svg viewBox="0 0 100 100">
                <circle cx="50" cy="50" r="35" stroke="currentColor" stroke-width="8" fill="none"/>
                ${dot}
            </svg>
        `;
    }

    // Crear SVG de cruz
    createCrossSVG(orientation) {
        let dotY = '50';
        if (orientation === 0) dotY = '20';
        else if (orientation === 2) dotY = '80';

        const dot = this.modoJuego === 4 && orientation !== null && orientation !== undefined
            ? `<circle cx="50" cy="${dotY}" r="5" fill="currentColor"/>`
            : '';

        return `
            <svg viewBox="0 0 100 100">
                <line x1="20" y1="20" x2="80" y2="80" stroke="currentColor" stroke-width="8"/>
                <line x1="80" y1="20" x2="20" y2="80" stroke="currentColor" stroke-width="8"/>
                ${dot}
            </svg>
        `;
    }

    // Verificar si un cubo es jugable
    async isCubePlayable(row, col) {
        try {
            const jugadorActual = document.getElementById('jugadorActual')?.value;
            const esPrimeraVuelta = document.getElementById('esPrimeraVuelta')?.value === 'true';

            if (!jugadorActual) {
                console.error('No se pudo obtener el jugador actual');
                return false;
            }

            const cube = document.querySelector(`[data-row="${row}"][data-col="${col}"]`);
            if (!cube) {
                console.error('Cubo no encontrado');
                return false;
            }

            const symbol = cube.dataset.symbol || 'N';

            // Primera vuelta: solo neutros
            if (esPrimeraVuelta) {
                return symbol === 'N';
            }

            // Modo 2 jugadores
            if (this.modoJuego === 2) {
                const playerSymbol = jugadorActual === '1' ? 'O' : 'X';
                return symbol === 'N' || symbol === playerSymbol;
            }

            // Modo 4 jugadores (verificar orientación)
            if (this.modoJuego === 4) {
                const team = (jugadorActual === '1' || jugadorActual === '3') ? 'A' : 'B';
                const teamSymbol = team === 'A' ? 'O' : 'X';

                if (symbol === 'N') return true;
                if (symbol !== teamSymbol) return false;

                // Verificar orientación del punto
                const orientation = cube.dataset.orientation;
                if (!orientation) return true;

                return this.orientationPointsToPlayer(row, col, parseInt(orientation), parseInt(jugadorActual));
            }

            return false;
        } catch (error) {
            console.error('Error verificando cubo jugable:', error);
            return false;
        }
    }

    // Verificar si está en la periferia
    isPeriphery(row, col) {
        return row === 0 || row === 4 || col === 0 || col === 4;
    }

    // Verificar si el destino es válido
    isValidDestination(originRow, originCol, destRow, destCol) {
        // No puede ser la misma posición
        if (originRow === destRow && originCol === destCol) {
            return false;
        }

        // Debe estar en la periferia
        if (!this.isPeriphery(destRow, destCol)) {
            return false;
        }

        // Debe ser la misma fila o columna
        const sameRow = originRow === destRow;
        const sameCol = originCol === destCol;

        if (!sameRow && !sameCol) {
            return false;
        }

        // Si es la misma fila, el destino debe ser columna 0 o 4
        if (sameRow) {
            return destCol === 0 || destCol === 4;
        }

        // Si es la misma columna, el destino debe ser fila 0 o 4
        if (sameCol) {
            return destRow === 0 || destRow === 4;
        }

        return false;
    }

    // Verificar si la orientación apunta al jugador
    orientationPointsToPlayer(row, col, orientation, player) {
        // 0=arriba, 1=derecha, 2=abajo, 3=izquierda
        // Jugador 1=arriba, 2=derecha, 3=abajo, 4=izquierda

        switch (player) {
            case 1: return orientation === 0 && row === 0;
            case 2: return orientation === 1 && col === 4;
            case 3: return orientation === 2 && row === 4;
            case 4: return orientation === 3 && col === 0;
            default: return false;
        }
    }

    // Resaltar destinos válidos
    highlightValidDestinations(originRow, originCol) {
        // Limpiar resaltados anteriores
        document.querySelectorAll('.cube').forEach(c => c.classList.remove('playable'));

        // Resaltar posibles destinos
        if (originRow === 0 || originRow === 4) {
            // Fila superior o inferior - resaltar extremos izquierdo y derecho
            const leftCube = document.querySelector(`[data-row="${originRow}"][data-col="0"]`);
            const rightCube = document.querySelector(`[data-row="${originRow}"][data-col="4"]`);

            if (leftCube && originCol !== 0) leftCube.classList.add('playable');
            if (rightCube && originCol !== 4) rightCube.classList.add('playable');
        }

        if (originCol === 0 || originCol === 4) {
            // Columna izquierda o derecha - resaltar extremos superior e inferior
            const topCube = document.querySelector(`[data-row="0"][data-col="${originCol}"]`);
            const bottomCube = document.querySelector(`[data-row="4"][data-col="${originCol}"]`);

            if (topCube && originRow !== 0) topCube.classList.add('playable');
            if (bottomCube && originRow !== 4) bottomCube.classList.add('playable');
        }
    }

    // Mostrar modal de orientación (modo 4 jugadores)
    showOrientationModal(originRow, originCol, destRow, destCol) {
        const modal = new bootstrap.Modal(document.getElementById('orientationModal'));
        modal.show();

        // Manejar selección de orientación
        const buttons = document.querySelectorAll('.orientation-selector button');
        buttons.forEach(btn => {
            btn.onclick = async () => {
                const orientation = parseInt(btn.dataset.orientation);
                modal.hide();
                await this.executeMove(originRow, originCol, destRow, destCol, orientation);
            };
        });
    }

    // Limpiar selección
    clearSelection() {
        if (this.selectedCube) {
            this.selectedCube.element.classList.remove('selected');
            this.selectedCube = null;
        }

        // Limpiar resaltados
        document.querySelectorAll('.cube').forEach(c => {
            c.classList.remove('playable', 'selected');
        });
    }

    // Manejar hover del cubo
    handleCubeHover(event) {
        const cube = event.currentTarget;
        const row = parseInt(cube.dataset.row);
        const col = parseInt(cube.dataset.col);

        if (this.isPeriphery(row, col) && !this.selectedCube) {
            cube.classList.add('peripheral-highlight');
        }
    }

    // Manejar salida del hover
    handleCubeLeave(event) {
        const cube = event.currentTarget;
        cube.classList.remove('peripheral-highlight');
    }

    // Actualizar indicador de jugador actual
    updateCurrentPlayer(jugadorActual) {
        const badge = document.getElementById('currentPlayerBadge');
        const hiddenInput = document.getElementById('jugadorActual');

        if (hiddenInput) {
            hiddenInput.value = jugadorActual;
        }

        if (badge) {
            if (this.modoJuego === 2) {
                const symbol = jugadorActual === 1 ? 'O' : 'X';
                badge.textContent = `Jugador ${jugadorActual} (${symbol})`;
            } else {
                const team = (jugadorActual === 1 || jugadorActual === 3) ? 'Equipo A' : 'Equipo B';
                badge.textContent = `Jugador ${jugadorActual} (${team})`;
            }
        }
    }

    // Manejar fin del juego
    handleGameEnd(ganador) {
        this.isGameActive = false;

        // Detener el reloj
        if (gameClock) {
            gameClock.pause();
        }

        // Mostrar mensaje de victoria
        let mensajeGanador = '';
        if (this.modoJuego === 2) {
            mensajeGanador = ganador === 'Jugador1' ? 'Jugador 1 Ganó' : 'Jugador 2 Ganó';
        } else {
            mensajeGanador = ganador === 'EquipoA' ? 'Equipo A Ganó' : 'Equipo B Ganó';
        }

        // Actualizar UI
        document.getElementById('gameStatus').textContent = 'Finalizada';

        // Mostrar alerta de victoria
        this.showVictoryAlert(mensajeGanador);

        // Deshabilitar todos los cubos
        document.querySelectorAll('.cube').forEach(cube => {
            cube.classList.add('disabled');
        });
    }

    // Mostrar alerta de victoria
    showVictoryAlert(mensaje) {
        const alertHtml = `
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <h4 class="alert-heading"><i class="fas fa-trophy"></i> ¡Partida Finalizada!</h4>
                <p class="mb-0"><strong>${mensaje}</strong></p>
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        const container = document.querySelector('.game-container');
        if (container) {
            container.insertAdjacentHTML('afterbegin', alertHtml);
        }
    }

    // Mostrar mensaje
    showMessage(message, type = 'info') {
        const messageBox = document.getElementById('gameMessage');
        if (messageBox) {
            messageBox.className = `alert alert-${type} small mt-3`;
            messageBox.innerHTML = `<strong>${message}</strong>`;
        }
    }

    // Cargar estado del juego
    async loadGameState() {
        console.log('Estado del juego cargado');
    }
}

// Inicializar el juego cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function () {
    const partidaIdElement = document.getElementById('partidaId');
    const modoJuegoElement = document.getElementById('modoJuego');

    if (partidaIdElement && modoJuegoElement) {
        const partidaId = parseInt(partidaIdElement.value);
        const modoJuego = parseInt(modoJuegoElement.value);

        // Verificar que los valores son válidos
        if (isNaN(partidaId) || isNaN(modoJuego)) {
            console.error('Error: valores de partida o modo inválidos');
            return;
        }

        try {
            window.quixoGame = new QuixoGame(partidaId, modoJuego);
        } catch (error) {
            console.error('Error al inicializar el juego:', error);
        }
    }
});