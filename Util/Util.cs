using System;
using Microsoft.Extensions.Configuration;
using MyAdmin.Application.Models;

namespace MyAdmin.Mvc.Util
{
    public static class Util
    {
        public static IConfigurationRoot Configuration { get; set; }
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
            {
                return Configuration.GetConnectionString(getDescConf(sistema, ambiente));
            }
            else
            {
                throw new Exception("Impossível identificar o sistema/ambiente, selecione os itens desejados.");
            }
        }

        public static bool validDB(string sistema, string ambiente)
        {
            return Configuration.GetConnectionString(getDescConf(sistema, ambiente)) != null;
        }
    }
}