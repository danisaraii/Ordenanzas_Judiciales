using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdenanzasJudiciales.Dominio.Entidades
{
    public class catalogosModel
    {
        public int funcionarioId { get; set; }
        public int juzgadoId { get; set; }
        public string nombreJuzgado { get; set; }
        public string nombreBeneficiario { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public string funcionario { get; set; }
        public string correo { get; set; }
    }
}
