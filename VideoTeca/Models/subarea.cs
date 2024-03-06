namespace VideoTeca.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subarea")]
    public partial class subarea
    {
        public long id { get; set; }

        [Required]
        [StringLength(255)]
        public string nome { get; set; }

        public long id_area { get; set; }

        public virtual area area { get; set; }
    }
}
