$(document).ready(function () {
    inicializarEventosReportes();
    establecerFechasDefault();
});

function inicializarEventosReportes() {
    $('#btnGenerarReporte').on('click', function () {
        generarReporte();
    });

    $('#btnDescargarPDF').on('click', function () {
        descargarReporte('pdf');
    });

    $('#btnDescargarExcel').on('click', function () {
        descargarReporte('excel');
    });

    $('#btnLimpiar').on('click', function () {
        limpiarFiltros();
    });
}

function establecerFechasDefault() {
    const hoy = new Date();
    const primerDiaMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1);

    $('#reporteFechaInicio').val(formatearFecha(primerDiaMes));
    $('#reporteFechaFin').val(formatearFecha(hoy));
}

function formatearFecha(fecha) {
    const year = fecha.getFullYear();
    const month = String(fecha.getMonth() + 1).padStart(2, '0');
    const day = String(fecha.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

function generarReporte() {
    const fechaInicio = $('#reporteFechaInicio').val();
    const fechaFin = $('#reporteFechaFin').val();

    if (!fechaInicio || !fechaFin) {
        mostrarAlerta('Debe seleccionar ambas fechas', 'warning');
        return;
    }

    if (new Date(fechaInicio) > new Date(fechaFin)) {
        mostrarAlerta('La fecha de inicio no puede ser mayor a la fecha fin', 'warning');
        return;
    }

    // Mostrar loading
    const btn = $('#btnGenerarReporte');
    const textoOriginal = btn.html();
    btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Cargando...');

    $.ajax({
        url: '/Ventas/ObtenerReporte',
        type: 'GET',
        data: {
            fechaInicio: fechaInicio,
            fechaFin: fechaFin
        },
        success: function (response) {
            if (response.success) {
                renderizarReporte(response.data);
                $('#resultadosReporte').slideDown();
            } else {
                mostrarAlerta('Error al generar reporte: ' + response.message, 'error');
            }
        },
        error: function () {
            mostrarAlerta('Error al conectar con el servidor', 'error');
        },
        complete: function () {
            btn.prop('disabled', false).html(textoOriginal);
        }
    });
}

function renderizarReporte(ventas) {
    const tbody = $('#reporteBody');
    tbody.empty();

    if (!ventas || ventas.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="6" class="text-center text-muted">
                    <i class="fas fa-inbox fa-2x mb-2"></i>
                    <p>No hay ventas en el período seleccionado</p>
                </td>
            </tr>
        `);
        limpiarTotales();
        return;
    }

    let totalSubtotal = 0;
    let totalIVA = 0;
    let totalGeneral = 0;

    ventas.forEach(venta => {
        totalSubtotal += venta.subTotal;
        totalIVA += venta.totalIVA;
        totalGeneral += venta.total;

        const fila = `
            <tr>
                <td>${venta.idVenta}</td>
                <td>${new Date(venta.fecha).toLocaleDateString('es-SV')}</td>
                <td>${venta.nombreUsuario}</td>
                <td class="text-end">$${venta.subTotal.toFixed(2)}</td>
                <td class="text-end">$${venta.totalIVA.toFixed(2)}</td>
                <td class="text-end fw-bold">$${venta.total.toFixed(2)}</td>
            </tr>
        `;
        tbody.append(fila);
    });

    // Actualizar totales
    $('#totalSubtotal').text('$' + totalSubtotal.toFixed(2));
    $('#totalIVA').text('$' + totalIVA.toFixed(2));
    $('#totalGeneral').text('$' + totalGeneral.toFixed(2));
}

function limpiarTotales() {
    $('#totalSubtotal').text('$0.00');
    $('#totalIVA').text('$0.00');
    $('#totalGeneral').text('$0.00');
}

function descargarReporte(tipo) {
    const fechaInicio = $('#reporteFechaInicio').val();
    const fechaFin = $('#reporteFechaFin').val();

    if (!fechaInicio || !fechaFin) {
        mostrarAlerta('Debe seleccionar ambas fechas', 'warning');
        return;
    }

    const url = tipo === 'pdf'
        ? `/Ventas/DescargarPDF?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`
        : `/Ventas/DescargarExcel?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    // Abrir en nueva ventana para descargar
    window.open(url, '_blank');
}

function limpiarFiltros() {
    establecerFechasDefault();
    $('#resultadosReporte').slideUp();
    $('#reporteBody').empty();
    limpiarTotales();
}

function mostrarAlerta(mensaje, tipo) {
    let icono = 'fa-info-circle';
    let clase = 'alert-info';

    switch (tipo) {
        case 'success':
            icono = 'fa-check-circle';
            clase = 'alert-success';
            break;
        case 'error':
            icono = 'fa-exclamation-circle';
            clase = 'alert-danger';
            break;
        case 'warning':
            icono = 'fa-exclamation-triangle';
            clase = 'alert-warning';
            break;
    }

    const alerta = `
        <div class="alert ${clase} alert-dismissible fade show" role="alert">
            <i class="fas ${icono}"></i> ${mensaje}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    $('.container-fluid').prepend(alerta);

    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);

    $('html, body').animate({ scrollTop: 0 }, 'fast');
}