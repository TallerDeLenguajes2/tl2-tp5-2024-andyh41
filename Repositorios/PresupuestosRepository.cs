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

    public List<Presupuestos> ListarPresupuestos(){

        // crea la lista de presupuestos que se va a devolver
        var presupuestos = new List<Presupuestos>();

        // realiza una primera consulta, tomando el id y nombre para crear el objeto presupuesto
        string queryString = "SELECT idPresupuesto, NombreDestinatario FROM Presupuestos;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // crea el objeto
                        var presupuesto = new Presupuestos
                        (
                            reader.GetInt32(0), // Primer columna: Id
                            reader.GetString(1) // Segunda columna: Nombre
                        );

                        // realiza una segunda consulta para el detalle del presupuesto
                        string queryString2 = "SELECT idProducto, Cantidad FROM PresupuestosDetalle WHERE idPresupuesto = @id;";
                        
                        using (var command2 = new SqliteCommand(queryString2, connection))
                        {
                            command2.Parameters.AddWithValue("@Id", presupuesto.IdPresupuesto);
                            using(var reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    var detallar = new PresupuestoDetalle
                                    (
                                        DetallarProducto(reader2.GetInt32(1)), // crea un producto a partir del id de producto de la tabla para agregarlo al objeto detalle
                                        reader2.GetInt32(2) // la cantidad del producto
                                    );
                                    // agrega el detalle a la lista de detalles del objeto
                                    presupuesto.Detalle.Add(detallar);
                                }
                                
                            }
                        }
                        // agrega el objeto a la lista que sera devuelta
                        presupuestos.Add(presupuesto);
                    }
                }
            }
        }
        return presupuestos;
    }


    public Presupuestos ObtenerPresupuesto(int id){
        // return (Presupuestos)ListarPresupuestos().Select(l=> l.IdPresupuesto=id); // metodo en una linea, que crea una lista y selecciona el objeto por el id
        // metodo que no crea una lista, solo crea el objeto
        // realiza una primera consulta para crear el objeto
        string queryString = "SELECT idPresupuesto, NombreDestinatario FROM Presupuestos WHERE idPresupuesto = @Id;";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using(var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // crea el objeto
                        var presupuesto = new Presupuestos
                        (
                            reader.GetInt32(0), // Primer columna: Id
                            reader.GetString(1) // Segunda columna: Nombre
                        );

                        // realiza una segunda consulta para el detalle del presupuesto
                        string queryString2 = "SELECT idProducto, Cantidad FROM PresupuestosDetalle WHERE idPresupuesto = @id;";
                        
                        using (var command2 = new SqliteCommand(queryString2, connection))
                        {
                            command2.Parameters.AddWithValue("@Id", id);
                            using(var reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    var detallar = new PresupuestoDetalle
                                    (
                                        DetallarProducto(reader2.GetInt32(1)), // crea un producto a partir del id de producto de la tabla para agregarlo al objeto detalle
                                        reader2.GetInt32(2) // la cantidad del producto
                                    );
                                    // agrega el detalle a la lista de detalles del objeto
                                    presupuesto.Detalle.Add(detallar);
                                }   
                            }
                        }
                        return presupuesto;
                    }
                }
            }
        }
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