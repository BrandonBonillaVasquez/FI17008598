// ========================================
// SINCRONIZACIÓN DE TIEMPO CON SERVIDOR
// ========================================

class GameTimer {
    constructor(partidaId) {
        this.partidaId = partidaId;
        this.syncInterval = null;
        this.syncIntervalTime = 30000; // 30 segundos
    }

    // Iniciar sincronización automática
    startSync() {
        this.syncInterval = setInterval(() => {
            this.syncTimeWithServer();
        }, this.syncIntervalTime);
    }

    // Detener sincronización
    stopSync() {
        if (this.syncInterval) {
            clearInterval(this.syncInterval);
            this.syncInterval = null;
        }
    }

    // Sincronizar tiempo con el servidor
    async syncTimeWithServer() {
        if (!gameClock) return;

        const tiempoTranscurrido = formatTime(gameClock.getElapsedTime());

        try {
            const response = await fetch('/Game/UpdateTime', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    partidaId: this.partidaId,
                    tiempoTranscurrido: tiempoTranscurrido
                })
            });

            const result = await response.json();

            if (!result.success) {
                console.error('Error al sincronizar tiempo con servidor');
            }
        } catch (error) {
            console.error('Error en sincronización de tiempo:', error);
        }
    }

    // Guardar tiempo final
    async saveFinalTime() {
        this.stopSync();
        await this.syncTimeWithServer();
    }
}

// Instancia global del timer
let gameTimer = null;

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function () {
    const partidaIdElement = document.getElementById('partidaId');
    const estadoPartida = document.getElementById('estadoPartida');

    if (partidaIdElement && estadoPartida && estadoPartida.value === 'EnCurso') {
        const partidaId = parseInt(partidaIdElement.value);
        gameTimer = new GameTimer(partidaId);
        gameTimer.startSync();
    }
});

// Guardar tiempo al cerrar/salir de la página
window.addEventListener('beforeunload', function () {
    if (gameTimer) {
        gameTimer.saveFinalTime();
    }
});