using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAdmin.Mvc.Models
{
    public class QueryModel
    {
        public string Sistema { get; set; }
        public string Ambiente { get; set; }
        public string Banco { get; set; }
        public bool SalvaArquivo { get; set; }
        public string NomeArquivo { get; set; }

        public string queryActiveText { get; set; }
        public string Query { get; set; }
        public string Resultado { get; set; }

        public List<string> Temas { get; set; }
    }
}