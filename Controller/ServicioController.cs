using System;
using System.Collections.Generic;
using System.Text;

namespace Controller
{
    using DataBase;
    using Model.Clases;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class ServicioController
    {
        private readonly DataBaseConnection dbConnection;

        public ServicioController()
        {
            dbConnection = new DataBaseConnection();
        }

        // Método para agregar un servicio
        public bool AgregarServicio(Servicio servicio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection.connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO Servicios (Descripcion, Fecha, Estado, ID_empleado) " +
                                   "VALUES (@Descripcion, @Fecha, @Estado, @ID_empleado)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                    cmd.Parameters.AddWithValue("@Fecha", servicio.Fecha);
                    cmd.Parameters.AddWithValue("@Estado", "En proceso"); // Siempre se agrega como "En proceso"
                    cmd.Parameters.AddWithValue("@ID_empleado", servicio.ID_empleado);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar servicio: " + ex.Message);
                return false;
            }
        }

        // Método para marcar un servicio como realizado
        public bool MarcarComoRealizado(int idServicio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection.connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Servicios SET Estado = 'Realizado' WHERE ID_servicio = @ID_servicio";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_servicio", idServicio);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al marcar como realizado: " + ex.Message);
                return false;
            }
        }

        // Método para eliminar un servicio
        public bool EliminarServicio(int idServicio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection.connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM Servicios WHERE ID_servicio = @ID_servicio";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_servicio", idServicio);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0; // Si se elimina al menos un servicio, devolvemos true
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar servicio: " + ex.Message);
                return false;
            }
        }

        // Método para obtener todos los servicios
        public List<Servicio> ObtenerTodosLosServicios()
        {
            List<Servicio> lista = new List<Servicio>();

            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnection.connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Servicios";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicio servicio = new Servicio()
                            {
                                ID_servicio = reader.GetInt32(0),
                                Descripcion = reader.GetString(1),
                                Fecha = reader.GetDateTime(2),
                                Estado = reader.GetString(3),
                                ID_empleado = reader.GetInt32(4)
                            };
                            lista.Add(servicio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener servicios: " + ex.Message);
            }

            return lista;
        }
    }

}
