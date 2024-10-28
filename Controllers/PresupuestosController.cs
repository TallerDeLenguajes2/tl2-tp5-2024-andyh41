using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Repositorios;

namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class PresupuestosController : ControllerBase
{
    
    private readonly PresupuestosRepository _presupuestosRepository;

    public PresupuestosController()
    {
        _presupuestosRepository = new PresupuestosRepository();
    }

    // POST api/Presupuesto
    [HttpPost]
    public IActionResult CrearPresupuesto([FromBody] Presupuestos presupuesto)
    {
        if (presupuesto == null)
        {
            return BadRequest("El presupuesto es nulo.");
        }
        
        _presupuestosRepository.CrearPresupuesto(presupuesto);
        return CreatedAtAction(nameof(ObtenerPresupuesto), new { id = presupuesto.IdPresupuesto }, presupuesto);
    }

    // POST api/Presupuesto/{id}/ProductoDetalle
    [HttpPost("{id}/ProductoDetalle")]
    public IActionResult AgregarProductoDetalle(int id, [FromBody] PresupuestoDetalle detalle)
    {
        if (detalle == null || detalle.Producto == null)
        {
            return BadRequest("El detalle del producto es nulo.");
        }

        _presupuestosRepository.AgregarDetalle(id, detalle.Producto.IdProducto, detalle.Cantidad);
        return NoContent();
    }

    // GET api/presupuesto
    [HttpGet]
    public ActionResult<List<Presupuestos>> ListarPresupuestos()
    {
        var presupuestos = _presupuestosRepository.ListarPresupuestos();
        return Ok(presupuestos);
    }

    // GET api/Presupuesto/{id}
    [HttpGet("{id}")]
    public ActionResult<Presupuestos> ObtenerPresupuesto(int id)
    {
        var presupuesto = _presupuestosRepository.ObtenerPresupuesto(id);
        if (presupuesto == null)
        {
            return NotFound();
        }
        return Ok(presupuesto);
    }
}