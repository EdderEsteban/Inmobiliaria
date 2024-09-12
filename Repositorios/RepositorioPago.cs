using System;
using System.Text;
using Inmobiliaria.Models;
using MySql.Data.MySqlClient;

namespace Inmobiliaria.Repositorios;

public class RepositorioPago : InmobiliariaBD.RepositorioBD
{
    public RepositorioPago() { }

    public IList<Pago> ListarPagos()
    {
        var pagos = new List<Pago>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql =
                @"SELECT id_pago, id_contrato, fecha_pago, monto
                FROM pago";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add(
                            new Pago
                            {
                                Id_Pago = reader.GetInt32("id_pago"),
                                Id_Contrato = reader.GetInt32("id_contrato"),
                                Fecha_Pago = reader.GetDateTime("fecha_pago"),
                                Monto = reader.GetInt32("monto"),
                            }
                        );
                    }
                }
                connection.Close();
            }
        }
        return pagos;
    }

    public int GuardarNuevo(Pago pago)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql =
                @"INSERT INTO pago (id_contrato, fecha_pago, monto)
                            VALUES (@id_contrato, @fecha_pago, @monto);
                            SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_contrato", pago.Id_Contrato);
                command.Parameters.AddWithValue("@fecha_pago", pago.Fecha_Pago);
                command.Parameters.AddWithValue("@monto", pago.Monto);

                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return id;
    }

    public Pago ObtenerPago(int id)
    {
        Pago? pago = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql =
                @"SELECT id_pago, id_contrato, fecha_pago, monto
                            FROM pago
                            WHERE id_pago = @id_pago";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pago", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pago = new Pago
                        {
                            Id_Pago = reader.GetInt32("id_pago"),
                            Id_Contrato = reader.GetInt32("id_contrato"),
                            Fecha_Pago = reader.GetDateTime("fecha_pago"),
                            Monto = reader.GetInt32("monto"),
                        };
                    }
                }
                connection.Close();
            }
        }
        return pago;
    }

    /*public void ActualizarPago(Pago pago)
     {
         using (var connection = new MySqlConnection(ConnectionString))
         {
             var sql = @"UPDATE Pago
                             id_contrato = @id_inquilino,
                             fecha_pago = @id_inmueble,
                             monto = @monto,
                         WHERE id_contrato = @id_contrato";
             using (var command = new MySqlCommand(sql, connection))
             {
                 command.Parameters.AddWithValue("@id_inquilino", contrato.Id_inquilino);
                 command.Parameters.AddWithValue("@id_inmueble", contrato.Id_inmueble);
                 command.Parameters.AddWithValue("@monto", contrato.Monto);
                 command.Parameters.AddWithValue("@fecha_inicio", contrato.Fecha_inicio);
                 command.Parameters.AddWithValue("@fecha_fin", contrato.Fecha_fin);
                 command.Parameters.AddWithValue("@id_contrato", contrato.Id_contrato);

                 connection.Open();
                 command.ExecuteNonQuery();
                 connection.Close();
             }
         }
     }*/

    public void EliminarPago(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = "DELETE FROM pago WHERE id_pago = @id_pago";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pago", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
