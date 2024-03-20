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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public subarea()
        {
            video = new HashSet<video>();
        }

        public long id { get; set; }

        [Required]
        [StringLength(255)]
        public string nome { get; set; }

        public long id_area { get; set; }

        public virtual area area { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<video> video { get; set; }
    }
}
