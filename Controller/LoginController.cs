using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataBase;

namespace Controller
{
    public class LoginController
    {
        private readonly DataBaseConnection dbConnection;

        public LoginController()
        {
            dbConnection = new DataBaseConnection();
        }

        // Método para validar las credenciales del usuario
        public (string Puesto, int IdEmpleado, string NombreUsuario, string Contraseña) ValidarCredenciales(string nombreUsuario, string contraseña)
        {
            using (SqlConnection connection = new SqlConnection(dbConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Consulta para verificar el usuario y la contraseña y obtener los datos necesarios
                    string query = "SELECT Puesto, ID_empleado, NombreUsuario, Contraseña FROM Empleados WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña";
                    
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    command.Parameters.AddWithValue("@Contraseña", contraseña);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string puesto = reader["Puesto"].ToString();
                            int idEmpleado = Convert.ToInt32(reader["ID_empleado"]);
                            string nombreUsuarioDb = reader["NombreUsuario"].ToString();
                            string contraseñaDb = reader["Contraseña"].ToString();
                            return (puesto, idEmpleado, nombreUsuarioDb, contraseñaDb); // Retorna los 4 datos
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error al validar credenciales: {ex.Message}");
                }
            }

            return (null, 0, null, null); // Retorna valores nulos si las credenciales son incorrectas
        }


        // Método para verificar si un empleado puede vender autos
        public bool PuedeVenderAutos(string nombreUsuario, string contraseña)
        {
            var (puesto, _, _, _) = ValidarCredenciales(nombreUsuario, contraseña);
            return puesto == "Empleado" || puesto == "Gerente";
        }

        // Método para verificar si un empleado puede agregar autos
        public bool PuedeAgregarAutos(string nombreUsuario, string contraseña)
        {
            var (puesto, _, _, _) = ValidarCredenciales(nombreUsuario, contraseña);
            return puesto == "Gerente";
        }
    }
}

