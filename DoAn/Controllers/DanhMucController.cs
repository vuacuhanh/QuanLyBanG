using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: DanhMuc
        QLBGDataContext db = new QLBGDataContext();
        public ActionResult DanhMucPar()
        {
            var listCD = db.DANHMUCs.Take(7).ToList();
            return View(listCD);
        }

    }
}