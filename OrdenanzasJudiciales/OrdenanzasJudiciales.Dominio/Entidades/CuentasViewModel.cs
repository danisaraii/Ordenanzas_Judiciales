using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class CuentasViewModel
    {
        public List<CuentasModel> ListaCuentas { get; set; } = new();
        public List<CuentasModel> ListaEntidad { get; set; } = new();
        public CuentasCrearDto NuevaCuenta { get; set; }
        public CuentasEntidadCrearDto NuevaEntidad { get; set; }
        public CuentasEditarDto EditarCuenta { get; set; }
        public DataTable TablaEntidad { get; set; }
        public DataTable TablaAcreedor { get; set; }
        public CuentasAgregar AgregarCuenta { get; set; }
       
    }
}
