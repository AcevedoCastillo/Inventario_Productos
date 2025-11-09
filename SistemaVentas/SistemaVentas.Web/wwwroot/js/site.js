// Funciones globales del sitio

// Formatear moneda
function formatearMoneda(valor) {
    return new Intl.NumberFormat('es-SV', {
        style: 'currency',
        currency: 'USD'
    }).format(valor);
}

// Formatear fecha
function formatearFecha(fecha) {
    return new Date(fecha).toLocaleDateString('es-SV', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
}

// Confirmar eliminación
function confirmarEliminacion(mensaje) {
    return confirm(mensaje || '¿Está seguro de que desea eliminar este registro?');
}

// Auto-cerrar alertas después de 5 segundos
$(document).ready(function () {
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);
});

// Prevenir envío de formularios con Enter (excepto en ventas)
$(document).on('keypress', 'form:not(#formVenta)', function (e) {
    if (e.which === 13 && e.target.type !== 'textarea') {
        e.preventDefault();
        return false;
    }
});

// Validación de números decimales
$('input[type="number"]').on('keypress', function (e) {
    const charCode = e.which ? e.which : e.keyCode;

    // Permitir: backspace, delete, tab, escape, enter
    if (charCode === 46 || charCode === 8 || charCode === 9 || charCode === 27 || charCode === 13 ||
        // Permitir: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
        (charCode === 65 && e.ctrlKey === true) ||
        (charCode === 67 && e.ctrlKey === true) ||
        (charCode === 86 && e.ctrlKey === true) ||
        (charCode === 88 && e.ctrlKey === true)) {
        return;
    }

    // Asegurar que es un número o punto decimal
    if ((charCode < 48 || charCode > 57) && charCode !== 46) {
        e.preventDefault();
    }

    // Solo permitir un punto decimal
    if (charCode === 46 && $(this).val().indexOf('.') !== -1) {
        e.preventDefault();
    }
});

// Scroll suave
$('a[href^="#"]').on('click', function (e) {
    e.preventDefault();

    const target = $(this.hash);
    if (target.length) {
        $('html, body').animate({
            scrollTop: target.offset().top - 70
        }, 600);
    }
});

// Loading overlay
function mostrarLoading() {
    $('body').append(`
        <div id="loadingOverlay" style="
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        ">
            <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Cargando...</span>
            </div>
        </div>
    `);
}

function ocultarLoading() {
    $('#loadingOverlay').remove();
}