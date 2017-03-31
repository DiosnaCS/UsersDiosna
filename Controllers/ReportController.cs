﻿using System;
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
        public async void SelectReport(ReportFormModel model) {
            int dateFrom = model.pkTimeFrom; //in pkTime
            int dateTo = model.pkTimeTo; //in pkTime
            int recipeNo = model.Recipe;
            bool OverLimits = model.Par0Sel;
            List<object[]> results = new List<object[]>();
            string sql = "";
            db db = new db("Dubravica", 2);
            if (OverLimits == true)
            {
                if ((model.Par1Sel || model.Par2Sel || model.Par3Sel || model.Par4Sel) == true) {
                    if (model.Par1Sel == true) {
                        sql =
                    }
                    if (model.Par2Sel == true) {

                    }
                    if (model.Par3Sel == true)
                    {

                    }
                    if (model.Par4Sel == true)
                    {

                    }
                } else {

                }
            }
            else {
                if ((model.Par1Sel || model.Par2Sel || model.Par3Sel || model.Par4Sel) == true)
                {
                    if (model.Par1Sel == true)
                    {
                        sql =
                    }
                    if (model.Par2Sel == true)
                    {
                    }
                    if (model.Par3Sel == true)
                    {

                    }
                    if (model.Par4Sel == true)
                    {

                    }
                }
            }

            results = await db.multipleItemSelectPostgresAsync(sql);
        }
    }
}