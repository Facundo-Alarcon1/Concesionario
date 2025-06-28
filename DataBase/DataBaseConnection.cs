using System;
using System.Data.SqlClient;

namespace DataBase
{
    public class DataBaseConnection
    {
        public string connectionString;

        public DataBaseConnection()
        {
            // Define la cadena de conexión 
            connectionString = "Server=DESKTOP-SG673N7\\SQLEXPRESS;Database=Concesionaria;Trusted_Connection=True;";
        }

        public void Connect()
        {
            SqlConnection connection = null;

            try
            {
                // Crear la conexión
                connection = new SqlConnection(connectionString);

                // Abrir la conexión
                connection.Open();
                Console.WriteLine("Conexión exitosa a la base de datos.");
            }
            catch (SqlException ex)
            {
                // Manejar errores de SQL
                Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
            finally
            {
                // Cerrar la conexión
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Conexión cerrada.");
                }
            }
        }
    }
}
