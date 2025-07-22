using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasEditarDto
    {
        public int IdCuenta { get; set; }
        public int IdAcreedor { get; set; }
        public int IdEntidad { get; set; }        
        public string NombreAcreedor { get; set; }
        public string RucCedula { get; set; }
        public string EntidadBancaria { get; set; }
        [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
        [StringLength(20, ErrorMessage = "El número de cuenta no debe superar los 20 dígitos.")]
        [RegularExpression(@"^\d{5,20}$", ErrorMessage = "El número de cuenta debe contener solo entre 5 y 20 dígitos.")]
        public string numeroCuenta { get; set; }
        public string tipoCuenta { get; set; }

    }
}
