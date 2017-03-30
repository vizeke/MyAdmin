namespace MyAdmin.Application.Models
{
    public class SistemasModel
    {
        public enum TipoServidorBD { SqlServer, Oracle }

        public enum Ambiente { Desenvolvimento, Homologacao, Producao }

        public bool AllowConnStringDefinition { get; set; }
    }
}