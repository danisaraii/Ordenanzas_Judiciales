using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Collections.Specialized;
namespace OrdenanzasJudiciales.Infraestructura.Data.Juzgados
{
    public class DatosOrdenanzasJuzgados : IServicioJuzgados
    {
        private readonly IConfiguration _configuration;
        public DatosOrdenanzasJuzgados(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<catalogosModel>> LeerJuzgadosSecretariosAsync(
            string nombreProcedimiento)
        {
            var lista = new List<catalogosModel>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(nombreProcedimiento, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                var resultadoParam = new SqlParameter("@resultado", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultadoParam);

                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lista.Add(new catalogosModel
                    {
                        funcionarioId = Convert.ToInt32(reader["ID_Secretario"]),
                        juzgadoId = Convert.ToInt32(reader["ID_Juzgado"]),
                        nombreJuzgado = reader["nombre_juzgado"].ToString(),
                        nombreBeneficiario = reader["alias"].ToString(),
                        direccion = reader["direccion"].ToString(),
                        ciudad = reader["ciudad"].ToString(),
                        funcionario = reader["nombre_secretario"].ToString(),
                        correo = reader["correo"].ToString()
                    });
                }

                await reader.CloseAsync();
                return lista;
            }
        }
        public async Task<(string mensaje, int codigoErr)> InsertarJuzgadoAsync(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();

            foreach (var param in parametros)
                parameters.Add(param.Key, param.Value);

            parameters.Add("@resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
            parameters.Add("@codigoErr", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(nombreProcedimiento, parameters, commandType: CommandType.StoredProcedure);

            string resultado = parameters.Get<string>("@resultado");
            int codigo = parameters.Get<int>("@codigoErr");

            return (resultado, codigo);
        }
        public async Task<(string mensaje, int codigoErr)> InsertarFuncionarioAsync(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();

            // Añadir parámetros de entrada
            foreach (var param in parametros)
                parameters.Add(param.Key, param.Value);

            // Añadir parámetros de salida
            parameters.Add("@resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
            parameters.Add("@codigoErr", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Ejecutar SP
            await connection.ExecuteAsync(nombreProcedimiento, parameters, commandType: CommandType.StoredProcedure);

            // Obtener valores de salida
            string resultado = parameters.Get<string>("@resultado");
            int codigo = parameters.Get<int>("@codigoErr");

            return (resultado, codigo);

        }

        public async Task<List<CuentasModel>> LeerCuentasAsync(
            string nombreProcedimiento)
        {
            var lista = new List<CuentasModel>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(nombreProcedimiento, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                var resultadoParam = new SqlParameter("@resultado", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultadoParam);

                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    lista.Add(new CuentasModel
                    {
                        idCuenta = Convert.ToInt32(reader["IdCuenta"]),
                        idAcreedor = Convert.ToInt32(reader["IdAcreedor"]),
                        numeroCuenta = reader["NumeroCuenta"].ToString(),
                        tipoCuenta = reader["TipoCuenta"].ToString(),
                        nombreAcreedor = reader["NombreAcreedor"].ToString(),
                        rucCedula = reader["RUC_Cedula"].ToString(),
                        idEntidad = Convert.ToInt32(reader["IdEntidadBancaria"]),
                        nombreEntidad = reader["NombreEntidad"].ToString()
                        //correo = reader["correo"].ToString()
                    });
                }

                await reader.CloseAsync();
                return lista;
            }
        }

        public async Task<DataTable> EjecutarSPGenericoAsync(string nombreProcedimiento)
        {
            var dataTable = new DataTable();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(nombreProcedimiento, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

    }
}
