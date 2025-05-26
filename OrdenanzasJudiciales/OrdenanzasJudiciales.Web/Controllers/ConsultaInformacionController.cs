using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Dominio.Entidades;
using System.Globalization;

namespace OrdenanzasJudiciales.Web.Controllers
{
    public class ConsultaInformacionController : Controller
    {
        private readonly IProcesosOrdenanzasRepositorio _proceso;

        public ConsultaInformacionController(IProcesosOrdenanzasRepositorio proceso)
        {
            _proceso = proceso;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
