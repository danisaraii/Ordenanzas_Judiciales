using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public IActionResult CargarDatos()
        {
            return View();
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
                    if (idProceso == 1) 
                    {
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
                                { "@nombre", nombre }, { "@identificacion", identificacion },
                                { "@tipo", tipo }, { "@cuenta", cuenta },
                                { "@fecharetencion", fecha }, { "@juicio", juicio },
                                { "@valor", monto }, { "@juzgado", juzgado },
                                { "@oficioretencion", oficioretencion }, { "@tramite", tramite },
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
                        await _proceso.EjecutarProcedimientoAsync("limpiarSumDevolucion");

                        for (int fila = 2; fila <= totalFilas; fila++)
                        {
                            string orden = hoja.Cell(fila, 1).GetString();
                            string nombre = hoja.Cell(fila, 2).GetString();
                            string identificacion = hoja.Cell(fila, 3).GetString();
                            string tipo = hoja.Cell(fila, 4).GetString();
                            string cuenta = hoja.Cell(fila, 5).GetString();
                            string fechaTexto = hoja.Cell(fila, 6).GetString();
                            string juicio = hoja.Cell(fila, 7).GetString();
                            string montoTexto = hoja.Cell(fila, 8).GetString();
                            string juzgado = hoja.Cell(fila, 9).GetString();
                            string oficioretencion = hoja.Cell(fila, 10).GetString();
                            string tramite = hoja.Cell(fila, 11).GetString();
                            string usuario = hoja.Cell(fila, 12).GetString();
                            string tramitedevolucion = hoja.Cell(fila, 13).GetString();
                            string oficiodevolucion = hoja.Cell(fila, 14).GetString();
                            string fechadevolucion = hoja.Cell(fila, 15).GetString();
                            string usuariodevolucion = hoja.Cell(fila, 16).GetString();

                            if (DateTime.TryParse(fechaTexto, out DateTime fecha) &&
                                float.TryParse(montoTexto, NumberStyles.Float, CultureInfo.InvariantCulture, out float monto) &&
                                DateTime.TryParse(fechadevolucion, out DateTime fecha_devolucion))
                            {
                                var parametros = new Dictionary<string, object>
                            {
                                {"@orden", orden}, { "@nombre", nombre },
                                { "@identificacion", identificacion }, { "@tipo", tipo },
                                { "@cuenta", cuenta }, { "@fecharetencion", fecha },
                                { "@juicio", juicio }, { "@valor", monto },
                                { "@juzgado", juzgado }, { "@oficioretencion", oficioretencion },
                                { "@tramite", tramite }, { "@usuario", usuario },
                                { "@tramitedevolucion", tramitedevolucion }, { "@oficiodevolucion", oficiodevolucion },
                                { "@fechadevolucion", fecha_devolucion }, { "@usuariodevolucion", usuariodevolucion }
                            };

                                await _proceso.InsertarDatosAsync("InsertarDatosDevoluciones", parametros);
                                registrosProcesados++;
                            }
                        }
                    }
                    else if (idProceso == 3)
                    {
                        //Embargos Cheques
                        await _proceso.EjecutarProcedimientoAsync("limpiarSumEmbargo");

                        for (int fila = 2; fila <= totalFilas; fila++)
                        {
                            string orden = hoja.Cell(fila, 1).GetString();
                            string nombre = hoja.Cell(fila, 2).GetString();
                            string identificacion = hoja.Cell(fila, 3).GetString();
                            string tipo = hoja.Cell(fila, 4).GetString();
                            string cuenta = hoja.Cell(fila, 5).GetString();
                            string fechaTexto = hoja.Cell(fila, 6).GetString();
                            string juicio = hoja.Cell(fila, 7).GetString();
                            string montoTexto = hoja.Cell(fila, 8).GetString();
                            string juzgado = hoja.Cell(fila, 9).GetString();
                            string oficioretencion = hoja.Cell(fila, 10).GetString();
                            string tramite = hoja.Cell(fila, 11).GetString();
                            string usuario = hoja.Cell(fila, 12).GetString();
                            string tramiteembargo = hoja.Cell(fila, 13).GetString();
                            string oficioembargo = hoja.Cell(fila, 14).GetString();
                            string fechaembargo = hoja.Cell(fila, 15).GetString();
                            string usuarioembargo = hoja.Cell(fila, 16).GetString();
                            string oficial = hoja.Cell(fila, 16).GetString();

                            if (DateTime.TryParse(fechaTexto, out DateTime fecha) &&
                                float.TryParse(montoTexto, NumberStyles.Float, CultureInfo.InvariantCulture, out float monto) &&
                                DateTime.TryParse(fechaembargo, out DateTime fecha_embargo))
                            {
                                var parametros = new Dictionary<string, object>
                            {
                                {"@orden", orden}, { "@nombre", nombre },
                                { "@identificacion", identificacion }, { "@tipo", tipo },
                                { "@cuenta", cuenta }, { "@fecharetencion", fecha },
                                { "@juicio", juicio }, { "@valor", monto },
                                { "@juzgado", juzgado }, { "@oficioretencion", oficioretencion },
                                { "@tramite", tramite }, { "@usuario", usuario },
                                { "@tramiteembargo", tramiteembargo }, { "@oficioembargo", oficioembargo },
                                { "@fechaembargo", fecha_embargo }, { "@usuarioembargo", usuarioembargo },
                                { "@oficial", usuarioembargo }
                            };

                                await _proceso.InsertarDatosAsync("InsertarDatosEmbargos", parametros);
                                registrosProcesados++;
                            }
                        }
                    }
                }
            }

            return Ok(new
            {
                mensaje = "Archivo procesado correctamente.",
                registros = registrosProcesados
            });
        }
       
        [HttpPost]
        public async Task<IActionResult> EjecutarConsulta([FromBody] procesosOrdenanzas model)
        {
            string usuario = "daniela"; //modificar para tomar el usuario logueado
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
            //return Json(new
            //{
            //    datos = lista,
            //    error = resultado.CodigoError,
            //    mensaje = resultado.Mensaje
            //});

            if (resultado.CodigoError == 0)
            {
                TempData["mensaje"] = resultado.Mensaje;
                //TempData["datos"] = JsonConvert.SerializeObject(lista);
                TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(lista);
                TempData["idProceso"] = proceso;
                return Json(new
                {
                    //success = true,
                    //redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas")
                    error = 0,
                    mensaje = resultado.Mensaje,
                    redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas", new { id = model.idCargaArchivo })
                });
            }
            else
            {
                return Json(new
                {
                    //success = false,
                    //mensaje = resultado.Mensaje

                    error = resultado.CodigoError,
                    mensaje = resultado.Mensaje

                });
            }

        }

        
    }
}