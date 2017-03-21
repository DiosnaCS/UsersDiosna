using UsersDiosna.Alarms.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;


namespace UsersDiosna.Controllers
{
    [Authorize]
    public class AlarmController : Controller
    {
        //define variables
        public int i = 0;
        private string DB, lang, connstring;
        private int plcID = 0;
        //Predefiened arrays

        public List<string> titles = new List<string>();
        public List<int> alarm_ids = new List<int>();
        public int[] id = new int[32];
        public int[] alarm_id = new int[32];
        public int[] originTime = new int[32];
        public int[] expTime = new int[32];
        public string[] labels = new string[32];
        public string[] datetimeOrigin = new string[32];
        public string[] datetimeExp = new string[32];

        public string pkTimeToDateTime(long timeForFormat)
        {
            long timeInNanoSeconds = (timeForFormat * 10000000);
            //TimeSpan converted = TimeSpan.FromSeconds(timeForFormat);
            DateTime DateTime = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
            //DateTime DateTime = new DateTime(years, months, days, hours, minutes, seconds);
            CultureInfo cultureInfo = new CultureInfo("en-US");
            string sDateTime = DateTime.ToString("yyyy-MM-dd h:mm tt", cultureInfo);
            return sDateTime;
        }

        public string DBConnnection()
        {
            foreach (string key in Session.Keys)
            {
                if (key.Contains("dbName" + Request.QueryString["name"] + Request.QueryString["plc"]))
                {
                    DB = Session[key].ToString();
                }
                if (key.Contains("lang" + Request.QueryString["name"] + Request.QueryString["plc"]))
                {
                    lang = Session[key].ToString();
                }
                if (key.Contains("plc" + Request.QueryString["name"] + Request.QueryString["plc"]))
                {
                    if ((Int32.TryParse(Session[key].ToString(), out plcID) == true))
                    {
                    }
                    else
                    {
                        plcID = 0;
                    }
                }
            }
            // PostgeSQL-style connection string            
            string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
              "192.168.2.12", 5432, "postgres", "Nordit0276", DB);
            return connstring;
        }

        public void SelectAlarms(int NumberOfRecords, int PageNumber)
        {
            if (NumberOfRecords == 0)
            {
                NumberOfRecords = 20;
            }
            if (connstring == null)
            {
                connstring = DBConnnection();
            }
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            if (plcID == 0)
            {
                plcID = 1;
            }
            string sql = "SELECT * FROM alarm_history WHERE plc_id=" + plcID + " ORDER BY origin_pktime DESC LIMIT " + NumberOfRecords + " OFFSET " + (PageNumber * NumberOfRecords);
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            //Prepare DataReader
            NpgsqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id[i] = Int16.Parse(dr["plc_id"].ToString());
                alarm_id[i] = Int16.Parse(dr["alarm_id"].ToString());
                //small improvment beacause alarm_id in table alarm_texts and alarm_id in table alarm_history are bind
                labels[i] = titles[alarm_id[i]];

                originTime[i] = Int32.Parse(dr["origin_pktime"].ToString());
                datetimeOrigin[i] = pkTimeToDateTime(originTime[i]);
                expTime[i] = Int32.Parse(dr["expiry_pktime"].ToString());
                datetimeExp[i] = pkTimeToDateTime(expTime[i]);
                i++;
            }
            //cmd.Dispose();
            conn.Close();

            //Give page number and number of records on page to view
            ViewBag.PageNumber = PageNumber;
            ViewBag.NumberOfRecords = NumberOfRecords;

            //Give data to view
            ViewBag.Id = alarm_id;
            ViewBag.Label = labels;
            ViewBag.originTime = datetimeOrigin;
            ViewBag.expTime = datetimeExp;
        }


        public void SelectAlarmsTexts()
        {
            // Making connection with Npgsql provider
            if (connstring == null)
            {
                connstring = DBConnnection();
            }
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            //If lang from config does not exists set it to english
            if (lang == null)
            {
                lang = "en";
            }
            // Execute the query and obtain a result set                
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title,alarm_id FROM alarm_texts WHERE lang='" + lang + "'", conn);
            //Prepare DataReader
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            //Cycle reads data from result set 
            while (dataReader.Read())
            {
                titles.Add(dataReader["title"].ToString());
                alarm_ids.Add(Int16.Parse(dataReader["alarm_id"].ToString()));

            }
            //We need to close connection to select texts
            //cmd.Dispose();
            conn.Close();

        }
        /// <summary>
        /// Select possible languages for project
        /// </summary>
        /// <returns>List of possible languages for project</returns>
        public List<string> possibleLangs() {
            List<string> possibleLangs = new List<string>();
            // Making connection with Npgsql provider
            if (connstring == null)
            {
                connstring = DBConnnection();
            }
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            // Execute the query and obtain a result set                
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT DISTINCT lang FROM alarm_texts", conn);
            //Prepare DataReader
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            //Cycle reads data from result set 
            while (dataReader.Read())
            {
                possibleLangs.Add(dataReader["lang"].ToString());
            }
            //We need to close connection to select texts
            conn.Close();
            //Only if something went wrong set lang to en
            if (possibleLangs.Count == 0 || possibleLangs == null) {
                possibleLangs.Add("en");
            }
            return possibleLangs;
        }
        public ActionResult Index()
        {

            try
            {
                connstring = DBConnnection();
                ViewBag.possibleLangs = possibleLangs();
                SelectAlarmsTexts();
                ViewBag.langEnabled = lang;
                //--------------------------------------------------------------------------

                //Select Alarms using SelectAlarms(int count) method (defined upper )
                SelectAlarms(20, 0);

            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why

                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Alarm
        [HttpPost]
        public ActionResult Index(AlarmFormModel model, string returnUrl)
        {
            try
            {
                if (Request.Form.Get("possibleLangs") != null)
                {
                    lang = Request.Form.Get("possibleLangs");
                }
                else
                {
                    lang = "en";
                }
                ViewBag.langEnabled = lang;
                SelectAlarmsTexts();
                //--------------------------------------------------------------------------
                int PageNumber = model.someId;
                int NumberOfRecords = model.NumberOfRecords;
                SelectAlarms(NumberOfRecords, PageNumber);
                ViewBag.possibleLangs = possibleLangs();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why

                throw msg;
            }

            return View(model);
        }
    }
}