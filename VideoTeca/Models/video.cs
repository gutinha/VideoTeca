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

        [Required]
        [StringLength(100)]
        public string descricao { get; set; }

        [StringLength(255)]
        public string enviadoPor { get; set; }

        [Required]
        public bool active { get; set; }
    }
}
