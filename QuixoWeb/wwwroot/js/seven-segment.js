// ========================================
// RELOJ DE 7 SEGMENTOS CON GUARDADO
// ========================================

class SevenSegmentClock {
    constructor(partidaId = null) {
        this.partidaId = partidaId;
        this.startTime = null;      // timestamp (ms) cuando arranca
        this.elapsedTime = 0;       // ms acumulados
        this.timerInterval = null;  // setInterval del tick visual
        this.saveInterval = null;   // setInterval del guardado periódico
        this.isRunning = false;
    }

    // Iniciar el reloj
    start(initialTimeMs = 0) {
        if (this.isRunning) return;

        // Acepta ms iniciales (0 si nuevo)
        this.elapsedTime = Number(initialTimeMs) || 0;
        this.startTime = Date.now() - this.elapsedTime;
        this.isRunning = true;

        // Primer pintado inmediato
        this.updateDisplay();

        // Actualizar cada segundo
        this.timerInterval = setInterval(() => {
            this.elapsedTime = Date.now() - this.startTime;
            this.updateDisplay();
        }, 1000);

        // Guardar en BD cada 10 segundos (ajusta si quieres)
        if (this.partidaId) {
            this.saveInterval = setInterval(() => {
                this.saveToServer();
            }, 10000);
        }
    }

    // Pausar el reloj
    pause() {
        if (!this.isRunning) return;

        clearInterval(this.timerInterval);
        this.timerInterval = null;

        if (this.saveInterval) {
            clearInterval(this.saveInterval);
            this.saveInterval = null;
        }

        this.isRunning = false;

        // Guardar tiempo final
        if (this.partidaId) {
            // No await en unload, pero aquí sí puede ser await si llamas manualmente
            this.saveToServer();
        }
    }

    // Detener y reiniciar el reloj
    reset() {
        this.pause();
        this.elapsedTime = 0;
        this.updateDisplay();
    }

    // (Opcional) reanudar sin perder tiempo acumulado
    resume() {
        if (this.isRunning) return;
        this.start(this.elapsedTime);
    }

    // Obtener tiempo transcurrido en milisegundos
    getElapsedTime() {
        return this.elapsedTime;
    }

    // Obtener tiempo formateado HH:MM:SS
    getFormattedTime() {
        const totalSeconds = Math.floor(this.elapsedTime / 1000);
        const hours = Math.floor(totalSeconds / 3600);
        const minutes = Math.floor((totalSeconds % 3600) / 60);
        const seconds = totalSeconds % 60;

        return {
            hours: hours.toString().padStart(2, '0'),
            minutes: minutes.toString().padStart(2, '0'),
            seconds: seconds.toString().padStart(2, '0')
        };
    }

    // Obtener tiempo como string HH:MM:SS
    getTimeString() {
        const time = this.getFormattedTime();
        return `${time.hours}:${time.minutes}:${time.seconds}`;
    }

    // Actualizar el display visual
    updateDisplay() {
        const time = this.getFormattedTime();
        this.setDigit('hour1', time.hours[0]);
        this.setDigit('hour2', time.hours[1]);
        this.setDigit('min1',  time.minutes[0]);
        this.setDigit('min2',  time.minutes[1]);
        this.setDigit('sec1',  time.seconds[0]);
        this.setDigit('sec2',  time.seconds[1]);
    }

    // Establecer un dígito individual
    setDigit(elementId, value) {
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = value;
        }
    }

    // Establecer tiempo desde formato HH:MM:SS
    setTimeFromString(timeString) {
        if (!timeString) return;

        const parts = timeString.split(':');
        if (parts.length === 3) {
            const hours   = parseInt(parts[0], 10) || 0;
            const minutes = parseInt(parts[1], 10) || 0;
            const seconds = parseInt(parts[2], 10) || 0;

            this.elapsedTime = (hours * 3600 + minutes * 60 + seconds) * 1000;
            this.updateDisplay();
        }
    }

    // Guardar tiempo en el servidor
    async saveToServer() {
        if (!this.partidaId) return;

        try {
            const timeString = this.getTimeString();

            const response = await fetch('/Game/UpdateTime', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    partidaId: this.partidaId,
                    tiempoTranscurrido: timeString
                })
            });

            const result = await response.json();

            if (result?.success) {
                // opcional: console.debug(`Tiempo guardado: ${timeString}`);
            } else {
                console.error('Error al guardar tiempo');
            }
        } catch (error) {
            console.error('❌ Error en saveToServer:', error);
        }
    }
}

// Instancia global del reloj
let gameClock = null;

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function () {
    // Verificar si estamos en una página con reloj
    const timeDisplay = document.getElementById('timeDisplay'); // contenedor/sentinela
    if (!timeDisplay) return;

    const partidaIdElement = document.getElementById('partidaId');
    const estadoPartida    = document.getElementById('estadoPartida');

    // Lectura de tiempo inicial desde data-attribute (opcional)
    const initialTimeString = timeDisplay.dataset.initial || ""; // "HH:MM:SS" si lo envías

    if (partidaIdElement) {
        const partidaId = parseInt(partidaIdElement.value, 10);
        gameClock = new SevenSegmentClock(partidaId);

        // Si viene tiempo inicial del servidor, cargarlo
        if (initialTimeString) {
            gameClock.setTimeFromString(initialTimeString);
        }

        // Iniciar el reloj automáticamente si la partida está en curso
        if (estadoPartida && estadoPartida.value === 'EnCurso') {
            // Si ya fijaste from string, pásale ms acumulados para continuar
            gameClock.start(gameClock.getElapsedTime());
        }
    } else {
        gameClock = new SevenSegmentClock();
        if (estadoPartida && estadoPartida.value === 'EnCurso') {
            gameClock.start();
        }
    }
});

// Guardar tiempo antes de salir de la página
window.addEventListener('beforeunload', function () {
    if (gameClock && gameClock.isRunning) {
        gameClock.pause(); // esto hace saveToServer()
    }
});

// Exportar para uso global (tests / módulos)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = SevenSegmentClock;
}