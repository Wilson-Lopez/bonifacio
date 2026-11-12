// =====================================================
//  CRUD de Estudiantes (consume /api/Estudiantes)
// =====================================================

let estudiantes = [];       // lista cargada desde la API
let idAEliminar = null;     // id seleccionado para eliminar

// Elementos de la página
const tablaBody = document.getElementById('tablaBody');
const contador = document.getElementById('contador');
const txtBuscar = document.getElementById('txtBuscar');
const filtroActivo = document.getElementById('filtroActivo');

const form = document.getElementById('formEstudiante');
const tituloModal = document.getElementById('tituloModal');
const alertaError = document.getElementById('alertaError');
const btnGuardar = document.getElementById('btnGuardar');
const txtId = document.getElementById('txtId');
const txtNombres = document.getElementById('txtNombres');
const txtApellidos = document.getElementById('txtApellidos');
const txtEmail = document.getElementById('txtEmail');
const txtTelefono = document.getElementById('txtTelefono');
const txtFechaRegistro = document.getElementById('txtFechaRegistro');
const chkActivo = document.getElementById('chkActivo');

const modalForm = new bootstrap.Modal('#modalForm');
const modalEliminar = new bootstrap.Modal('#modalEliminar');

const COLUMNAS = 8;

// ---------- LISTAR (GET) ----------
async function cargarEstudiantes() {
    const filtro = filtroActivo.value;
    const url = filtro ? `${API.estudiantes}?activo=${filtro}` : API.estudiantes;

    tablaBody.innerHTML = filaMensaje(COLUMNAS, '<div class="spinner-border spinner-border-sm me-2"></div>Cargando...');

    try {
        estudiantes = await apiRequest(url);
        mostrarTabla();
    } catch (error) {
        tablaBody.innerHTML = filaMensaje(COLUMNAS, `<i class="bi bi-wifi-off me-2"></i>${escaparHtml(error.message)}`);
        contador.textContent = '';
        mostrarToast(error.message, 'danger');
    }
}

function mostrarTabla() {
    const texto = txtBuscar.value.trim().toLowerCase();

    const lista = estudiantes.filter(e =>
        `${e.nombres} ${e.apellidos} ${e.email} ${e.telefono ?? ''}`.toLowerCase().includes(texto));

    contador.textContent = `${lista.length} de ${estudiantes.length} registro(s)`;

    if (lista.length === 0) {
        tablaBody.innerHTML = filaMensaje(COLUMNAS, '<i class="bi bi-inbox me-2"></i>No hay estudiantes para mostrar.');
        return;
    }

    tablaBody.innerHTML = lista.map(e => `
        <tr>
            <td class="fw-semibold">${e.idEstudiante}</td>
            <td>${escaparHtml(e.nombres)}</td>
            <td>${escaparHtml(e.apellidos)}</td>
            <td>${escaparHtml(e.email)}</td>
            <td>${e.telefono ? escaparHtml(e.telefono) : '<span class="text-muted">—</span>'}</td>
            <td class="text-nowrap">${formatearFecha(e.fechaRegistro)}</td>
            <td>${badgeEstado(e.activo)}</td>
            <td class="acciones text-end">
                <button class="btn btn-sm btn-outline-primary" data-accion="editar" data-id="${e.idEstudiante}" title="Editar">
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" data-accion="eliminar" data-id="${e.idEstudiante}" title="Eliminar">
                    <i class="bi bi-trash3"></i>
                </button>
            </td>
        </tr>`).join('');
}

// ---------- ABRIR FORMULARIO ----------
function abrirNuevo() {
    form.reset();
    form.classList.remove('was-validated');
    alertaError.classList.add('d-none');
    txtId.value = '';
    txtFechaRegistro.value = '';
    chkActivo.checked = true;
    tituloModal.textContent = 'Nuevo estudiante';
    modalForm.show();
}

// ---------- OBTENER POR ID (GET /{id}) ----------
async function abrirEditar(id) {
    try {
        const e = await apiRequest(`${API.estudiantes}/${id}`);

        form.classList.remove('was-validated');
        alertaError.classList.add('d-none');
        txtId.value = e.idEstudiante;
        txtNombres.value = e.nombres;
        txtApellidos.value = e.apellidos;
        txtEmail.value = e.email;
        txtTelefono.value = e.telefono ?? '';
        txtFechaRegistro.value = formatearFecha(e.fechaRegistro);
        chkActivo.checked = e.activo;
        tituloModal.textContent = `Editar estudiante #${e.idEstudiante}`;
        modalForm.show();
    } catch (error) {
        mostrarToast(error.message, 'danger');
        cargarEstudiantes();
    }
}

// ---------- GUARDAR: CREAR (POST) o ACTUALIZAR (PUT) ----------
form.addEventListener('submit', async (e) => {
    e.preventDefault();

    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return;
    }

    const datos = {
        nombres: txtNombres.value.trim(),
        apellidos: txtApellidos.value.trim(),
        email: txtEmail.value.trim(),
        telefono: txtTelefono.value.trim() || null,
        activo: chkActivo.checked
    };

    const id = txtId.value;
    btnGuardar.disabled = true;
    alertaError.classList.add('d-none');

    try {
        if (id) {
            await apiRequest(`${API.estudiantes}/${id}`, 'PUT', datos);
            mostrarToast('Estudiante actualizado correctamente.');
        } else {
            const nuevo = await apiRequest(API.estudiantes, 'POST', datos);
            mostrarToast(`Estudiante registrado con Id ${nuevo.idEstudiante}.`);
        }
        modalForm.hide();
        await cargarEstudiantes();
    } catch (error) {
        alertaError.textContent = error.message;
        alertaError.classList.remove('d-none');
    } finally {
        btnGuardar.disabled = false;
    }
});

// ---------- ELIMINAR (DELETE) ----------
function confirmarEliminar(id) {
    const e = estudiantes.find(x => x.idEstudiante === id);
    idAEliminar = id;
    document.getElementById('textoEliminar').textContent =
        e ? `${e.nombres} ${e.apellidos}` : `Estudiante #${id}`;
    modalEliminar.show();
}

document.getElementById('btnConfirmarEliminar').addEventListener('click', async () => {
    try {
        await apiRequest(`${API.estudiantes}/${idAEliminar}`, 'DELETE');
        mostrarToast('Estudiante eliminado correctamente.');
    } catch (error) {
        mostrarToast(error.message, 'danger');
    } finally {
        modalEliminar.hide();
        idAEliminar = null;
        await cargarEstudiantes();
    }
});

// ---------- EVENTOS ----------
document.getElementById('btnNuevo').addEventListener('click', abrirNuevo);
txtBuscar.addEventListener('input', mostrarTabla);
filtroActivo.addEventListener('change', cargarEstudiantes);

// Botones Editar / Eliminar de cada fila
tablaBody.addEventListener('click', (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    if (boton.dataset.accion === 'editar') abrirEditar(id);
    if (boton.dataset.accion === 'eliminar') confirmarEliminar(id);
});

// Carga inicial
cargarEstudiantes();
