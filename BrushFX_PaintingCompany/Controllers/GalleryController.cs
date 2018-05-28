using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrushFX_PaintingCompany.Controllers
{
    public class GalleryController : Controller
    {
        // GET: Gallery
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Interior()
        {
            return View();
        }

        public ActionResult Exterior()
        {
            return View();
        }

        public ActionResult Videos()
        {
            return View();
        }

        public ActionResult Cabinets()
        {
            return View();
        }
    }
}