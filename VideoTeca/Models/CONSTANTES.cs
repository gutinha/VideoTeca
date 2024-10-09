using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTeca.Models
{
    public class CONSTANTES
    {
        public static readonly Dictionary<long, string> userTypes = new Dictionary<long, string>
        {
            { 1, "Usuário" },
            { 2, "Professor" },
            { 3, "Avaliador" },
            { 4, "Dicom" },
            { 5, "Coordenador" }
        };
    }
}