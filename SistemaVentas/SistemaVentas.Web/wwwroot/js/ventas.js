// Variables globales
let detalleVenta = [];
let contadorLinea = 1;
const IVA_PORCENTAJE = 0.13; // 13%

$(document).ready(function () {
    inicializarEventos();
});

function inicializarEventos() {
    // Evento para buscar producto al presionar Enter
    $('#txtCodigo').on('keypress', function (e) {
        if (e.which === 13) { // Enter
            e.preventDefault();
            buscarProducto();
        }
    });

    // Evento para agregar producto
    $('#btnAgregar').on('click', function () {
        agregarProducto();
    });

    // Evento para registrar venta
    $('#btnRegistrarVenta').on('click', function () {
        registrarVenta();
    });

    // Evento para cantidad - Enter para agregar
    $('#txtCantidad').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            agregarProducto();
        }
    });
}

// Buscar producto por código
function buscarProducto() {
    const codigo = $('#txtCodigo').val().trim();

    if (codigo === '') {
        mostrarNotificacion('Por favor ingrese un código', 'warning');
        return;
    }

    // Mostrar loading
    $('#txtProducto').val('Buscando...');

    $.ajax({
        url: `${API_BASE}Productos/buscar/${encodeURIComponent(codigo)}`,
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const producto = response.data;
                $('#txtProducto').val(producto.producto);
                $('#txtPrecio').val(producto.precio.toFixed(2));
                $('#txtCantidad').focus().select();

                // Guardar IdPro en el campo oculto
                $('#txtCodigo').data('idpro', producto.idPro);
                $('#txtCodigo').data('stock', producto.stock);
            } else {
                limpiarCamposProducto();
                mostrarNotificacion('Producto no encontrado', 'error');
            }
        },
        error: function () {
            limpiarCamposProducto();
            mostrarNotificacion('Error al buscar el producto', 'error');
        }
    });

}

// Agregar producto al detalle
function agregarProducto() {
    const codigo = $('#txtCodigo').val().trim();
    const producto = $('#txtProducto').val().trim();
    const precio = parseFloat($('#txtPrecio').val()) || 0;
    const cantidad = parseInt($('#txtCantidad').val()) || 0;
    const idPro = $('#txtCodigo').data('idpro');
    const stock = $('#txtCodigo').data('stock') || 0;

    // Validaciones
    if (codigo === '' || producto === '' || precio === 0) {
        mostrarNotificacion('Debe buscar un producto válido', 'warning');
        return;
    }

    if (cantidad <= 0) {
        mostrarNotificacion('La cantidad debe ser mayor a 0', 'warning');
        $('#txtCantidad').focus();
        return;
    }

    if (cantidad > stock) {
        mostrarNotificacion(`Stock insuficiente. Disponible: ${stock}`, 'error');
        return;
    }

    // Verificar si el producto ya está en el detalle
    const productoExistente = detalleVenta.find(item => item.idPro === idPro);

    if (productoExistente) {
        const nuevaCantidad = productoExistente.cantidad + cantidad;

        if (nuevaCantidad > stock) {
            mostrarNotificacion(`Stock insuficiente. Disponible: ${stock}, en tabla: ${productoExistente.cantidad}`, 'error');
            return;
        }

        productoExistente.cantidad = nuevaCantidad;
        productoExistente.total = calcularTotalItem(productoExistente.precio, nuevaCantidad);
        productoExistente.iva = calcularIVA(productoExistente.precio, nuevaCantidad);
    } else {
        // Agregar nuevo producto
        const item = {
            numero: contadorLinea++,
            idPro: idPro,
            codigo: codigo,
            fecha: new Date().toLocaleDateString('es-SV'),
            producto: producto,
            precio: precio,
            cantidad: cantidad,
            iva: calcularIVA(precio, cantidad),
            total: calcularTotalItem(precio, cantidad)
        };

        detalleVenta.push(item);
    }

    renderizarTabla();
    calcularTotales();
    limpiarCamposProducto();
    $('#txtCodigo').focus();

    // Habilitar botón de registrar
    $('#btnRegistrarVenta').prop('disabled', false);
}

// Calcular IVA de un item
function calcularIVA(precio, cantidad) {
    const subtotal = precio * cantidad;
    return subtotal * IVA_PORCENTAJE;
}

