using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Infraestructura.Data.Juzgados;

namespace OrdenanzasJudiciales.Web.Controllers
{
    public class CuentasBancariasController : Controller
    {
        private readonly IServicioJuzgados _servicio;
        public CuentasBancariasController(IServicioJuzgados servicio)
        {
            _servicio = servicio;
        }
        public async Task<IActionResult> Index()
        {
            string procedimientoNombre = "ObtenerCuentasActivas";
            string procedimientoEntidad = "ListarEntidadesBancariasActivas";
            string procedimientoAcreedor = "ListarAcreedoresActivos";
            var entidad = await _servicio.EjecutarSPGenericoAsync(procedimientoEntidad);
            var acreedor = await _servicio.EjecutarSPGenericoAsync(procedimientoAcreedor);
            var cuentas = await _servicio.LeerCuentasAsync(procedimientoNombre);
            var model = new CuentasViewModel
            {
                ListaCuentas = cuentas,
                TablaEntidad = entidad,
                TablaAcreedor = acreedor
            };
            return View(model);
            //return View();
        }
        public async Task<IActionResult> AgregarAcreedor(CuentasCrearDto model)
    {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "ObtenerCuentasActivas";
                var lista = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errores = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { exito = false, errores });
                }

                return View("Index", new CuentasViewModel
                {
                    //ListaCuentas = lista,
                    //NuevaCuenta = model,
                    //ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync()
                });
            }
            try
            {
                string nombreProcedimiento = "AgregarAcreedor";
                string usuario = "daniela"; // Usar User.Identity?.Name
                var parametros = new Dictionary<string, object>
                {
                    { "@usuario", usuario },
                    { "@NombreAcreedor", model.nombreAcreedor },
                    { "@RUC_Cedula", model.rucCedula },
                    { "@TipoIdentificacion", model.tipoIdentificacion }
                };

                // Ejecutar SP y obtener resultado
                var (resultado, codigo) = await _servicio.InsertarJuzgadoAsync(nombreProcedimiento, parametros);

                TempData["Mensaje"] = resultado;
                if (codigo != 0)
                {
                    TempData["TipoMensaje"] = "error";
                }
                else
                {
                    TempData["TipoMensaje"] = "success";
                }
                return Json(new { exito = codigo == 0, mensaje = resultado });

                // return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error inesperado: {ex.Message}";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> AgregarEntidad(CuentasEntidadCrearDto model)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "ObtenerCuentasActivas";
                var lista = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errores = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { exito = false, errores });
                }

                return View("Index", new CuentasViewModel
                {
                    //ListaCuentas = lista,
                    //NuevaCuenta = model,
                    //ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync()
                });
            }
            try
            {
                string nombreProcedimiento = "AgregarEntidadBancaria";
                string usuario = "daniela"; // Usar User.Identity?.Name
                var parametros = new Dictionary<string, object>
                {
                    { "@usuario", usuario },
                    { "@NombreEntidad", model.nombreEntidad }
                };

                // Ejecutar SP y obtener resultado
                var (resultado, codigo) = await _servicio.InsertarJuzgadoAsync(nombreProcedimiento, parametros);

                TempData["Mensaje"] = resultado;
                if (codigo != 0)
                {
                    TempData["TipoMensaje"] = "error";
                }
                else
                {
                    TempData["TipoMensaje"] = "success";
                }
                return Json(new { exito = codigo == 0, mensaje = resultado });

                // return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error inesperado: {ex.Message}";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> AgregarCuentaBancaria(CuentasAgregar model)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { exito = false, mensaje = "Datos inválidos.", errores });
            }
            try
            {
                string nombreProcedimiento = "AgregarCuentaBancaria";
                string usuario = "daniela"; // Usar User.Identity?.Name
                var parametros = new Dictionary<string, object>
                {
                    { "@usuario", usuario },
                    { "@NumeroCuenta", model.numeroCuenta },
                    { "@TipoCuenta", model.tipoCuenta },
                    { "@IdAcreedor", model.idAcreedor },
                    { "@IdEntidadBancaria", model.idEntidadBancaria }
                };

                // Ejecutar SP y obtener resultado
                var (resultado, codigo) = await _servicio.InsertarJuzgadoAsync(nombreProcedimiento, parametros);
                return Json(new { exito = codigo == 0, mensaje = resultado });


                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error inesperado: {ex.Message}";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarCuenta(CuentasEditarDto model)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "ObtenerCuentasActivas";
                var lista = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errores = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    return Json(new { exito = false, errores });
                }
                return View("Index", new CuentasViewModel{ });
            }

            string nombreProcedimiento = "ActualizarCuentaBancaria";
            string usuario = "daniela"; // Usar User.Identity?.Name cuando haya autenticación
            var parametros = new Dictionary<string, object>
            {
                { "@IdCuenta", model.IdCuenta },
                { "@NumeroCuenta", model.numeroCuenta },
                { "@TipoCuenta", model.tipoCuenta },
                { "@usuario", usuario   }
            };

            var (resultado, codigo) = await _servicio.InsertarFuncionarioAsync(nombreProcedimiento, parametros);
            TempData["Mensaje"] = resultado;
            TempData["TipoMensaje"] = (codigo != 0) ? "error" : "success";

            return Json(new { exito = codigo == 0, mensaje = resultado });
        }
        public async Task<IActionResult> EliminarCuentaBancaria(int idCuenta)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "ObtenerCuentasActivas";
                var lista = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errores = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { exito = false, errores });
                }
            }

            string nombreProcedimiento = "EliminarCuentaBancaria";
            string usuario = "daniela"; // Usar User.Identity?.Name cuando haya autenticación
            var parametros = new Dictionary<string, object>
            {
                { "@idCuenta", idCuenta },
                { "@usuario", usuario   }
            };
            var (resultado, codigo) = await _servicio.InsertarFuncionarioAsync(nombreProcedimiento, parametros);
            return Json(new { exito = codigo == 0, mensaje = resultado });
        }
    }
}
