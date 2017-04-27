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
        public static ReportModel RVM;
        // GET: Report
        public ActionResult Index()
        {
            ReportModel RVM = new ReportModel();
            RVM = SelectReports(RVM);
            ViewBag.RVM = RVM;
            return View(RVM);
        }
        
        [HttpPost]
        public ActionResult Index(ReportModel model) {
            RVM = SelectReports(model);
            ViewBag.RVM = RVM;
            return View(RVM);
        }

        public ActionResult getBatch()
        {
            int batchId = int.Parse(Request.QueryString["batchid"].ToString());
            ViewBag.Steps = getBatchData(batchId);
            return View("Index", RVM);
        }
        /// <summary>
        /// Unfortuantly this is only for Dubravica 
        /// </summary>
        /// <param name="model">Model with data from form</param>
        public ReportModel SelectReports(ReportModel model)
        {
            List<object[]> results = getReportData(model);
            
            model.Batches = new Batch[results.Count];
            int i = 0;
            foreach (object[] result in results)
            {
                Batch batch = new Batch();

                //Id = BatchNo
                if (result[0] != DBNull.Value)
                {
                    batch.Id = Convert.ToUInt32(result[0]);
                }
                //pkTime StartTime format to model
                if (result[1] != DBNull.Value)
                {
                    long pkTime = Convert.ToInt64(result[1]);
                    long timeInNanoSeconds = pkTime * 10000000;
                    DateTime datetimeStart = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                    batch.StartTime = datetimeStart;
                }
                //pkTime EndTime format to model
                if (result[2] != DBNull.Value)
                {
                    long pkTime = Convert.ToInt64(result[2]);
                    long timeInNanoSeconds = pkTime * 10000000;
                    DateTime datetimeEnd = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                    batch.EndTime = datetimeEnd;
                }

                //RecipeName 
                batch.RecipeName = result[3].ToString();

                //RecipeNumber
                if (result[4] != DBNull.Value)
                {
                    batch.RecipeNo = Convert.ToInt32(result[4]);
                }

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
                model.Batches[i] = batch;
                i++;
            }
            return model;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static List<object[]> getReportData(ReportModel model)
        {
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

            sql = "SELECT dibatchno, MAX(pktimefrom) AS timefrom, MAX(pktimeto) AS timeto,";
            sql += "MAX(rcpname) AS rcpname, MAX(recipenumber) AS recipenumber,";
            sql += "MAX(maxamount) AS maxamnt, MAX(mintamount) AS minamnt,";
            sql += "MAX(maxtemperature) AS maxtemp, MAX(mintemperature) AS mintemp,";
            sql += "MAX(maxsteptime) AS maxst, MAX(minsteptime) AS minst,";
            sql += "MAX(maxintersteptime) AS maxist, MAX(minintersteptime) AS minist ";
            sql = string.Format(sql + "FROM get_bad_batches({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}) GROUP BY dibatchno ORDER BY dibatchno ASC",
                dateFrom, dateTo, recipeNo, OverLimits, AmountSel, model.AmountTolerance,
                TempSel, model.TempTolerance, StepTimeSel, model.StepTimeTolerance, InterStepTimeSel,
                model.InterStepTimeTolerance);

            results = db.multipleItemSelectPostgres(sql);
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchId"></param>
        public Steps getBatchData(int batchId)
        {
            string sql = "";
            List<object[]> results = new List<object[]>();
            db db = new db("Dubravica", 2);

            sql = "SELECT MAX(batchno)," +
                    "step," +
                    "MAX(recipeno), " +
                    "MAX(timefrom), " +
                    "MAX(timeto), " +
                    "MAX(operation), " +
                    "MAX(deviceid), " +
                    "MAX(devicerecipe), " +
                    "MAX(need), " +
                    "MAX(done), " +
                    "MAX(status_forced), " +
                    "MAX(status_ok), " +
                    "MAX(stepscount) " +
                        "FROM get_batch(" + batchId + ") GROUP BY step ORDER BY step DESC;";

            results = db.multipleItemSelectPostgres(sql);
            Steps Steps = returnBatch(results);

            return Steps;
        }
        /// <summary>
        /// Refractored method to return batch in model 
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static Steps returnBatch(List<object[]> results)
        {
            Steps Steps = new Steps();
            RecipeStep rcpStep = new RecipeStep();

            //BatchId to model
            if (results[0][0] != DBNull.Value)
            {
                Steps.Id = Convert.ToUInt32(results[0][0]);
            }

            //BowlId to model
            if (results[0][6] != DBNull.Value)
            {
                Steps.BowlId = Convert.ToInt32(results[0][6]);
            }

            //pkTime StartTime format to model
            if (results[0][3] != DBNull.Value)
            {
                long pkTime = Convert.ToInt64(results[0][3]);
                long timeInNanoSeconds = pkTime * 10000000;
                DateTime datetimeStartBatch = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                Steps.StartTime = datetimeStartBatch;
            }
            //pkTime EndTime format to model
            if (results[0][4] != DBNull.Value)
            {
                long pkTime = Convert.ToInt64(results[0][4]);
                long timeInNanoSeconds = pkTime * 10000000;
                DateTime datetimeEndBatch = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                Steps.EndTime = datetimeEndBatch;
            }
            //RecipeNo to model
            if (results[0][2] != DBNull.Value)
            {
                Steps.RecipeNo = Convert.ToInt32(results[0][2]);
            }

            //RecipeName to model
            Steps.RecipeName = results[0][7].ToString();

            //Number of steps
            if (results[0][12] != DBNull.Value)
            {
                Steps.StepsCount = Convert.ToInt32(results[0][12]);
            }

            for (int i = 1; i < results.Count; i++)
            {
                RecipeStep recipeStep = new RecipeStep();

                //Current Step
                if (results[i][1] != DBNull.Value)
                {
                    recipeStep.step = (int)results[i][1];
                }
                //Device RecipeNo
                if (results[i][2] != DBNull.Value)
                {
                    recipeStep.RecipeNo = Convert.ToInt32(results[i][2]);
                }

                //pkTime StartTime format to model
                if (results[i][3] != DBNull.Value)
                {
                    long pkTime = Convert.ToInt64(results[i][3]);
                    long timeInNanoSeconds = pkTime * 10000000;
                    DateTime datetimeStart = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                    recipeStep.StartTime = datetimeStart;
                }
                //pkTime EndTime format to model
                if (results[i][4] != DBNull.Value)
                {
                    long pkTime = Convert.ToInt64(results[i][4]);
                    long timeInNanoSeconds = pkTime * 10000000;
                    DateTime datetimeEnd = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                    recipeStep.EndTime = datetimeEnd;
                }
                //type of operation
                switch ((int)results[i][5])
                {
                    case 15:
                        recipeStep.OperationNr = OperationType.Dosing;
                        break;
                    case 25:
                        recipeStep.OperationNr = OperationType.Kneading;
                        break;
                    case 35:
                        recipeStep.OperationNr = OperationType.Ripping;
                        break;
                    case 45:
                        recipeStep.OperationNr = OperationType.Tipping;
                        break;
                }

                //DeviceId
                if (results[i][6] != DBNull.Value)
                {
                    recipeStep.DeviceId = Convert.ToInt32(results[i][6]);
                }
                
                //DeviceName
                recipeStep.Device = results[i][7].ToString();

                //Need
                if (results[i][8] != DBNull.Value)
                {
                    recipeStep.Need = Convert.ToInt32(results[i][8]);
                }

                //Done
                if (results[i][9] != DBNull.Value)
                {
                    recipeStep.Done = Convert.ToInt32(results[i][9]);
                }

                //StepStatus
                switch ((int)results[i][10])
                {
                    case 0:
                        recipeStep.Status = StepStatus.Error;
                        break;
                    case 1:
                        recipeStep.Status = StepStatus.ForcedStart;
                        break;
                }
                switch ((int)results[i][11])
                {
                    case 0:
                        recipeStep.Status |= StepStatus.Error;
                        break;
                    case 1:
                        recipeStep.Status |= StepStatus.OK;
                        break;
                }

                Steps.BatchSteps.Add(recipeStep);
            }

            return Steps;
        }
    }
}