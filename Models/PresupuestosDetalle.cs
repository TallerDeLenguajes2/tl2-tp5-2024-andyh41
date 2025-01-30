using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Models;

public class PresupuestoDetalle
{
    private Productos producto;
    private int cantidad;

    public PresupuestoDetalle(Productos producto, int cantidad)
    {
        this.Producto = producto;
        this.Cantidad = cantidad;
    }

    public int Cantidad { get => cantidad; set => cantidad = value; }
    public Productos Producto { get => producto; set => producto = value; }

 
}