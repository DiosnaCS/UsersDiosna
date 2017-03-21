using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsersDiosna.Report.Models;

namespace UsersDiosna.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        List<Batch> BatchList = new List<Batch>();
        public void getBatch() {
            Batch batch = new Batch() { };
            BatchList.Add(batch);
        }
        public ReportViewModel getReport() {
            return new ReportViewModel() { Batches = BatchList.ToArray() };
        }
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(ReportFormModel model) {
            return View();
        }
    }
}