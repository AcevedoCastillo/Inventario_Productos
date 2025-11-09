using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Core.DTOs;
using SistemaVentas.Core.Interfaces;

namespace SistemaVentas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Obtener todos los productos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productos = await _productoService.ObtenerTodosAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Productos obtenidos exitosamente",
                    Data = productos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al obtener productos",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);

                return Ok(new
                {
                    Success = true,
                    Message = "Producto obtenido exitosamente",
                    Data = producto
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al obtener producto",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Buscar producto por código
        /// </summary>
        [HttpGet("buscar/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            try
            {
                var producto = await _productoService.ObtenerPorCodigoAsync(codigo);

                return Ok(new
                {
                    Success = true,
                    Message = "Producto encontrado",
                    Data = producto
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al buscar producto",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crear nuevo producto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoDTO productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var producto = await _productoService.CrearAsync(productoDto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = producto.IdPro },
                    new
                    {
                        Success = true,
                        Message = "Producto creado exitosamente",
                        Data = producto
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualizar producto existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoDTO productoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Datos inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var resultado = await _productoService.ActualizarAsync(id, productoDto);

                if (!resultado)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Producto actualizado exitosamente"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Eliminar producto (lógico)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _productoService.EliminarAsync(id);

                if (!resultado)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Producto eliminado exitosamente"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Verificar stock disponible
        /// </summary>
        [HttpGet("verificar-stock/{idPro}/{cantidad}")]
        public async Task<IActionResult> VerificarStock(int idPro, int cantidad)
        {
            try
            {
                var disponible = await _productoService.VerificarStockAsync(idPro, cantidad);

                return Ok(new
                {
                    Success = true,
                    Disponible = disponible,
                    Message = disponible ? "Stock disponible" : "Stock insuficiente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error al verificar stock",
                    Error = ex.Message
                });
            }
        }
    }
}