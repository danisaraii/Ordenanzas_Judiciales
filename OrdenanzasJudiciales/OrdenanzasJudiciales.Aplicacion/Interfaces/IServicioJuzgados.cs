using OrdenanzasJudiciales.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace OrdenanzasJudiciales.Aplicacion.Interfaces
{
    public interface IServicioJuzgados
    {
       
        Task<List<catalogosModel>> LeerJuzgadosSecretariosAsync(string procedimiento);
        Task<List<CuentasModel>> LeerCuentasAsync(string procedimiento);
        Task<(string mensaje, int codigoErr)> InsertarJuzgadoAsync(string procedimiento, Dictionary<string, object> parametros);
        Task<(string mensaje, int codigoErr)> InsertarFuncionarioAsync(string procedimiento, Dictionary<string, object> parametros);
        Task<DataTable> EjecutarSPGenericoAsync(string procedimiento);

        //Task<(string mensaje, int codigoErr)> EditarFuncionarioAsync(string procedimiento, Dictionary<string, object> parametros);
        //Task<List<CuentasModel>> LeerCuentasAsync(string procedimiento);

    }
}
