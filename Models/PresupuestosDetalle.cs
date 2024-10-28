using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Models;

class PresupuestoDetalle
{
    private Productos producto;
    private int cantidad;

    public PresupuestoDetalle(Productos producto, int cantidad)
    {
        this.Producto = producto;
        this.Cantidad = cantidad;
    }

    public int Cantidad { get => cantidad; set => cantidad = value; }
    internal Productos Producto { get => producto; set => producto = value; }

}