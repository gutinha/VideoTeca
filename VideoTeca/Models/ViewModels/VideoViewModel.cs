using System;
using System.ComponentModel.DataAnnotations;

namespace VideoTeca.Models.ViewModels
{
    public class VideoViewModel
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public DateTime EnviadoEm { get; set; }
        public string AreaNome { get; set; }
        public string SubareaNome { get; set; }
        public string StatusNome { get; set; }
        public bool Aprovado { get; set; }
        public bool Active { get; set; }
        [StringLength(4000)]
        public string Descricao { get; set; }
        [Required]
        [StringLength(255)]
        public string Url { get; set; }
        public long? SubareaId { get; set; }
        public long AreaId { get; set; }
        public long StatusId { get; set; }
    }
}
