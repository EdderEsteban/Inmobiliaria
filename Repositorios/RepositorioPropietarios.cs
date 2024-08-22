using Inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria.Repositorios;

// Clase RepositorioPropietarios que hereda de RepositorioBD
public class RepositorioPropietarios : InmobiliariaBD.RepositorioBD
{
    public RepositorioPropietarios() { }

    // Método para listar todos los propietarios
    public IList<Propietarios> ListarPropietarios()
    {
        var propietarios = new List<Propietarios>();
        try
        {
            // Se obtiene la conexión a la base de datos
            using (var connection = GetConnection())
            {
                // Consulta SQL para seleccionar todos los campos de la tabla propietario
                var sql =
                    @$"SELECT {nameof(Propietarios.Id_Propietario)}, {nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                    {nameof(Propietarios.Dni)}, {nameof(Propietarios.Direccion)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Correo)} 
                    FROM propietario";

                // Creación del comando SQL
                using (var command = new MySqlCommand(sql, connection))
                {
                    // Apertura de la conexión
                    connection.Open();
                    // Ejecución del comando y obtención de un lector de datos
                    using (var reader = command.ExecuteReader())
                    {
                        // Lectura de cada registro y creación de objetos Propietarios
                        while (reader.Read())
                        {
                            propietarios.Add(
                                new Propietarios
                                {
                                    Id_Propietario = reader.GetInt32(nameof(Propietarios.Id_Propietario)),
                                    Nombre = reader.GetString(nameof(Propietarios.Nombre)),
                                    Apellido = reader.GetString(nameof(Propietarios.Apellido)),
                                    Dni = reader.GetString(nameof(Propietarios.Dni)),
                                    Direccion = reader.GetString(nameof(Propietarios.Direccion)),
                                    Telefono = reader.GetString(nameof(Propietarios.Telefono)),
                                    Correo = reader.GetString(nameof(Propietarios.Correo))
                                }
                            );
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception("Error al listar propietarios", ex);
        }
        return propietarios;
    }

    // Método para guardar un nuevo propietario
    public int GuardarNuevo(Propietarios propietario)
    {
        int id = 0;
        try
        {
            using (var connection = GetConnection())
            {
                // Consulta SQL para insertar un nuevo propietario y obtener el ID generado
                var sql =
                    @$"INSERT INTO propietario ({nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                    {nameof(Propietarios.Dni)}, {nameof(Propietarios.Direccion)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Correo)})
                    VALUES (@{nameof(Propietarios.Nombre)}, @{nameof(Propietarios.Apellido)}, @{nameof(Propietarios.Dni)},
                    @{nameof(Propietarios.Direccion)}, @{nameof(Propietarios.Telefono)}, @{nameof(Propietarios.Correo)});
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    // Asignación de parámetros al comando
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Nombre)}", propietario.Nombre);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Apellido)}", propietario.Apellido);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Dni)}", propietario.Dni);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Direccion)}", propietario.Direccion);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Telefono)}", propietario.Telefono);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Correo)}", propietario.Correo);

                    connection.Open();
                    // Ejecución del comando y obtención del ID insertado
                    id = Convert.ToInt32(command.ExecuteScalar());
                    propietario.Id_Propietario = id;
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception("Error al guardar el propietario", ex);
        }
        return id;
    }

    // Método para obtener un propietario por su ID
    public Propietarios? ObtenerPropietario(int id)
    {

        Propietarios? propietario = null;
        try
        {
            using (var connection = GetConnection())
            {
                // Consulta SQL para seleccionar un propietario por su ID
                var sql =
                    @$"SELECT {nameof(Propietarios.Id_Propietario)}, {nameof(Propietarios.Nombre)}, {nameof(Propietarios.Apellido)}, 
                    {nameof(Propietarios.Dni)}, {nameof(Propietarios.Direccion)}, {nameof(Propietarios.Telefono)}, {nameof(Propietarios.Correo)} 
                    FROM propietario
                    WHERE {nameof(Propietarios.Id_Propietario)} = @{nameof(Propietarios.Id_Propietario)}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Id_Propietario)}", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Lectura del registro si existe
                        if (reader.Read())
                        {
                            propietario = new Propietarios
                            {
                                Id_Propietario = reader.GetInt32(nameof(Propietarios.Id_Propietario)),
                                Nombre = reader.GetString(nameof(Propietarios.Nombre)),
                                Apellido = reader.GetString(nameof(Propietarios.Apellido)),
                                Dni = reader.GetString(nameof(Propietarios.Dni)),
                                Direccion = reader.GetString(nameof(Propietarios.Direccion)),
                                Telefono = reader.GetString(nameof(Propietarios.Telefono)),
                                Correo = reader.GetString(nameof(Propietarios.Correo))
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception("Error al obtener el propietario", ex);
        }
        return propietario;
    }

    // Método para actualizar los datos de un propietario
    public void ActualizarPropietario(Propietarios propietario)
    {
        try
        {
            using (var connection = GetConnection())
            {
                // Consulta SQL para actualizar un propietario
                var sql =
                    @$"UPDATE propietario SET
                        {nameof(Propietarios.Nombre)} = @{nameof(Propietarios.Nombre)},
                        {nameof(Propietarios.Apellido)} = @{nameof(Propietarios.Apellido)},
                        {nameof(Propietarios.Dni)} = @{nameof(Propietarios.Dni)},
                        {nameof(Propietarios.Direccion)} = @{nameof(Propietarios.Direccion)},
                        {nameof(Propietarios.Telefono)} = @{nameof(Propietarios.Telefono)},
                        {nameof(Propietarios.Correo)} = @{nameof(Propietarios.Correo)}
                    WHERE {nameof(Propietarios.Id_Propietario)} = @{nameof(Propietarios.Id_Propietario)}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    // Asignación de parámetros al comando
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Nombre)}", propietario.Nombre);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Apellido)}", propietario.Apellido);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Dni)}", propietario.Dni);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Direccion)}", propietario.Direccion);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Telefono)}", propietario.Telefono);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Correo)}", propietario.Correo);
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Id_Propietario)}", propietario.Id_Propietario);

                    connection.Open();
                    // Ejecución del comando
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception("Error al actualizar el propietario", ex);
        }
    }

    // Método para verificar si un propietario tiene inmuebles asociados
    public int InmueblesAsociados(int idPropietario)
    {
        int cantidadInmuebles = 0;
        try
        {
            using (var connection = GetConnection())
            {
                // Consulta SQL para contar la cantidad de inmuebles asociados a un propietario
                var sql =
                    @$"SELECT COUNT(*) 
                FROM inmueble 
                WHERE {nameof(Inmueble.Id_propietario)} = @{nameof(Inmueble.Id_propietario)}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Id_propietario)}", idPropietario);

                    connection.Open();
                    // Ejecutar la consulta y obtener la cantidad de inmuebles
                    cantidadInmuebles = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception("Error al verificar los inmuebles asociados", ex);
        }
        return cantidadInmuebles;
    }

    // Método para eliminar un propietario por su ID
    public int EliminarPropietario(int id)
    {
        // Comprobar si el propietario tiene inmuebles asociados
        int inmueblesAsociados = InmueblesAsociados(id);

        if (inmueblesAsociados > 0)
        {
            //Console.WriteLine($"El propietario tiene inmuebles asociados {inmueblesAsociados}");
            
            throw new InvalidOperationException("El propietario tiene inmuebles asociados. Elimine primero los inmuebles.");
        }
        int filasAfectadas = 0;
        try
        {
            using (var connection = GetConnection())
            {
                // Consulta SQL para eliminar un propietario por su ID
                var sql =
                    @$"DELETE FROM propietario 
                    WHERE {nameof(Propietarios.Id_Propietario)} = @{nameof(Propietarios.Id_Propietario)}";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Propietarios.Id_Propietario)}", id);

                    connection.Open();
                    // Ejecución del comando y obtención del número de filas afectadas
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de excepciones y registro del error
            throw new Exception($"Error al eliminar el propietario {ex.Message}");
        }
        return filasAfectadas;
    }

    public int BuscarPropietario(Propietarios busqueda)
    {
        Console.WriteLine($"Buscando propietario... {busqueda}");
    return 0;
    }
}
