using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Repositorios;

namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{

        private readonly ProductosRepository _productosRepository;

        public ProductosController()
        {
            _productosRepository = new ProductosRepository();
        }

        // POST api/Producto
        [HttpPost]
        public IActionResult CrearProducto([FromBody] Productos producto)
        {
            if (producto == null)
            {
                return BadRequest("Producto es nulo.");
            }

            _productosRepository.CrearProducto(producto);
            return CreatedAtAction(nameof(ObtenerProducto), new { id = producto.IdProducto }, producto);
        }


        // GET api/Producto
        [HttpGet]
        public IActionResult ListarProductos()
        {
            var productos = _productosRepository.ListarProductos();
            return Ok(productos);
        }

        // PUT api/Producto/{id}
        [HttpPut("{id}")]
        public IActionResult ModificarProducto(int id, [FromBody] Productos producto)
        {
            if (producto == null || producto.IdProducto != id)
            {
                return BadRequest("Producto no válido.");
            }

            var productoExistente = _productosRepository.DetallarProducto(id);
            if (productoExistente == null)
            {
                return NotFound("Producto no encontrado.");
            }

            _productosRepository.ModificarProducto(id, producto);
            return NoContent(); // Indica que la modificación fue exitosa sin retornar contenido
        }

        // GET api/Producto/{id} (para obtener un producto específico)
        [HttpGet("{id}")]
        public IActionResult ObtenerProducto(int id)
        {
            var producto = _productosRepository.DetallarProducto(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }
            return Ok(producto);
        }
}