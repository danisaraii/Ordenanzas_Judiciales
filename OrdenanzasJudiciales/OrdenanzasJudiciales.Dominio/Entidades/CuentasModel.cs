using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasModel
    {
        public int idCuenta { get; set; }
        public int idAcreedor { get; set; }
        public int idEntidad { get; set; }
        public string numeroCuenta { get; set; }
        public string tipoCuenta { get; set; }
        public string nombreAcreedor { get; set; }
        public string rucCedula { get; set; }
        public string nombreEntidad { get; set; }
    }
}