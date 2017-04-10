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
        List<Batch> BatchList = new List<Batch>();
        public void getBatch(object[] result) {
            Batch batch = new Batch() { };
            BatchList.Add(batch);
        }
        public ReportViewModel getReport(List<Batch> BatchList) {
            return new ReportViewModel() { Batches = BatchList.ToArray() };
        }
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(ReportFormModel model) {
            SelectReport(model);
            return PartialView("_Overview");
        }
        /// <summary>
        /// Unfortuantly this is only for Dubravica 
        /// </summary>
        /// <param name="model">Model with data from form</param>
        public void SelectReport(ReportFormModel model) {
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
            string sqlFunctions = "";
            string where = "";
            db db = new db("Dubravica", 2);
            if (OverLimits == true)
            {
                    if (model.Recipe == 0) {
                        if (AmountSel == true) {
                            float AmountTolerance = model.AmountTolerance;
                            sqlFunctions += "getavolationoverlimits(" + dateFrom +","+ dateTo +", "+ AmountTolerance +", 11, \"diBatchNo\") AS amount";
                            where += "\"diBatchNo\" IN(getwrongbatchesoverlimits(" + dateFrom +", "+ dateTo +", "+ AmountTolerance + ", 11, \"diBatchNo\"))";
                        }
                        if (TempSel == true) {
                            float TemperatureTolerance = model.TempTolerance;                        
						    if (where.Length != 0 && sqlFunctions.Length != 0)
						    {
							    sqlFunctions += ", getavolationoverlimits(" + dateFrom + "," + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
							    where += " AND \"diBatchNo\" IN(getwrongbatchesoverlimits(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
						    }
						    else
						    {
							    sqlFunctions += "getavolationoverlimits(" + dateFrom + "," + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
							    where += "\"diBatchNo\" IN(getwrongbatchesoverlimits(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
						    }
                        }
                        if (StepTimeSel == true)
                        {
						    float StepTimeTolerance = model.StepTimeTolerance;
						    sqlFunctions += "getavolationoverlimits(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 11, \"diBatchNo\") AS steptime";
						    if (where.Length != 0 && sqlFunctions.Length != 0)
						    {
							    sqlFunctions += ", getavolationoverlimits(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
							    where += " AND \"diBatchNo\" IN(getwrongbatchesoverlimits(" + dateFrom + ", " + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
						    }
						    else
						    {
							    sqlFunctions += "getavolationoverlimits(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
							    where += "\"diBatchNo\" IN(getwrongbatchesoverlimits(" + dateFrom + ", " + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
						    }
					    }
                        if (InterStepTimeSel == true)
                        {
						    float InterStepTimeTolerance = model.InterStepTimeTolerance;
                            if (where.Length != 0 && sqlFunctions.Length != 0)
                            {
                                sqlFunctions += ", getintersteptime(\"diBatchNo\") AS intersteptime";
                                where += " AND getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                            }
                            else {
                                sqlFunctions += "getintersteptime(\"diBatchNo\") AS intersteptime";
                                where += "getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                            }
                        }
                    } else {
                            if (AmountSel == true)
                            {
                                float AmountTolerance = model.AmountTolerance;
                                sqlFunctions += "getavolationoverlimitsrcpno(" + dateFrom + "," + dateTo + ", " + AmountTolerance + ", 11, \"diBatchNo\") AS amount";
                                where += "\"diBatchNo\" IN(getwrongbatchesoverlimitsrcpno(" + dateFrom + ", " + dateTo + ", " + AmountTolerance + ", 11, \"diBatchNo\"))";
                            }
                            if (TempSel == true)
                            {
                                float TemperatureTolerance = model.TempTolerance;
                                if (where.Length != 0 && sqlFunctions.Length != 0)
                                {
                                    sqlFunctions += ", getavolationoverlimitsrcpno(" + dateFrom + "," + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                                    where += " AND \"diBatchNo\" IN(getwrongbatchesoverlimitsrcpno(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                                }
                                else
                                {
                                    sqlFunctions += "getavolationoverlimitsrcpno(" + dateFrom + "," + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                                    where += "\"diBatchNo\" IN(getwrongbatchesoverlimitsrcpno(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                                }
                            }
                            if (StepTimeSel == true)
                            {
                                float StepTimeTolerance = model.StepTimeTolerance;
                                if (where.Length != 0 && sqlFunctions.Length != 0)
                                {
                                    sqlFunctions += ", getavolationoverlimitsrcpno(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
                                    where += " AND \"diBatchNo\" IN(getwrongbatchesoverlimitsrcpno(" + dateFrom + ", " + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                                }
                                else
                                {
                                    sqlFunctions += "getavolationoverlimitsrcpno(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
                                    where += "\"diBatchNo\" IN(getwrongbatchesoverlimitsrcpno(" + dateFrom + ", " + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                                }
                            }
                            if (InterStepTimeSel == true)
                            {
                                float InterStepTimeTolerance = model.InterStepTimeTolerance;
                                if (where.Length != 0 && sqlFunctions.Length != 0)
                                {
                                    sqlFunctions += ", getintersteptime(\"diBatchNo\") AS intersteptime";
                                    where += " AND getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                                }
                                else
                                {
                                    sqlFunctions += "getintersteptime(\"diBatchNo\") AS intersteptime";
                                    where += "getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                                }
                            }
                    }
                sql = "SELECT DISTINCT getstarttime(\"diBatchNo\") AS startTime, getendtime(\"diBatchNo\") AS endTime, getrecipename(\"iRecipeNo\") AS recipeName," + sqlFunctions + " FROM events WHERE " +  where;
            }
            else {
                if (model.Recipe == 0)
                {
                    if (AmountSel == true)
                    {
                        float AmountTolerance = model.AmountTolerance;
                        sqlFunctions += "getavolation(" + dateFrom + "," + dateTo + ", " + AmountTolerance + ", 11, \"diBatchNo\") AS amount";
                        where += "\"diBatchNo\" IN(getwrongbatches(" + dateFrom + ", " + dateTo + ", " + AmountTolerance + ", 11, \"diBatchNo\"))";
                    }
                    if (TempSel == true)
                    {
                        float TemperatureTolerance = model.TempTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getavolation(" + dateFrom + "," + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                            where += " AND \"diBatchNo\" IN(getwrongbatches(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                        }
                        else
                        {
                            sqlFunctions += "getavolation(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                            where += "\"diBatchNo\" IN(getwrongbatches(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                        }
                    }
                    if (StepTimeSel == true)
                    {
                        float StepTimeTolerance = model.StepTimeTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getavolation(" + dateFrom + "," + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
                            where += " AND \"diBatchNo\" IN(getwrongbatches(" + dateFrom + ", " + dateTo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                        }
                        else
                        {
                            sqlFunctions += "getavolation(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS temperature";
                            where += "\"diBatchNo\" IN(getwrongbatches(" + dateFrom + ", " + dateTo + "," + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                        }
                    }
                    if (InterStepTimeSel == true)
                    {
                        float InterStepTimeTolerance = model.InterStepTimeTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getintersteptime(\"diBatchNo\") AS intersteptime";
                            where += " AND getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                        }
                        else
                        {
                            sqlFunctions += "getintersteptime(\"diBatchNo\") AS intersteptime";
                            where += "getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                        }
                    }
                }
                
             else {
                    if (AmountSel == true)
                    {
                        float AmountTolerance = model.AmountTolerance;
                        sqlFunctions += "getavolationrcpno(" + dateFrom + "," + dateTo + ", "+ recipeNo +", " + AmountTolerance + ", 11, \"diBatchNo\") AS amount";
                        where += "\"diBatchNo\" IN(getwrongbatchesrcpno(" + dateFrom + ", " + dateTo + ", " + recipeNo + ", " + AmountTolerance + ", 11, \"diBatchNo\"))";
                    }
                    if (TempSel == true)
                    {
                        float TemperatureTolerance = model.TempTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getavolationrcpno(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                            where += " AND \"diBatchNo\" IN(getwrongbatchesrcpno(" + dateFrom + ", " + dateTo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                        }
                        else
                        {
                            sqlFunctions += "getavolationrcpno(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\") AS temperature";
                            where += "\"diBatchNo\" IN(getwrongbatchesrcpno(" + dateFrom + ", " + dateTo + ", " + recipeNo + ", " + TemperatureTolerance + ", 21, \"diBatchNo\"))";
                        }
                    }
                    if (StepTimeSel == true)
                    {
                        float StepTimeTolerance = model.StepTimeTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getavolationrcpno(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS steptime";
                            where += " AND \"diBatchNo\" IN(getwrongbatchesrcpno(" + dateFrom + ", " + dateTo + ", " + recipeNo + "," + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                        }
                        else
                        {
                            sqlFunctions += "getavolationrcpno(" + dateFrom + "," + dateTo + ", " + recipeNo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\") AS steptime";
                            where += "\"diBatchNo\" IN(getwrongbatchesrcpno(" + dateFrom + ", " + dateTo + ", " + recipeNo + ", " + StepTimeTolerance + ", 31, \"diBatchNo\"))";
                        }
                    }
                    if (InterStepTimeSel == true)
                    {
                        float InterStepTimeTolerance = model.InterStepTimeTolerance;
                        if (where.Length != 0 && sqlFunctions.Length != 0)
                        {
                            sqlFunctions += ", getintersteptime(\"diBatchNo\") AS intersteptime";
                            where += " AND getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                        }
                        else
                        {
                            sqlFunctions += "getintersteptime(\"diBatchNo\") AS intersteptime";
                            where += "getintersteptime(\"diBatchNo\")>=" + InterStepTimeTolerance;
                        }
                    }
                }
            sql = "SELECT DISTINCT getstarttime(\"diBatchNo\") AS startTime, getendtime(\"diBatchNo\") AS endTime, getrecipename(\"iRecipeNo\") AS recipeName," + sqlFunctions + " FROM events WHERE " + where;
        }
            results = db.multipleItemSelectPostgres(sql);
        }
    }
}