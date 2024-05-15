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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public video()
        {
            video_avaliacoes = new HashSet<video_avaliacoes>();
        }

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

        public bool aprovado { get; set; }

        public DateTime enviadoEm { get; set; }

        public virtual area area { get; set; }

        public virtual status status { get; set; }

        public virtual subarea subarea { get; set; }

        public virtual usuario usuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<video_avaliacoes> video_avaliacoes { get; set; }
    }
}
