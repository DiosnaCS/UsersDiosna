using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsersDiosna.Report.Models;
using System.Collections.Generic;

namespace UsersDiosna.Controllers
{
    //[Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            ReportFormModel newOne = new ReportFormModel();
            ReportViewModel RVM = SelectReports(newOne);
            ViewBag.RVM = RVM;
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(ReportFormModel model) {
            ReportViewModel RVM = SelectReports(model);
            ViewBag.RVM = RVM;
            return View();
        }

        public ActionResult getBatch()
        {
            int batchId = int.Parse(Request.QueryString["id"].ToString());
            getBatchData(batchId);
            return View("Index");
        }
        /// <summary>
        /// Unfortuantly this is only for Dubravica 
        /// </summary>
        /// <param name="model">Model with data from form</param>
        public ReportViewModel SelectReports(ReportFormModel model) {
            int dateFrom = model.pkTimeFrom; //in pkTime
            int dateTo = model.pkTimeTo; //in pkTime
            int recipeNo = model.Recipe;

            bool OverLimits = model.Par0Sel;
            bool AmountSel = model.Par1Sel;
            bool TempSel = model.Par2Sel;
            bool StepTimeSel = model.Par3Sel;
            bool InterStepTimeSel = model.Par4Sel;

            List<object[]> results = new List<object[]>();
            string sql = "";
            db db = new db("Dubravica", 2);

            sql =  "SELECT dibatchno, MAX(pktimefrom) AS timefrom, MAX(pktimeto) AS timeto,";
            sql += "MAX(rcpname) AS rcpname, MAX(rcpno) AS rcpno,";
            sql += "MAX(maxamount) AS maxamnt, MAX(mintamount) AS minamnt,";
            sql += "MAX(maxtemperature) AS maxtemp, MAX(mintemperature) AS mintemp,";
            sql += "MAX(maxsteptime) AS maxst, MAX(minsteptime) AS minst,";
            sql += "MAX(maxintersteptime) AS maxist, MAX(minintersteptime) AS minist ";
            sql = string.Format(sql + "FROM get_bad_batches({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}) GROUP BY dibatchno ORDER BY dibatchno ASC", 
                dateFrom, dateTo, recipeNo, OverLimits, AmountSel, model.AmountTolerance, 
                TempSel, model.TempTolerance, StepTimeSel, model.StepTimeTolerance, InterStepTimeSel,
                model.InterStepTimeTolerance);

            results = db.multipleItemSelectPostgres(sql);
            ReportViewModel RVM = new ReportViewModel();
            RVM.Batches = new Batch[results.Count];
            int i = 0;
            foreach (object[] result in results) {
                Batch batch = new Batch();

                //Id = BatchNo
                batch.Id = Convert.ToUInt32(result[0]);

                //pkTime StartTime format to model
                long pkTime = Convert.ToInt64(result[1]);
                long timeInNanoSeconds = pkTime * 10000000;
                DateTime datetimeStart = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                batch.StartTime = datetimeStart;

                //pkTime EndTime format to model
                pkTime = Convert.ToInt64(result[2]);
                timeInNanoSeconds = pkTime * 10000000;
                DateTime datetimeEnd = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                batch.EndTime = datetimeEnd;
                
                //RecipeName 
                batch.RecipeName = result[3].ToString();

                //RecipeNumber
                batch.RecipeNo = (int)result[4];

                batch.status = BatchStatus.None;
                //Batch Status
                if (result[5].ToString().Length != 0 && result[6].ToString().Length != 0)
                {
                    batch.status |= BatchStatus.AM;
                }
                if (result[7].ToString().Length != 0 && result[8].ToString().Length != 0)
                {
                    batch.status |= BatchStatus.Temp;
                }
                if (result[9].ToString().Length != 0 && result[10].ToString().Length != 0)
                {
                    batch.status |= BatchStatus.ST;
                }
                if (result[11].ToString().Length != 0 && result[12].ToString().Length != 0)
                {
                    batch.status |= BatchStatus.IST;
                }
                //Batch to Batches
                RVM.Batches[i] = batch;
                i++;
            }
            return RVM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public void getBatchData(int batchId)
        {
            string sql = "";
            List<object[]> results = new List<object[]>();
            db db = new db("Dubravica", 2);
            results = db.multipleItemSelectPostgres(sql);

            Steps Steps = new Steps();
            int i = 0;
            foreach (object[] result in results)
            {
                RecipeStep recipeStep = new RecipeStep();

            }
        }
    }
}