using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.EntityFrameworkCore;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    [Keyless]
    public class procesosOrdenanzas
    {
        public int idCargaArchivo { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        public string nombreTabla { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        public string nombreArchivo { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        public string url { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaUltimaEdicion { get; set; }
        public string usuarioEdit { get; set; }
        public int fkIdProceso { get; set; }
        public int fkIdTipoArchivo { get; set; }
        public string nombre { get; set; }
    }

    public class ResultadoConsulta
    {
        public DataTable Datos { get; set; }
        public int CodigoError { get; set; }
        public string Mensaje { get; set; }
    }

    public class datosProceso
    {
        public int proceso { get; set; }
        public string nombre { get; set; }
        public string identificacion { get; set; }
        public string tipo { get; set; }
        public string cuenta { get; set; }
        public string juicio { get; set; }
        public float monto { get; set; }
        public string juzgado { get; set; }
        public string oficioretencion { get; set; }
        public string tramite { get; set; }
        public string usuario { get; set; }
        public DateTime fechaCreacion { get; set; }
    }
}
