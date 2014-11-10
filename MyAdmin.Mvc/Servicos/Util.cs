using System;
using MyAdmin.Mvc.Models;

namespace MyAdmin.Mvc.Servicos
{
    public static class Util
    {
        private static string getDescConf(string sistema, string ambiente)
        {

            return string.Format("{0}-{1}",
                                 sistema,
                                 ambiente == SistemasModel.Ambiente.Desenvolvimento.ToString() ? "des" :
                                 ambiente == SistemasModel.Ambiente.Homologacao.ToString() ? "hom" :
                                 ambiente == SistemasModel.Ambiente.Producao.ToString() ? "pro" : "");
        }

        public static string getConnectionString(string sistema, string ambiente)
        {
            if (!string.IsNullOrEmpty(sistema) && !string.IsNullOrEmpty(ambiente))
                return System.Configuration.ConfigurationManager.ConnectionStrings[getDescConf(sistema, ambiente)].ConnectionString;
            else
                throw new Exception("Impossível identificar o sistema/ambiente, selecione os itens desejados.");
        }

        public static bool validDB(string sistema, string ambiente)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[getDescConf(sistema, ambiente)] != null;
        }
    }
}