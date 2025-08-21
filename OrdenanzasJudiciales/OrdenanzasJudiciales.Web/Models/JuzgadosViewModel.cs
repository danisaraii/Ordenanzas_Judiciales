using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OrdenanzasJudiciales.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrdenanzasJudiciales.Web.Models
{
    public class JuzgadosViewModel
    {
        public List<catalogosModel> ListaJuzgados { get; set; } = new();
        public JuzgadoCrearDto NuevoJuzgado { get; set; }
        public FuncionarioCrearDto NuevoFuncionario { get; set; }
        public FuncionarioEditarDto EditarFuncionario { get; set; }
        public List<SelectListItem> ListaJuzgadosSelect { get; set; }
    }
}