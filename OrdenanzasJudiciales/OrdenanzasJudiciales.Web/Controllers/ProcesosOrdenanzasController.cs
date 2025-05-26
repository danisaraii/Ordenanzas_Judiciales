using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Infraestructura.Data.Repositorios;
using System.Globalization; 

namespace OrdenanzasJudiciales.Web.Controllers
{
    public class ProcesosOrdenanzasController : Controller
    {
        private readonly IProcesosOrdenanzasRepositorio _proceso;

        public ProcesosOrdenanzasController(IProcesosOrdenanzasRepositorio proceso)
        {
            _proceso = proceso;
        }

        public async Task<IActionResult> Index()
        {
            var datos = await _proceso.ObtenerReporteAsync();
            return View(datos);
        }

        [HttpPost]
        public async Task<IActionResult> ContarRegistros(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo inválido");

            int cantidad = 0;

            using (var stream = archivo.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                        cantidad++;
                }
            }

            return Json(new { registros = cantidad });
        }

        [HttpPost]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo, int idProceso)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { mensaje = "Archivo inválido." });

            int registrosProcesados = 0;

            using (var stream = new MemoryStream())
            {
                await archivo.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var hoja = workbook.Worksheet(1);
                    var totalFilas = hoja.LastRowUsed().RowNumber();

                    if (idProceso == 1) {
                        //Retenciones
                        await _proceso.EjecutarProcedimientoAsync("limpiarSumRetencion");

                        for (int fila = 2; fila <= totalFilas; fila++)
                        {
                            string nombre = hoja.Cell(fila, 1).GetString();
                            string identificacion = hoja.Cell(fila, 2).GetString();
                            string tipo = hoja.Cell(fila, 3).GetString();
                            string cuenta = hoja.Cell(fila, 4).GetString();
                            string fechaTexto = hoja.Cell(fila, 5).GetString();
                            string juicio = hoja.Cell(fila, 6).GetString();
                            string montoTexto = hoja.Cell(fila, 7).GetString();
                            string juzgado = hoja.Cell(fila, 8).GetString();
                            string oficioretencion = hoja.Cell(fila, 9).GetString();
                            string tramite = hoja.Cell(fila, 10).GetString();
                            string usuario = hoja.Cell(fila, 11).GetString();

                            if (DateTime.TryParse(fechaTexto, out DateTime fecha) &&
                                float.TryParse(montoTexto, NumberStyles.Float, CultureInfo.InvariantCulture, out float monto))
                            {
                                var parametros = new Dictionary<string, object>
                        {
                            { "@nombre", nombre },
                            { "@identificacion", identificacion },
                            { "@tipo", tipo },
                            { "@cuenta", cuenta },
                            { "@fecharetencion", fecha },
                            { "@juicio", juicio },
                            { "@valor", monto },
                            { "@juzgado", juzgado },
                            { "@oficioretencion", oficioretencion },
                            { "@tramite", tramite },
                            { "@usuario", usuario }
                        };

                            await _proceso.InsertarDatosAsync("InsertarDatosRetenciones", parametros);
                            registrosProcesados++;
                        }
                    }
                    }
                    else if(idProceso == 2)
                    {
                    //Devoluciones
                    }
                }
            }

            return Ok(new
            {
                mensaje = "Archivo procesado correctamente.",
                registros = registrosProcesados
            });
        }

        //[HttpPost]
        //public async Task<IActionResult> EjecutarConsulta([FromBody] cargaArchivo model)
        //{
        //    string mensaje = "";
        //    int error = 0;
        //    var parametros = new Dictionary<string, object>
        //    {
        //        { "@xcont", model.fkIdProceso },
        //        { "@idCargaArchivo", model.idCargaArchivo }
        //    };
        //    var resultado = await _proceso.EjecutarResultadoAsync("AgregaRetenciones", parametros, out error, out mensaje);

        //    var lista = new List<Dictionary<string, object>>();
        //    foreach (System.Data.DataRow row in resultado.Rows)
        //    {
        //        var dict = new Dictionary<string, object>();
        //        foreach (System.Data.DataColumn col in resultado.Columns)
        //        {
        //            dict[col.ColumnName] = row[col];
        //        }
        //        lista.Add(dict);
        //    }

        //    return Json(lista);
        //}

        [HttpPost]
        public async Task<IActionResult> EjecutarConsulta([FromBody] procesosOrdenanzas model)
        {
            string usuario = "daniela";
            int proceso = model.idCargaArchivo;
            var parametros = new Dictionary<string, object>
            {
                //{ "@usuario", model.nombre }
                { "@usuario", usuario },
                { "@moneda","USD" }
            };

            var resultado = await _proceso.EjecutarResultadoAsync("AgregaRetenciones", parametros);

            var lista = new List<Dictionary<string, object>>();
            foreach (System.Data.DataRow row in resultado.Datos.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in resultado.Datos.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                lista.Add(dict);
            }

            return Json(new
            {
                datos = lista,
                error = resultado.CodigoError,
                mensaje = resultado.Mensaje
            });
        }


    }
}
