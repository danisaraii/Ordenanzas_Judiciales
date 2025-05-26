using OrdenanzasJudiciales.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Aplicacion.Interfaces
{
    public interface IProcesosOrdenanzasRepositorio
    {
        Task<IEnumerable<procesosOrdenanzas>> ObtenerReporteAsync();

        //Task InsertarDatosAsync(datosProceso datos);        
        Task InsertarDatosAsync(string nombreProcedimiento, Dictionary<string, object> parametros);
        Task EjecutarProcedimientoAsync(string nombreProcedimiento);       
        Task<ResultadoConsulta> EjecutarResultadoAsync(string nombreProcedimiento, Dictionary<string, object> parametros);



    }

}

