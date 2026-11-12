// =====================================================
//  Funciones compartidas para consumir la API
// =====================================================

const API = {
    instructores: '/api/Instructores',
    estudiantes: '/api/Estudiantes'
};

/**
 * Hace una petición a la API y devuelve el JSON de respuesta.
 * Si la API responde con error, lanza un Error con el mensaje del servidor.
 */
async function apiRequest(url, method = 'GET', data = null) {
    const options = { method, headers: {} };

    if (data !== null) {
        options.headers['Content-Type'] = 'application/json';
        options.body = JSON.stringify(data);
    }

    let response;
    try {
        response = await fetch(url, options);
    } catch {
        throw new Error('No se pudo conectar con la API. Verifique que esté en ejecución.');
    }

    // 204 No Content (DELETE) no trae cuerpo
    const text = await response.text();
    let body = null;
    if (text) {
        try { body = JSON.parse(text); } catch { body = null; }
    }

    if (!response.ok) {
        throw new Error(obtenerMensajeError(response.status, body));
    }

    return body;
}

/** Convierte la respuesta de error de la API en un texto legible. */
function obtenerMensajeError(status, body) {
    // Errores propios del controlador: { mensaje: "..." }
    if (body && body.mensaje) return body.mensaje;

    // Errores de validación de ASP.NET: { errors: { Campo: ["..."] } }
    if (body && body.errors) return Object.values(body.errors).flat().join(' ');

    if (status === 404) return 'El registro no existe.';
    return `Ocurrió un error inesperado (HTTP ${status}).`;
}

/** Muestra una notificación en la esquina inferior derecha. */
function mostrarToast(mensaje, tipo = 'success') {
    const iconos = { success: 'check-circle-fill', danger: 'x-circle-fill', warning: 'exclamation-triangle-fill' };
    const contenedor = document.getElementById('toastContainer');

    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-bg-${tipo} border-0`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <i class="bi bi-${iconos[tipo] || 'info-circle-fill'} me-2"></i>${escaparHtml(mensaje)}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>`;

    contenedor.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast, { delay: 4000 });
    toast.addEventListener('hidden.bs.toast', () => toast.remove());
    bsToast.show();
}

/** Evita que el texto ingresado por el usuario se interprete como HTML. */
function escaparHtml(valor) {
    return String(valor ?? '')
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');
}

/** Insignia verde/gris según el estado Activo. */
function badgeEstado(activo) {
    return activo
        ? '<span class="badge rounded-pill text-bg-success">Activo</span>'
        : '<span class="badge rounded-pill text-bg-secondary">Inactivo</span>';
}

/** Formatea una fecha ISO como dd/mm/aaaa hh:mm. */
function formatearFecha(iso) {
    if (!iso) return '';
    const fecha = new Date(iso);
    return fecha.toLocaleString('es-PE', {
        day: '2-digit', month: '2-digit', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });
}

/** Muestra un mensaje dentro de la tabla (cargando, vacío o error). */
function filaMensaje(columnas, html) {
    return `<tr><td colspan="${columnas}" class="text-center text-muted py-5">${html}</td></tr>`;
}
