using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Infraestructura.Data.Context;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace OrdenanzasJudiciales.Infraestructura.Data.Repositorios
{
    public class ProcesosOrdenanzasRepositorio : IProcesosOrdenanzasRepositorio
    {
        private readonly IConfiguration _configuration;

        public ProcesosOrdenanzasRepositorio(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<procesosOrdenanzas>> ObtenerReporteAsync()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var resultado = await connection.QueryAsync<procesosOrdenanzas>(
                    "cargarArchivosPorProceso",
                    commandType: CommandType.StoredProcedure
                );

                return resultado;
            }
        }       

        public async Task InsertarDatosAsync(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var dapperParams = new DynamicParameters();

                if (parametros != null)
                {
                    foreach (var param in parametros)
                    {
                        dapperParams.Add(param.Key, param.Value);
                    }
                }

                await connection.ExecuteAsync(
                    nombreProcedimiento,
                    dapperParams,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task EjecutarProcedimientoAsync(string nombreProcedimiento)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.ExecuteAsync(nombreProcedimiento, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<ResultadoConsulta> EjecutarResultadoAsync(string nombreProcedimiento, Dictionary<string, object> parametros)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand(nombreProcedimiento, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Agregar parámetros de entrada
                foreach (var param in parametros)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                // Agregar parámetros de salida
                var errorParam = new SqlParameter("@codigoErr", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var mensajeParam = new SqlParameter("@resultado", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                command.Parameters.Add(errorParam);
                command.Parameters.Add(mensajeParam);

                var dataTable = new DataTable();
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(reader);
                }

                // Crear objeto con los resultados
                var resultado = new ResultadoConsulta
                {
                    Datos = dataTable,
                    CodigoError = (int)(errorParam.Value ?? 0),
                    Mensaje = mensajeParam.Value?.ToString()
                };

                return resultado;
            }
        }



    }

}
