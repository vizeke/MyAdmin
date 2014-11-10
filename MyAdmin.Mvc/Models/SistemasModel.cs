using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAdmin.Mvc.Models
{
    public class SistemasModel
    {
        public enum TipoServidorBD { SqlServer, Oracle }

        public enum Ambiente { Desenvolvimento, Homologacao, Producao }
    }
}