namespace WebAppPoriteTestingMachine.Models
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

        public double? CCWSpeed { get; set; }

        public double? CWSpeed { get; set; }

        public double? CCWCurrent { get; set; }

        public double? CWCurrent { get; set; }

        public double? CCWAvgSpeed { get; set; }

        public double? CWAvgSpeed { get; set; }

        public double? CCWAvgCurrent { get; set; }

        public double? CWAvgCurrent { get; set; }
    }
}
