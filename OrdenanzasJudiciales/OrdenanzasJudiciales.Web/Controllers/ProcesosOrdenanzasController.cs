using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Infraestructura.Data.Repositorios;
using OrdenanzasJudiciales.Web.Models;
using System.Diagnostics;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            string usuario = "daniela"; //Reemplazar con usuario autenticado
            string procedimiento = "cargarArchivosPorProceso";
            var datos = await _proceso.ObtenerReporteAsync(procedimiento);
            
            //proceso de saldo diario del portafolio
            var parametros = new Dictionary<string, object>
                    {{ "@usuario", usuario }};
            var resultado = await _proceso.EjecutarResultadoAsync(
                "ConsultaProcesoDia", parametros);
            var lista = new List<Dictionary<string, object>>();
            foreach (System.Data.DataRow row in resultado.Datos.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in resultado.Datos.Columns)
                { dict[col.ColumnName] = row[col]; }
                lista.Add(dict);
            }
            var vm = new ReporteViewModel
            {
                DatosPrincipales = datos,
                DatosConsultaDia = lista
            };
            return View(vm);
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

            Dictionary<int, List<string>> columnasPorProceso = new()
            {
                { 1, new List<string> { "NOMBRE", "IDENTIFICACION", "TIPO", "CUENTA",
                                        "FECHARETENCION", "JUICIO", "VALOR", "JUZGADO",
                                        "NOOFICIORETENCION", "NOTRAMITE", "USUARIORETENCION" } },
                { 2, new List<string> { "ORDEN", "NOMBRE", "IDENTIFICACION", "TIPO", 
                                        "CUENTA", "FECHARETENCION", "JUICIO", "VALOR", 
                                        "JUZGADO", "NOOFICIORETENCION", "NOTRAMITE", "USUARIORETENCION",
                                        "NOTRAMITEDEVOLUCION", "NOOFICIODEVOLUCION", "FECHAPROCESO", 
                                        "USUARIOPROCESO" } },
                { 3, new List<string> { "ORDEN", "NOMBRE", "IDENTIFICACION", "TIPO", 
                                        "CUENTA", "FECHARETENCION", "JUICIO", "VALOR", 
                                        "JUZGADO", "NOOFICIORETENCION", "NOTRAMITE", "USUARIORETENCION",
                                        "NOTRAMITEEMBARGO", "NOOFICIOEMBARGO", "FECHAEMBARGO", "USUARIOPROCESO",
                                        "OFICIAL" } },
                { 4, new List<string> { "ORDEN", "NOMBRE", "IDENTIFICACION", "TIPO", 
                                        "CUENTA", "FECHARETENCION", "JUICIO", "VALOR", 
                                        "JUZGADO", "NOOFICIORETENCION", "NOTRAMITE", "USUARIORETENCION",
                                        "NOTRAMITETRANSFERENCIA", "NOOFICIOTRANSFERENCIA", "FECHATRANSFERENCIA",
                                        "USUARIOPROCESO", "CUENTADESTINO", "BANCODESTINO", "OFICIAL","OBSERVACION" } }
            };

            if (!columnasPorProceso.ContainsKey(idProceso))
                return BadRequest(new { mensaje = "Proceso no reconocido." });

            var columnasEsperadas = columnasPorProceso[idProceso];
            using var stream = new MemoryStream();
            await archivo.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var hoja = workbook.Worksheet(1);
            var encabezados = hoja.Row(1).Cells().Select(c => c.GetString().Trim().ToUpper()).ToList();

            //var encabezados = hoja.Row(1).Cells()
            //         .Select(c => c.GetString().Trim().ToUpper().Replace("\u200B", ""))
            //         .ToList();
            
            bool columnasValidas = columnasEsperadas.All(c => encabezados.Contains(c));
            if (!columnasValidas)
                return BadRequest(new { mensaje = "El archivo no tiene el formato esperado. Verifique las columnas requeridas." });

            int registrosProcesados = 0;
            int totalFilas = hoja.LastRowUsed().RowNumber();

            switch (idProceso)
            {
                case 1:
                    await _proceso.EjecutarProcedimientoAsync("limpiarSumRetencion");
                    break;
                case 2:
                    await _proceso.EjecutarProcedimientoAsync("limpiarSumDevolucion");
                    break;
                case 3:
                    await _proceso.EjecutarProcedimientoAsync("limpiarSumEmbargo");
                    break; 
                case 4:
                    await _proceso.EjecutarProcedimientoAsync("limpiarSumTransferencia");
                    break;
                case 7:
                    await _proceso.EjecutarProcedimientoAsync("limpiarSumDevolucionProcesada");
                    break;
            }
                //if (idProceso == 1)
                //{await _proceso.EjecutarProcedimientoAsync("limpiarSumRetencion");}
                //else if (idProceso == 2)
                //{await _proceso.EjecutarProcedimientoAsync("limpiarSumDevolucion");}
                //else if (idProceso == 3)
                //{ await _proceso.EjecutarProcedimientoAsync("limpiarSumEmbargo"); }

            for (int i = 2; i <= totalFilas; i++)
            {
                var fila = hoja.Row(i);
                var parametros = new Dictionary<string, object>();
                foreach (var columna in columnasEsperadas)
                {
                    var celda = fila.Cell(encabezados.IndexOf(columna) + 1);
                    string valor = celda.GetValue<string>().Trim();
                    object valorTransformado = valor;

                    if (columna.StartsWith("FECHA", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!DateTime.TryParse(valor, out DateTime fecha))
                        {
                            return BadRequest(new { mensaje = $"Fila {i}: La columna {columna} tiene un formato de fecha inválido." });
                        }
                        else
                        {
                            valorTransformado = fecha.ToString("yyyy-MM-dd");
                        }
                    }
                    else if (columna == "VALOR")
                    {
                        if (!float.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out float monto))
                        {
                            return BadRequest(new { mensaje = $"Fila {i}: La columna {columna} tiene un valor numérico inválido." });
                        }
                        else
                        {
                            valorTransformado = monto;
                        }
                    }
                    parametros.Add("@" + columna, valorTransformado);
                }
                string nombreSP = idProceso switch
                {
                    1 => "InsertarDatosRetenciones",
                    2 => "InsertarDatosDevoluciones",
                    3 => "InsertarDatosEmbargos",
                    4 => "InsertarDatosTransferencias",
                    7 => "InsertarDatosDevolucionesProcesadas",
                    _ => throw new Exception("Proceso no válido")
                };
                await _proceso.InsertarDatosAsync(nombreSP, parametros);
                registrosProcesados++;
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
            string usuario = "daniela"; //Reemplazar con usuario autenticado
            int proceso = model.idCargaArchivo;
            var parametrosUsu = new Dictionary<string, object>
                    {{ "@usuario", usuario }};
            switch (proceso)
            {
                case 1:
                    var parametros = new Dictionary<string, object>
                    {
                        { "@usuario", usuario },
                        { "@moneda", "USD" }
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

                    if (resultado.CodigoError == 0)
                    {
                        TempData["mensaje"] = resultado.Mensaje;
                        TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(lista);
                        TempData["idProceso"] = proceso;

                        return Json(new
                        {
                            error = 0,
                            mensaje = resultado.Mensaje,
                            redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas", new { id = model.idCargaArchivo })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            error = resultado.CodigoError,
                            mensaje = resultado.Mensaje
                        });
                    }
                case 2:
                    var resultadoDev = await _proceso.EjecutarResultadoAsync("BuscaRetencionesDevolucion", 
                        parametrosUsu);
                    var listaDev = new List<Dictionary<string, object>>();
                    if (resultadoDev.CodigoError == 0)
                    {
                        var resultadoReg = await _proceso.EjecutarResultadoAsync("RegistraDevolucion",
                            parametrosUsu);
                        var resultadoBuscar = await _proceso.EjecutarResultadoAsync("BuscaDevolucionesPendientes",
                            parametrosUsu);
                        var listaBuscar = new List<Dictionary<string, object>>();
                        if (resultadoBuscar.CodigoError == 0)
                        {
                            foreach (System.Data.DataRow row in resultadoBuscar.Datos.Rows)
                            {
                                var dict = new Dictionary<string, object>();
                                foreach (System.Data.DataColumn col in resultadoBuscar.Datos.Columns)
                                {
                                    dict[col.ColumnName] = row[col];
                                }
                                listaBuscar.Add(dict);
                            }
                            TempData["mensaje"] = resultadoBuscar.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscar);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 0,
                                mensaje = resultadoBuscar.Mensaje,
                                redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }
                        else
                        {
                            TempData["mensaje"] = resultadoBuscar.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscar);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 1,
                                mensaje = resultadoBuscar.Mensaje,
                                redirectUrl = Url.Action("ValidarData", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }                        
                    }
                    else
                    {
                        return Json(new
                        {
                            error = resultadoDev.CodigoError,
                            mensaje = resultadoDev.Mensaje
                        });
                    }
                case 3:
                    var resultadoEmb = await _proceso.EjecutarResultadoAsync("BuscaRetencionesEmbargo", 
                        parametrosUsu);
                    var listaEmb = new List<Dictionary<string, object>>();
                    if (resultadoEmb.CodigoError == 0)
                    {
                        var resultadoRegistroEmbargo = await _proceso.EjecutarResultadoAsync("RegistraEmbargo",
                            parametrosUsu);
                        var resultadoBuscarEmbargo = await _proceso.EjecutarResultadoAsync("BuscaEmbargosPendientes",
                            parametrosUsu);
                        var listaBuscarEmbargo = new List<Dictionary<string, object>>();
                        if (resultadoBuscarEmbargo.CodigoError == 0)
                        {
                            foreach (System.Data.DataRow row in resultadoBuscarEmbargo.Datos.Rows)
                            {
                                var dict = new Dictionary<string, object>();
                                foreach (System.Data.DataColumn col in resultadoBuscarEmbargo.Datos.Columns)
                                {
                                    dict[col.ColumnName] = row[col];
                                }
                                listaBuscarEmbargo.Add(dict);
                            }
                            TempData["mensaje"] = resultadoBuscarEmbargo.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscarEmbargo);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 0,
                                mensaje = resultadoBuscarEmbargo.Mensaje,
                                redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }
                        else
                        {
                            TempData["mensaje"] = resultadoBuscarEmbargo.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscarEmbargo);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 1,
                                mensaje = resultadoBuscarEmbargo.Mensaje,
                                redirectUrl = Url.Action("ValidarData", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }                        
                    }
                    else
                    {
                        return Json(new
                        {
                            error = resultadoEmb.CodigoError,
                            mensaje = resultadoEmb.Mensaje
                        });
                    }               
                case 4:
                    var resultadoTra = await _proceso.EjecutarResultadoAsync("BuscaRetencionesTransferencia", 
                        parametrosUsu);
                    var listaTra = new List<Dictionary<string, object>>();
                    if (resultadoTra.CodigoError == 0)
                    {
                        var resultadoRegistroTra = await _proceso.EjecutarResultadoAsync("RegistraTransferencia",
                            parametrosUsu);
                        var resultadoBuscarTra = await _proceso.EjecutarResultadoAsync("BuscaTransferenciasPendientes",
                            parametrosUsu);
                        var listaBuscarTra = new List<Dictionary<string, object>>();
                        if (resultadoBuscarTra.CodigoError == 0)
                        {
                            foreach (System.Data.DataRow row in resultadoBuscarTra.Datos.Rows)
                            {
                                var dict = new Dictionary<string, object>();
                                foreach (System.Data.DataColumn col in resultadoBuscarTra.Datos.Columns)
                                {
                                    dict[col.ColumnName] = row[col];
                                }
                                listaBuscarTra.Add(dict);
                            }
                            TempData["mensaje"] = resultadoBuscarTra.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscarTra);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 0,
                                mensaje = resultadoBuscarTra.Mensaje,
                                redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }
                        else
                        {
                            TempData["mensaje"] = resultadoBuscarTra.Mensaje;
                            TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaBuscarTra);
                            TempData["idProceso"] = proceso;
                            return Json(new
                            {
                                error = 1,
                                mensaje = resultadoBuscarTra.Mensaje,
                                redirectUrl = Url.Action("ValidarData", "ProcesosOrdenanzas",
                                new { id = model.idCargaArchivo })
                            });
                        }                        
                    }
                    else
                    {
                        return Json(new
                        {
                            error = resultadoTra.CodigoError,
                            mensaje = resultadoTra.Mensaje
                        });
                    }
                case 7:
                    //Validar devoluciones que fueron procesadas con exito.
                    var validarDevolucion = await _proceso.EjecutarResultadoAsync("ValidaEjecucionDevolucion",
                            parametrosUsu);
                    var listaValidacionDevolucion = new List<Dictionary<string, object>>();
                    if (validarDevolucion.CodigoError == 0)
                    {
                        foreach (System.Data.DataRow row in validarDevolucion.Datos.Rows)
                        {
                            var dict = new Dictionary<string, object>();
                            foreach (System.Data.DataColumn col in validarDevolucion.Datos.Columns)
                            {
                                dict[col.ColumnName] = row[col];
                            }
                            listaValidacionDevolucion.Add(dict);
                        }
                        TempData["mensaje"] = validarDevolucion.Mensaje;
                        TempData["lista"] = Newtonsoft.Json.JsonConvert.SerializeObject(listaValidacionDevolucion);
                        TempData["idProceso"] = proceso;
                        return Json(new
                        {
                            error = 0,
                            mensaje = validarDevolucion.Mensaje,
                            redirectUrl = Url.Action("CargarDatos", "ProcesosOrdenanzas",
                            new { id = model.idCargaArchivo })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            error = validarDevolucion.CodigoError,
                            mensaje = validarDevolucion.Mensaje
                        });
                    }
                case 6:


                    return Json(new { error = 1, mensaje = "Proceso aún no implementado." });
                case 8:
                    return Json(new { error = 1, mensaje = "Proceso aún no implementado." });
                default:
                    return Json(new { error = 1, mensaje = "Proceso inválido." });
            }
        }
    }
}