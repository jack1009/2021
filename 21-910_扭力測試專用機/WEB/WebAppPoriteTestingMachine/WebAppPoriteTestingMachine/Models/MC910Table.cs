namespace WebAppPoriteTestingMachine.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MC910Table
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string QRCode { get; set; }

        public DateTime? TestingDateTime { get; set; }

        [StringLength(3)]
        public string JudgmentResult { get; set; }

        public double? TestingAngle { get; set; }

        public double? TestingTorque { get; set; }
    }
}
