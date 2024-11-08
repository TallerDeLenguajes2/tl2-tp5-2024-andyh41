using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Models;

namespace Repositorios; 

public class PresupuestosRepository()
{
    private readonly string connectionString = "Data Source=Db/Tienda.db;Cache=Shared";
     ProductosRepository repoProd = new ProductosRepository();

    public void CrearPresupuesto(Presupuestos pres)
    {
        // primero modifica la tabla presupuestos, agregando id nombre y fecha
        string queryString = "INSERT INTO Presupuestos (idPresupuesto, NombreDestinatario, FechaCreacion) VALUES (@Id, @Nombre, @Fecha);";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Id", pres.IdPresupuesto);
                command.Parameters.AddWithValue("@Nombre", pres.NombreDestinatario);
                command.Parameters.AddWithValue("@Fecha",  DateTime.Now.ToString("yyyy-MM-dd"));
                
                command.ExecuteNonQuery();
            }
        }

        // luego modifica la tabla presupuestos detalle, agregando 1 fila por producto y especificando la cantidad
        for (int i = 0; i < pres.Detalle.Count()  ; i++)
        {
            //AgregarDetalle(pres.IdPresupuesto,pres.Detalle[i].Producto.IdProducto,pres.Detalle[i].Cantidad);
            string queryString2 = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@Idpres, @Idprod, @Cantidad);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(queryString2, connection))
                {
                    command.Parameters.AddWithValue("@Idpres", pres.IdPresupuesto);
                    command.Parameters.AddWithValue("@Idprod", pres.Detalle[i].Producto.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad",  pres.Detalle[i].Cantidad);
                    
                    command.ExecuteNonQuery();
                }
            }

        }
    }

  

    public List<Presupuestos> ListarPresupuestos()
    {
        var presupuestos = new List<Presupuestos>();

        // Abre la conexión 
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string queryString = @"
                SELECT p.idPresupuesto, p.NombreDestinatario, pd.idProducto, pd.Cantidad 
                FROM Presupuestos p
                LEFT JOIN PresupuestosDetalles pd ON p.idPresupuesto = pd.idPresupuesto;
            ";

            using (var command = new SqliteCommand(queryString, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Presupuestos presupuesto = null;

                    while (reader.Read())
                    {
                        // Si es el primer registro de un nuevo presupuesto, crea una nueva instancia
                        if (presupuesto == null || presupuesto.IdPresupuesto != reader.GetInt32(0))
                        {
                            // Si ya existe un presupuesto, lo añadimos a la lista
                            if (presupuesto != null)
                            {
                                presupuestos.Add(presupuesto);
                            }

                            // Crear un nuevo presupuesto
                            presupuesto = new Presupuestos
                            {
                                IdPresupuesto = reader.GetInt32(0),
                                NombreDestinatario = reader.GetString(1),
                                Detalle = new List<PresupuestoDetalle>()
                            };
                        }

                        // Si el producto es válido, añadirlo al detalle del presupuesto
                        if (!reader.IsDBNull(2)) // Si idProducto no es null
                        {
                            var detalle = new PresupuestoDetalle
                            (
                                repoProd.DetallarProducto(reader.GetInt32(2)), // Obtiene el producto por idProducto
                                reader.GetInt32(3) // La cantidad del producto
                            );

                            // Agregar el detalle al presupuesto actual
                            presupuesto.Detalle.Add(detalle);
                        }
                    }

                    // Al final del ciclo, añade el último presupuesto (si existe)
                    if (presupuesto != null)
                    {
                        presupuestos.Add(presupuesto);
                    }
                }
            }
        }

        return presupuestos;
    }




    public Presupuestos ObtenerPresupuesto(int id)
    {
        string queryString = @"
            SELECT p.idPresupuesto, p.NombreDestinatario, pd.idProducto, pd.Cantidad 
            FROM Presupuestos p 
            LEFT JOIN PresupuestosDetalles pd ON p.idPresupuesto = pd.idPresupuesto 
            WHERE p.idPresupuesto = @id;
        ";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                // Corregir el nombre del parámetro para que sea consistente con la consulta
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    Presupuestos presupuesto = null; // Inicializa la variable presupuesto

                    // Lee los registros
                    while (reader.Read())
                    {
                        // Si aún no hemos creado el objeto presupuesto, lo hacemos en el primer registro
                        if (presupuesto == null)
                        {
                            presupuesto = new Presupuestos
                            {
                                IdPresupuesto = reader.GetInt32(0),  // Primer columna: IdPresupuesto
                                NombreDestinatario = reader.GetString(1) // Segunda columna: NombreDestinatario
                            };
                        }

                        // Verifica si el idProducto es nulo (es decir, si hay detalles de presupuesto)
                        if (!reader.IsDBNull(2)) // Si idProducto no es null
                        {
                            var detallar = new PresupuestoDetalle
                            (
                                repoProd.DetallarProducto(reader.GetInt32(2)), // Obtiene el producto a partir de idProducto
                                reader.GetInt32(3) // La cantidad del producto
                            );
                            // Agrega el detalle al presupuesto
                            presupuesto.Detalle.Add(detallar);
                        }
                    }

                    // Devuelve el presupuesto si existe, de lo contrario, devuelve null
                    return presupuesto;
                }
            }
        }

        // Si no se encuentra el presupuesto, devuelve null
        return null;
    }



    public void AgregarDetalle(int id, int idProd, int cant){
        string queryString = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@Idpres, @Idprod, @Cantidad);";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Idpres", id);
                command.Parameters.AddWithValue("@Idprod", idProd);
                command.Parameters.AddWithValue("@Cantidad", cant);
                    
                command.ExecuteNonQuery();
            }
        }
    }



    public void EliminarPresupuesto(int id)
    {
        string queryString = "DELETE FROM Presupuestos WHERE idPresupuesto = @Id;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        string queryString2 = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @Id;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString2, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

    }



}