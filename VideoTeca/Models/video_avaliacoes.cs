namespace VideoTeca.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class video_avaliacoes
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id_video { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id_avaliador { get; set; }

        [StringLength(100)]
        public string justificativa { get; set; }

        public DateTime? data_avaliacao { get; set; }

        public virtual usuario usuario { get; set; }

        public virtual video video { get; set; }
    }
}