// Calcular total de un item (subtotal + IVA)
function calcularTotalItem(precio, cantidad) {
    const subtotal = precio * cantidad;
    const iva = subtotal * IVA_PORCENTAJE;
    return subtotal + iva;
}

// Renderizar tabla de detalle
function renderizarTabla() {
    const tbody = $('#detalleVentaBody');
    tbody.empty();

    if (detalleVenta.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="8" class="text-center text-muted">
                    <i class="fas fa-inbox fa-2x mb-2"></i>
                    <p>No hay productos agregados</p>
                </td>
            </tr>
        `);
        return;
    }

    detalleVenta.forEach((item, index) => {
        const fila = `
            <tr>
                <td class="text-center">${item.numero}</td>
                <td>${item.codigo}</td>
                <td>${item.fecha}</td>
                <td>${item.producto}</td>
                <td class="text-end">$${item.precio.toFixed(2)}</td>
                <td class="text-center">${item.cantidad}</td>
                <td class="text-end fw-bold">$${item.total.toFixed(2)}</td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-danger" onclick="quitarProducto(${index})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
        tbody.append(fila);
    });
}

// Quitar producto del detalle
function quitarProducto(index) {
    if (confirm('¿Está seguro de quitar este producto?')) {
        detalleVenta.splice(index, 1);
        renderizarTabla();
        calcularTotales();

        if (detalleVenta.length === 0) {
            $('#btnRegistrarVenta').prop('disabled', true);
        }
    }
}

// Calcular totales generales
function calcularTotales() {
    if (detalleVenta.length === 0) {
        $('#lblSubTotal').text('$0.00');
        $('#lblIVA').text('$0.00');
        $('#lblTotal').text('$0.00');
        return;
    }

    let subtotal = 0;
    let totalIva = 0;
    let total = 0;

    detalleVenta.forEach(item => {
        const itemSubtotal = item.precio * item.cantidad;
        subtotal += itemSubtotal;
        totalIva += item.iva;
        total += item.total;
    });

    $('#lblSubTotal').text('$' + subtotal.toFixed(2));
    $('#lblIVA').text('$' + totalIva.toFixed(2));
    $('#lblTotal').text('$' + total.toFixed(2));
}

// Registrar venta
function registrarVenta() {
    if (detalleVenta.length === 0) {
        mostrarNotificacion('Debe agregar al menos un producto', 'warning');
        return;
    }

    // Preparar datos para enviar
    const vendedor = $('.page-title').text().split('Vendedor: ')[1]?.replace(')', '') || 'Vendedor';

    const ventaData = {
        vendedor: vendedor,
        detalles: detalleVenta.map(item => ({
            idPro: item.idPro,
            cantidad: item.cantidad,
            precio: item.precio,
            iva: item.iva,
            total: item.total
        }))
    };

    // Deshabilitar botón y mostrar loading
    const btnRegistrar = $('#btnRegistrarVenta');
    const textoOriginal = btnRegistrar.html();
    btnRegistrar.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Procesando...');

    $.ajax({
        url: '/Ventas/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(ventaData),
        success: function (response) {
            if (response.success) {
                $('#numeroVenta').text(response.idVenta);
                $('#modalExito').modal('show');

                // Limpiar todo
                detalleVenta = [];
                contadorLinea = 1;
                renderizarTabla();
                calcularTotales();
                limpiarCamposProducto();
            } else {
                mostrarNotificacion('Error: ' + response.message, 'error');
                btnRegistrar.prop('disabled', false).html(textoOriginal);
            }
        },
        error: function (xhr) {
            let mensaje = 'Error al registrar la venta';

            if (xhr.responseJSON && xhr.responseJSON.message) {
                mensaje = xhr.responseJSON.message;
            }

            mostrarNotificacion(mensaje, 'error');
            btnRegistrar.prop('disabled', false).html(textoOriginal);
        }
    });
}

// Limpiar campos de producto
function limpiarCamposProducto() {
    $('#txtCodigo').val('').removeData();
    $('#txtProducto').val('');
    $('#txtPrecio').val('');
    $('#txtCantidad').val('1');
}

// Mostrar notificación
function mostrarNotificacion(mensaje, tipo) {
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

    // Insertar al inicio del main
    $('main').prepend(alerta);

    // Auto cerrar después de 5 segundos
    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);

    // Scroll al inicio
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}