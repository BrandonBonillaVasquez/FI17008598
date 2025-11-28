// ========================================
// QUIXO - NAVEGACIÓN DE HISTORIAL
// ========================================

class HistoryViewer {
    constructor(partidaId, totalJugadas, modoJuego) {
        this.partidaId = partidaId;
        this.totalJugadas = totalJugadas;
        this.modoJuego = modoJuego;
        this.currentMove = 0;

        this.init();
    }

    // Inicializar
    init() {
        this.attachEventListeners();
        this.loadMove(0); // Cargar estado inicial
    }

    // Adjuntar event listeners
    attachEventListeners() {
        document.getElementById('btnFirst')?.addEventListener('click', () => this.goToFirst());
        document.getElementById('btnPrevious')?.addEventListener('click', () => this.goToPrevious());
        document.getElementById('btnNext')?.addEventListener('click', () => this.goToNext());
        document.getElementById('btnLast')?.addEventListener('click', () => this.goToLast());
    }

    // Ir a la primera jugada
    goToFirst() {
        this.loadMove(0);
    }

    // Ir a la jugada anterior
    goToPrevious() {
        if (this.currentMove > 0) {
            this.loadMove(this.currentMove - 1);
        }
    }

    // Ir a la siguiente jugada
    goToNext() {
        if (this.currentMove < this.totalJugadas - 1) {
            this.loadMove(this.currentMove + 1);
        }
    }

    // Ir a la última jugada
    goToLast() {
        this.loadMove(this.totalJugadas - 1);
    }

    // Cargar una jugada específica
    async loadMove(moveNumber) {
        try {
            const response = await fetch(`/Game/GetHistoryMove?partidaId=${this.partidaId}&numeroJugada=${moveNumber}`);
            const result = await response.json();

            if (result.success) {
                this.currentMove = moveNumber;
                this.updateBoard(result.jugada);
                this.updateMoveInfo(result.jugada);
                this.updateControls();
            } else {
                console.error('Error al cargar jugada:', result.message);
            }
        } catch (error) {
            console.error('Error al cargar jugada:', error);
        }
    }

    // Actualizar el tablero
    updateBoard(jugada) {
        const tablero = jugada.tablero;
        const orientaciones = jugada.orientaciones;

        for (let i = 0; i < 5; i++) {
            for (let j = 0; j < 5; j++) {
                const cube = document.querySelector(`#historyBoard [data-row="${i}"][data-col="${j}"]`);
                if (!cube) continue;

                // Acceder como lista en lugar de array multidimensional
                const symbol = tablero[i][j];
                const orientation = orientaciones ? orientaciones[i][j] : null;

                this.updateCubeSymbol(cube, symbol, orientation);
            }
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
        } else if (symbol === 'X') {
            const svg = this.createCrossSVG(orientation);
            const div = document.createElement('div');
            div.className = 'symbol cross';
            div.innerHTML = svg;
            content.appendChild(div);
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

    // Actualizar información de la jugada
    updateMoveInfo(jugada) {
        document.getElementById('currentMove').textContent = jugada.numeroJugada;
        document.getElementById('movePlayer').textContent = `Jugador ${jugada.jugadorActual}`;
        document.getElementById('moveTime').textContent = jugada.tiempoTranscurrido;
    }

    // Actualizar controles de navegación
    updateControls() {
        const btnFirst = document.getElementById('btnFirst');
        const btnPrevious = document.getElementById('btnPrevious');
        const btnNext = document.getElementById('btnNext');
        const btnLast = document.getElementById('btnLast');

        if (btnFirst) btnFirst.disabled = this.currentMove === 0;
        if (btnPrevious) btnPrevious.disabled = this.currentMove === 0;
        if (btnNext) btnNext.disabled = this.currentMove >= this.totalJugadas - 1;
        if (btnLast) btnLast.disabled = this.currentMove >= this.totalJugadas - 1;
    }
}

// Inicializar el visor de historial
document.addEventListener('DOMContentLoaded', function () {
    const partidaIdElement = document.getElementById('partidaId');
    const totalJugadasElement = document.getElementById('totalJugadas');
    const modoJuegoElement = document.getElementById('modoJuego');

    if (partidaIdElement && totalJugadasElement && modoJuegoElement) {
        const partidaId = parseInt(partidaIdElement.value);
        const totalJugadas = parseInt(totalJugadasElement.value);
        const modoJuego = parseInt(modoJuegoElement.value);

        window.historyViewer = new HistoryViewer(partidaId, totalJugadas, modoJuego);
    }
});