using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrdenanzasJudiciales.Web.Controllers
{
    public class AdministracionController : Controller
    {
        // GET: AdministracionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdministracionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdministracionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdministracionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdministracionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdministracionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdministracionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdministracionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
