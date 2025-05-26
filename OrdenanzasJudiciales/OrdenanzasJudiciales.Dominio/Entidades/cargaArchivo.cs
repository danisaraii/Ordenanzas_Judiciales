using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class cargaArchivo
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
}
