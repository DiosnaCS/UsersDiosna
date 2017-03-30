using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UsersDiosna.Report.Models
{   

    public class ReportFormModel
    {
        public Int32 ConvertDT2pkTime(DateTime dateTime)
        {
            DateTime pkTimeStart = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);
            Int32 pkTime = 0;            
            pkTime = (Int32)(dateTime.ToUniversalTime() - pkTimeStart).TotalSeconds;

            return pkTime;
        }

        // StartId
        public Int32 StartId { get; set; }

        // Count
        public Int16 Count { get; set; }

        // pk time from
        public Int32 pkTimeFrom { get; set; }

        // pk time to
        public Int32 pkTimeTo { get; set; }

        // date time from
        [Display(Name = "From:")]
        public DateTime DateTimeFormFrom { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateTimeFrom {
            get { return DateTimeFormFrom; }
            set {
                pkTimeFrom = ConvertDT2pkTime(value);
            }
        }       

        // date time to
        [Display(Name = "To:")]
        public DateTime DateTimeFormTo { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateTimeTo {
            get { return DateTimeFormTo; }
            set {
                pkTimeTo = ConvertDT2pkTime(value);
            }
        }

        // recipe        
        [Display(Name = "Recipe:")]
        public int Recipe { get; set; }

        // over limits
        [Display(Name = "over lim.")]
        public bool Par0Sel { get; set; }

        // amount       
        [Display(Name = "Amount: ")]
        public bool Par1Sel { get; set; }       
        public string Par1Tol { get; set; }        
        public const int Par1Tol_coef = 1000;
        public int Par1Tol_req { get; set; }

        // temperature
        [Display(Name = "Temperature: ")]
        public bool Par2Sel { get; set; }
        public string Par2Tol { get; set; }        
        public const int Par2Tol_coef = 10;        
        public int Par2Tol_req { get; set; }

        // step time
        [Display(Name = "Step time: ")]
        public bool Par3Sel { get; set; }        
        public string Par3Tol { get; set; }
        public const int Par3Tol_coef = 60;                
        public int Par3Tol_req { get; set; }

        // interstep time
        [Display(Name = "Interstep time: ")]
        public bool Par4Sel { get; set; }        
        public string Par4Tol { get; set; }
        public const int Par4Tol_coef = 60;
        public int Par4Tol_req { get; set; }

    }
    [Flags]
    public enum BatchStatus
    {
        None = 0,
        Amount = 1,
        Temperature = 2,
        StepTime = 4,
        InterStepTime = 8
    }

    [Flags]
    public enum StepStatus
    {
        None = 0,
        Forced = 1,
        OK = 2,
     }
    public class ReportViewModel
    {
        public Batch[] Batches; //array of current batches 
    }
    public class Batch
    {
        public uint Id /*diRecordNo directly from db*/ { get; set; }
        public DateTime StartTime /*In pkTime from server, at a client conversion to dateTime*/ { get { return StartTime; }
            set
            {
                long timeInNanoSeconds = value.Ticks * 10000000;
                DateTime datetime = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                StartTime = datetime;
            } }
        public DateTime EndTime /*In pkTime from server, at a client conversion to dateTime*/ { get { return EndTime; }
            set
            {
                long timeInNanoSeconds = value.Ticks * 10000000;
                DateTime datetime = new DateTime(((630836424000000000 - 13608000000000) + timeInNanoSeconds));
                EndTime = datetime;
            } }
        public short RecipeNr /*iRecipeNo directly from db*/ { get; set; }
        public string RecipeName /*string directly from db*/ { get; set; }
        public BatchStatus status /* flag type */ { get; set; }
        public List<RecipeStep> Steps /*List of Bataches for this Product */{ get; set; } //null until batch detail click
    }

    public class RecipeStep
    {
        //Whole model sorted from time ascending
        public DateTime StartTime /*In pkTime from server, at a client conversion to dateTime*/ { get; set; }
        public DateTime EndTime /*In pkTime from server, at a client conversion to dateTime*/ { get; set; }
        public byte OperationNr /*Resolve how to get that*/ { get; set; }
        public string Device /*DeviceName directly from db*/ { get; set; }
        public Int32 Need /*DeviceName directly from db*/ { get; set; }
        public Int32 Done /*diNeedDone directly from db*/ { get; set; }

        //Diff should be calculted by client

        public StepStatus Status /*siStatus directly from db*/ { get; set; }
        
    }
}