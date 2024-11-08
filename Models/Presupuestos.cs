using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Models;

public class Presupuestos
{
    private int idPresupuesto;
    private string nombreDestinatario;
    private List<PresupuestoDetalle> detalle= new List<PresupuestoDetalle>();

    public Presupuestos(int idPresupuesto, string nombreDestinatario)
    {
        IdPresupuesto = idPresupuesto;
        this.NombreDestinatario = nombreDestinatario;
    }
    public Presupuestos(){}

    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
    internal List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }

    public int MontoPresupuesto() {
        return this.Detalle.Sum(d=> d.Producto.Precio * d.Cantidad);
    }

    public double MontoPresupuestoConIva() {
        return  this.MontoPresupuesto()*1.21;
    }

    public int CantidadProductos() {
        return this.Detalle.Sum(d=> d.Cantidad);
    }

    
}