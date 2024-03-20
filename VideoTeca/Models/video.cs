namespace VideoTeca.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("video")]
    public partial class video
    {
        public long id { get; set; }

        [Required]
        [StringLength(255)]
        public string url { get; set; }

        [Required]
        [StringLength(255)]
        public string titulo { get; set; }

        [StringLength(4000)]
        public string descricao { get; set; }

        public bool active { get; set; }

        public long enviadoPor { get; set; }

        public long id_status { get; set; }

        public long id_area { get; set; }

        public long? id_subarea { get; set; }

        public virtual area area { get; set; }

        public virtual status status { get; set; }

        public virtual subarea subarea { get; set; }

        public virtual usuario usuario { get; set; }
    }
}
