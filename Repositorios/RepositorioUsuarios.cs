using System;
using System.Text;
using Inmobiliaria.Models;
using MySql.Data.MySqlClient;
using static Inmobiliaria.Models.Usuario;

namespace Inmobiliaria.Repositorios
{
    // Clase RepositorioUsuarios que hereda de RepositorioBD
    public class RepositorioUsuarios : InmobiliariaBD.RepositorioBD
    {
        public RepositorioUsuarios() { }

        // Método para Listar todos los usuarios
        public List<Usuario> ListarUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"SELECT {nameof(Usuario.Id_usuario)}, {nameof(Usuario.Nombre)}, 
                            {nameof(Usuario.Apellido)}, {nameof(Usuario.Avatar)}, 
                            {nameof(Usuario.Email)}, {nameof(Usuario.Password)}, 
                            {nameof(Usuario.Rol)}, {nameof(Usuario.Fecha)}
                            FROM usuario
                            WHERE borrado = 0;";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Manejo de los Enum en C#
                                string uso = reader.GetString(nameof(Usuario.Rol));
                                Roles RolEnum;
                                Enum.TryParse(uso, out RolEnum);
                                // Fin Manejo de los Enum en C#

                                Usuario usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(nameof(Usuario.Id_usuario)),
                                    Nombre = reader.GetString(nameof(Usuario.Nombre)),
                                    Apellido = reader.GetString(nameof(Usuario.Apellido)),
                                    Avatar = reader.GetString(nameof(Usuario.Avatar)),
                                    Email = reader.GetString(nameof(Usuario.Email)),
                                    Password = reader.GetString(nameof(Usuario.Password)),
                                    Rol = RolEnum,
                                    Fecha = reader.GetDateTime(nameof(Usuario.Fecha))
                                };
                                usuarios.Add(usuario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return usuarios;
        }

        // Método para obtener un usuario por su ID
        public Usuario ObtenerUsuarioPorId(int id)
        {
            Usuario? usuario = null;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"SELECT {nameof(Usuario.Id_usuario)}, {nameof(Usuario.Nombre)}, 
                            {nameof(Usuario.Apellido)}, {nameof(Usuario.Avatar)}, 
                            {nameof(Usuario.Email)}, {nameof(Usuario.Rol)}
                            FROM usuario
                            WHERE id_usuario = @id AND borrado = 0;";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Manejo de los Enum en C#
                                string uso = reader.GetString(nameof(Usuario.Rol));
                                Roles RolEnum;
                                Enum.TryParse(uso, out RolEnum);
                                // Fin Manejo de los Enum en C#

                                usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(nameof(Usuario.Id_usuario)),
                                    Nombre = reader.GetString(nameof(Usuario.Nombre)),
                                    Apellido = reader.GetString(nameof(Usuario.Apellido)),
                                    Avatar = reader.GetString(nameof(Usuario.Avatar)),
                                    Email = reader.GetString(nameof(Usuario.Email)),
                                    Rol = RolEnum,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario con id {id}: {ex.Message}");
            }
            return usuario;
        }

        // Método para crear un nuevo usuario
        public int CrearUsuario(Usuario usuario)
        {
            Console.WriteLine($"Creando usuario: {usuario.Avatar}, {usuario.AvatarFile}");
            int resultado = -1;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"INSERT INTO usuario ({nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, 
                                    {nameof(Usuario.Avatar)}, {nameof(Usuario.Email)}, 
                                    {nameof(Usuario.Password)}, {nameof(Usuario.Rol)})
                                VALUES (@{nameof(Usuario.Nombre)}, @{nameof(Usuario.Apellido)}, 
                                    @{nameof(Usuario.Avatar)}, @{nameof(Usuario.Email)}, 
                                    @{nameof(Usuario.Password)}, @{nameof(Usuario.Rol)});
                                SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue(
                            $"@{nameof(Usuario.Nombre)}",
                            usuario.Nombre
                        );
                        command.Parameters.AddWithValue(
                            $"@{nameof(Usuario.Apellido)}",
                            usuario.Apellido
                        );
                        command.Parameters.AddWithValue(
                            $"@{nameof(Usuario.Avatar)}",  
                            usuario.Avatar
                        );
                        command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", usuario.Email);
                        command.Parameters.AddWithValue(
                            $"@{nameof(Usuario.Password)}",
                            usuario.Password
                        );
                        command.Parameters.AddWithValue($"@{nameof(Usuario.Rol)}", usuario.Rol);

                        connection.Open();
                        resultado = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el usuario en el repositorio: {ex.Message}");
            }
            return resultado;
        }

        // Método para actualizar un usuario
        public int ActualizarUsuario(Usuario usuario)
        {
            int resultado = -1;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"UPDATE usuario SET {nameof(Usuario.Nombre)} = @Nombre, 
                                    {nameof(Usuario.Apellido)} = @Apellido, 
                                    {nameof(Usuario.Avatar)} = @Avatar, 
                                    {nameof(Usuario.Email)} = @Email, 
                                    {nameof(Usuario.Rol)} = @Rol
                                WHERE {nameof(Usuario.Id_usuario)} = @Id AND borrado = 0;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", usuario.Id_usuario);
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Avatar", usuario.Avatar);
                        command.Parameters.AddWithValue("@Email", usuario.Email);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol);

                        connection.Open();
                        resultado = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resultado;
        }

        // Método para eliminar (lógico) un usuario
        public int EliminarUsuario(int id)
        {
            int resultado = -1;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql = @$"UPDATE usuario SET borrado = 1 WHERE id_usuario = @id";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        connection.Open();
                        resultado = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resultado;
        }

        // Metodo para verificar si existe el usuario
        public bool ExisteUsuario(string email)
        {
            bool existe = false;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"SELECT COUNT(*) FROM usuario WHERE email = @email AND borrado = 0;";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        connection.Open();
                        existe = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return existe;
        }

        // Método para obtener un usuario por su email y contraseña
        public Usuario? BuscarLogin(string email, string password)
        {
            Usuario? usuario = null;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    var sql =
                        @$"SELECT {nameof(Usuario.Id_usuario)}, {nameof(Usuario.Nombre)}, 
                                        {nameof(Usuario.Apellido)}, {nameof(Usuario.Avatar)}, 
                                        {nameof(Usuario.Email)}, {nameof(Usuario.Rol)}
                                FROM usuario
                                WHERE {nameof(Usuario.Email)} = @Email 
                                AND {nameof(Usuario.Password)} = @Password
                                AND borrado = 0;"; // Consideramos solo los usuarios no borrados

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string rolStr = reader.GetString(nameof(Usuario.Rol));
                                Roles rolEnum;
                                Enum.TryParse(rolStr, out rolEnum);

                                usuario = new Usuario
                                {
                                    Id_usuario = reader.GetInt32(nameof(Usuario.Id_usuario)),
                                    Nombre = reader.GetString(nameof(Usuario.Nombre)),
                                    Apellido = reader.GetString(nameof(Usuario.Apellido)),
                                    Avatar = reader.GetString(nameof(Usuario.Avatar)),
                                    Email = reader.GetString(nameof(Usuario.Email)),
                                    Rol = rolEnum
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario: {ex.Message}");
            }
            return usuario;
        }
    }
}
