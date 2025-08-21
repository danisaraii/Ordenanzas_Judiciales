using System.Collections.Generic;
using OrdenanzasJudiciales.Dominio.Entidades;

namespace OrdenanzasJudiciales.Web.Models
{
    public class ReporteViewModel
    {
        //public cargaArchivo DatosPrincipales { get; set; }
        public IEnumerable<procesosOrdenanzas> DatosPrincipales { get; set; }
        public List<Dictionary<string, object>> DatosConsultaDia { get; set; }
    }
}
