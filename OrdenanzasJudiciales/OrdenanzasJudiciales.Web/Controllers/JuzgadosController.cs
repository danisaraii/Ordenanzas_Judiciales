using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdenanzasJudiciales.Aplicacion.Interfaces;
using OrdenanzasJudiciales.Dominio.Entidades;
using OrdenanzasJudiciales.Infraestructura.Data.Juzgados;

namespace OrdenanzasJudiciales.Web.Controllers
{
    public class JuzgadosController : Controller
    {
        private readonly IServicioJuzgados _servicio;

        public JuzgadosController(IServicioJuzgados servicio)
        {
            _servicio = servicio;
        }
        public async Task<IActionResult> Index()
        {   
            string procedimientoNombre = "LeerJuzgadosSecretarios";
            var juzgados = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

            var model = new JuzgadosViewModel
            {
                ListaJuzgados = juzgados,
                NuevoJuzgado = new JuzgadoCrearDto(),

                ListaJuzgadosSelect = juzgados
                .GroupBy(j => new { j.juzgadoId, j.nombreJuzgado })
                .Select(g => g.First())
                .Select(j => new SelectListItem
                {
                    Value = j.juzgadoId.ToString(),
                    Text = j.nombreJuzgado
                })
                .ToList()
            };

            return View(model);
        }
        public async Task<IActionResult> AgregarJuzgado(JuzgadoCrearDto model)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "LeerJuzgadosSecretarios";
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

                return View("Index", new JuzgadosViewModel
                {
                    ListaJuzgados = lista,
                    NuevoJuzgado = model,
                    ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync()
                });
            }

            try
            {
                string nombreProcedimiento = "AgregarJuzgados";
                string usuario = "daniela"; // Puedes usar User.Identity?.Name si tienes autenticación

                var parametros = new Dictionary<string, object>
                {
                    { "@usuario", usuario },
                    { "@nombre", model.juzgadoNombre },
                    { "@alias", model.nombreAlias },
                    { "@direccion", model.direccionJuzgado },
                    { "@ciudad", model.ciudadJuzgado },
                    { "@nombreSecretario", model.funcionarioJuez },
                    { "@correo", model.email }
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
        public async Task<IActionResult> AgregarFuncionario(FuncionarioCrearDto model)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "LeerJuzgadosSecretarios";
                var juzgados = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                return View("Index", new JuzgadosViewModel
                {
                    ListaJuzgados = juzgados,
                    ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync(),
                    NuevoFuncionario = model
                });

            }

            try
            {
                string nombreProcedimiento = "AgregarFuncionario";
                string usuario = "daniela"; // Usar User.Identity?.Name cuando haya autenticación

                var parametros = new Dictionary<string, object>
                {
                    { "@usuario", usuario },
                    { "@idJuzgado", model.JuzgadoId},                   
                    { "@nombreSecretario", model.funcionarioNombre },
                    { "@correo", model.emailFuncionario }
                };

                // Ejecutar SP y obtener resultado
                var (resultado, codigo) = await _servicio.InsertarFuncionarioAsync(nombreProcedimiento, parametros);

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
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error inesperado: {ex.Message}";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }
        private async Task<List<SelectListItem>> ObtenerJuzgadosSelectAsync()
        {
            string procedimientoNombre = "LeerJuzgadosSecretarios";
            var juzgados = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

            return juzgados
                .GroupBy(j => new { j.juzgadoId, j.nombreJuzgado })
                .Select(g => g.First())
                .Select(j => new SelectListItem
                {
                    Value = j.juzgadoId.ToString(),
                    Text = j.nombreJuzgado
                })
                .ToList();
        }
        [HttpPost]
        public async Task<IActionResult> EditarFuncionario(FuncionarioEditarDto model)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "LeerJuzgadosSecretarios";
                // Reenviar datos al Index si hay error
                var lista = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);
                return View("Index", new JuzgadosViewModel
                {
                    ListaJuzgados = lista,
                    ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync()
                    
                });
            }

            string nombreProcedimiento = "ActualizarFuncionario";
            string usuario = "daniela"; // Usar User.Identity?.Name cuando haya autenticación
            var parametros = new Dictionary<string, object>
            {
                { "@idSecretario", model.FuncionarioId },
                { "@idJuzgado", model.JuzgadoId },
                { "@nuevoNombreSecretario", model.NombreFuncionario },
                { "@nuevoCorreo", model.Correo },
                { "@usuario", usuario   },
                { "@alias", model.Alias },
                { "@direccion", model.DireccionJuzgado }
            };

            var (resultado, codigo) = await _servicio.InsertarFuncionarioAsync(nombreProcedimiento, parametros);
            TempData["Mensaje"] = resultado;
            TempData["TipoMensaje"] = (codigo != 0) ? "error" : "success";

            return Json(new { exito = codigo == 0, mensaje = resultado });
            //return RedirectToAction("Index");
        }

        public async Task<IActionResult> EliminarFuncionario(int juzgadoId, int funcionarioId)
        {
            if (!ModelState.IsValid)
            {
                string procedimientoNombre = "LeerJuzgadosSecretarios";
                var juzgados = await _servicio.LeerJuzgadosSecretariosAsync(procedimientoNombre);

                return View("Index", new JuzgadosViewModel
                {
                    ListaJuzgados = juzgados,
                    ListaJuzgadosSelect = await ObtenerJuzgadosSelectAsync()
                });

            }

            string nombreProcedimiento = "EliminarFuncionario";
            string usuario = "daniela"; // Usar User.Identity?.Name cuando haya autenticación
            var parametros = new Dictionary<string, object>
            {
                { "@idFuncionario", funcionarioId },
                { "@idJuzgado", juzgadoId},
                { "@usuario", usuario   }
            };

            var (resultado, codigo) = await _servicio.InsertarFuncionarioAsync(nombreProcedimiento, parametros);
    
            return Json(new { exito = codigo == 0, mensaje = resultado });
            //return RedirectToAction("Index");
        }
    
    }
}
