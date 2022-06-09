namespace _21_910
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MC909Table
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string QRCode { get; set; }

        public DateTime? TestingDateTime { get; set; }

        [StringLength(3)]
        public string CCWJudgmentResult { get; set; }

        [StringLength(3)]
        public string CWJudgmentResult { get; set; }

        public double? CCWMaxSpeed { get; set; }

        public double? CWMaxSpeed { get; set; }

        public double? CCWMaxCurrent { get; set; }

        public double? CWMaxCurrent { get; set; }

        public double? CCWAvgSpeed { get; set; }

        public double? CWAvgSpeed { get; set; }

        public double? CCWAvgCurrent { get; set; }

        public double? CWAvgCurrent { get; set; }

        public double? CCWMinCurrent { get; set; }

        public double? CWMinCurrent { get; set; }
    }
}
