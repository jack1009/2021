using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppPoriteTestingMachine.Models;
using System.Net;
using System.IO;
using System.Data.Entity;

namespace WebAppPoriteTestingMachine.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
       
        //=============================================
        #region MC910
        public ActionResult ListMc910()
        {
            using (PoriteDBModelContext _db = new PoriteDBModelContext())
            {
                return View(_db.MC910Table.ToList());
            }
        }

        public ActionResult Query910Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query910Index(MC910Table table)
        {
            string uQRCode = table.QRCode;
            string uJudgment = table.JudgmentResult;
            DateTime uDateTime;
            PoriteDBModelContext db = new PoriteDBModelContext();
            var ListAll = db.MC910Table.Select(s => s);

            if (table.TestingDateTime.HasValue)
            {
                uDateTime = new DateTime();
                uDateTime = table.TestingDateTime.Value;
                DateTime uDateTime1 = uDateTime.AddDays(1);
                if (uDateTime != null)
                {
                    ListAll = ListAll.Where(w => w.TestingDateTime.Value >= uDateTime && w.TestingDateTime<uDateTime1);
                }
            }
            if (!string.IsNullOrWhiteSpace(uQRCode))
            {
                ListAll = ListAll.Where(w => w.QRCode.Contains(uQRCode));
            }
            if (!string.IsNullOrWhiteSpace(uJudgment))
            {
                ListAll = ListAll.Where(w => w.JudgmentResult.Contains(uJudgment));
            }
           
            return View("Query910_Result",ListAll.ToList());
        }
        #endregion
        //=============================================
        #region MC909
        public ActionResult ListMc909()
        {
            using (PoriteDBModelContext _db = new PoriteDBModelContext())
            {
                return View(_db.MC909Table.ToList());
            }
        }

        public ActionResult Query909Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query909Index(MC909Table table)
        {
            string uQRCode = table.QRCode;
            string uCCWJudgment = table.CCWJudgmentResult;
            string uCWJudgment = table.CWJudgmentResult;
            DateTime uDateTime;
            PoriteDBModelContext db = new PoriteDBModelContext();
            var ListAll = db.MC909Table.Select(s => s);

            if (table.TestingDateTime.HasValue)
            {
                uDateTime = new DateTime();
                uDateTime = table.TestingDateTime.Value;
                DateTime uDateTime1 = uDateTime.AddDays(1);
                if (uDateTime != null)
                {
                    ListAll = ListAll.Where(w => w.TestingDateTime.Value >= uDateTime && w.TestingDateTime < uDateTime1);
                }
            }
            if (!string.IsNullOrWhiteSpace(uQRCode))
            {
                ListAll = ListAll.Where(w => w.QRCode.Contains(uQRCode));
            }
            if (!string.IsNullOrWhiteSpace(uCCWJudgment))
            {
                ListAll = ListAll.Where(w => w.CCWJudgmentResult.Contains(uCCWJudgment));
            }
            if (!string.IsNullOrWhiteSpace(uCWJudgment))
            {
                ListAll = ListAll.Where(w => w.CWJudgmentResult.Contains(uCWJudgment));
            }
            return View("Query909_Result", ListAll.ToList());
        }
        #endregion
        //=============================================
        #region MC908
        public ActionResult ListMc908()
        {
            using (PoriteDBModelContext _db = new PoriteDBModelContext())
            {
                return View(_db.MC908Table.ToList());
            }
        }

        public ActionResult Query908Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query908Index(MC908Table table)
        {
            string uQRCode = table.QRCode;
            string uCCWJudgment = table.CCWJudgmentResult;
            string uCWJudgment = table.CWJudgmentResult;
            DateTime uDateTime;
            PoriteDBModelContext db = new PoriteDBModelContext();
            var ListAll = db.MC908Table.Select(s => s);

            if (table.TestingDateTime.HasValue)
            {
                uDateTime = new DateTime();
                uDateTime = table.TestingDateTime.Value;
                DateTime uDateTime1 = uDateTime.AddDays(1);
                if (uDateTime != null)
                {
                    ListAll = ListAll.Where(w => w.TestingDateTime.Value >= uDateTime && w.TestingDateTime < uDateTime1);
                }
            }
            if (!string.IsNullOrWhiteSpace(uQRCode))
            {
                ListAll = ListAll.Where(w => w.QRCode.Contains(uQRCode));
            }
            if (!string.IsNullOrWhiteSpace(uCCWJudgment))
            {
                ListAll = ListAll.Where(w => w.CCWJudgmentResult.Contains(uCCWJudgment));
            }
            if (!string.IsNullOrWhiteSpace(uCWJudgment))
            {
                ListAll = ListAll.Where(w => w.CWJudgmentResult.Contains(uCWJudgment));
            }
            return View("Query908_Result", ListAll.ToList());
        }
        #endregion
        //=============================================
    }
}