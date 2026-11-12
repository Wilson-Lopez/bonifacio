// =====================================================
//  CRUD de Instructores (consume /api/Instructores)
// =====================================================

let instructores = [];      // lista cargada desde la API
let idAEliminar = null;     // id seleccionado para eliminar

// Elementos de la página
const tablaBody = document.getElementById('tablaBody');
const contador = document.getElementById('contador');
const txtBuscar = document.getElementById('txtBuscar');
const filtroActivo = document.getElementById('filtroActivo');

const form = document.getElementById('formInstructor');
const tituloModal = document.getElementById('tituloModal');
const alertaError = document.getElementById('alertaError');
const btnGuardar = document.getElementById('btnGuardar');
const txtId = document.getElementById('txtId');
const txtNombres = document.getElementById('txtNombres');
const txtApellidos = document.getElementById('txtApellidos');
const txtEspecialidad = document.getElementById('txtEspecialidad');
const txtEmail = document.getElementById('txtEmail');
const chkActivo = document.getElementById('chkActivo');

const modalForm = new bootstrap.Modal('#modalForm');
const modalEliminar = new bootstrap.Modal('#modalEliminar');

const COLUMNAS = 7;

// ---------- LISTAR (GET) ----------
async function cargarInstructores() {
    const filtro = filtroActivo.value;
    const url = filtro ? `${API.instructores}?activo=${filtro}` : API.instructores;

    tablaBody.innerHTML = filaMensaje(COLUMNAS, '<div class="spinner-border spinner-border-sm me-2"></div>Cargando...');

    try {
        instructores = await apiRequest(url);
        mostrarTabla();
    } catch (error) {
        tablaBody.innerHTML = filaMensaje(COLUMNAS, `<i class="bi bi-wifi-off me-2"></i>${escaparHtml(error.message)}`);
        contador.textContent = '';
        mostrarToast(error.message, 'danger');
    }
}

function mostrarTabla() {
    const texto = txtBuscar.value.trim().toLowerCase();

    const lista = instructores.filter(i =>
        `${i.nombres} ${i.apellidos} ${i.especialidad} ${i.email}`.toLowerCase().includes(texto));

    contador.textContent = `${lista.length} de ${instructores.length} registro(s)`;

    if (lista.length === 0) {
        tablaBody.innerHTML = filaMensaje(COLUMNAS, '<i class="bi bi-inbox me-2"></i>No hay instructores para mostrar.');
        return;
    }

    tablaBody.innerHTML = lista.map(i => `
        <tr>
            <td class="fw-semibold">${i.idInstructor}</td>
            <td>${escaparHtml(i.nombres)}</td>
            <td>${escaparHtml(i.apellidos)}</td>
            <td>${escaparHtml(i.especialidad)}</td>
            <td>${escaparHtml(i.email)}</td>
            <td>${badgeEstado(i.activo)}</td>
            <td class="acciones text-end">
                <button class="btn btn-sm btn-outline-primary" data-accion="editar" data-id="${i.idInstructor}" title="Editar">
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" data-accion="eliminar" data-id="${i.idInstructor}" title="Eliminar">
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
    chkActivo.checked = true;
    tituloModal.textContent = 'Nuevo instructor';
    modalForm.show();
}

// ---------- OBTENER POR ID (GET /{id}) ----------
async function abrirEditar(id) {
    try {
        const i = await apiRequest(`${API.instructores}/${id}`);

        form.classList.remove('was-validated');
        alertaError.classList.add('d-none');
        txtId.value = i.idInstructor;
        txtNombres.value = i.nombres;
        txtApellidos.value = i.apellidos;
        txtEspecialidad.value = i.especialidad;
        txtEmail.value = i.email;
        chkActivo.checked = i.activo;
        tituloModal.textContent = `Editar instructor #${i.idInstructor}`;
        modalForm.show();
    } catch (error) {
        mostrarToast(error.message, 'danger');
        cargarInstructores();
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
        especialidad: txtEspecialidad.value.trim(),
        email: txtEmail.value.trim(),
        activo: chkActivo.checked
    };

    const id = txtId.value;
    btnGuardar.disabled = true;
    alertaError.classList.add('d-none');

    try {
        if (id) {
            await apiRequest(`${API.instructores}/${id}`, 'PUT', datos);
            mostrarToast('Instructor actualizado correctamente.');
        } else {
            const nuevo = await apiRequest(API.instructores, 'POST', datos);
            mostrarToast(`Instructor registrado con Id ${nuevo.idInstructor}.`);
        }
        modalForm.hide();
        await cargarInstructores();
    } catch (error) {
        alertaError.textContent = error.message;
        alertaError.classList.remove('d-none');
    } finally {
        btnGuardar.disabled = false;
    }
});

// ---------- ELIMINAR (DELETE) ----------
function confirmarEliminar(id) {
    const i = instructores.find(x => x.idInstructor === id);
    idAEliminar = id;
    document.getElementById('textoEliminar').textContent =
        i ? `${i.nombres} ${i.apellidos}` : `Instructor #${id}`;
    modalEliminar.show();
}

document.getElementById('btnConfirmarEliminar').addEventListener('click', async () => {
    try {
        await apiRequest(`${API.instructores}/${idAEliminar}`, 'DELETE');
        mostrarToast('Instructor eliminado correctamente.');
    } catch (error) {
        mostrarToast(error.message, 'danger');
    } finally {
        modalEliminar.hide();
        idAEliminar = null;
        await cargarInstructores();
    }
});

// ---------- EVENTOS ----------
document.getElementById('btnNuevo').addEventListener('click', abrirNuevo);
txtBuscar.addEventListener('input', mostrarTabla);
filtroActivo.addEventListener('change', cargarInstructores);

// Botones Editar / Eliminar de cada fila
tablaBody.addEventListener('click', (e) => {
    const boton = e.target.closest('button[data-accion]');
    if (!boton) return;

    const id = Number(boton.dataset.id);
    if (boton.dataset.accion === 'editar') abrirEditar(id);
    if (boton.dataset.accion === 'eliminar') confirmarEliminar(id);
});

// Carga inicial
cargarInstructores();
