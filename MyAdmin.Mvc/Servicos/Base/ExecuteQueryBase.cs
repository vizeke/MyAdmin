using System;
using System.Collections.Generic;
using System.Data;

namespace MyAdmin.Mvc.Servicos.Base
{
    public abstract class ExecuteQueryBase
    {
        private readonly Common.Logging.ILog log = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected string _fileName;
        protected string _appPath;
        protected bool _saveFile;
        protected string _connString;

        public ExecuteQueryBase(string connString, bool saveFile = false, string fileName = "", string appPath = "")
        {
            _fileName = fileName;
            _appPath = appPath;
            _saveFile = saveFile;
            _connString = connString;
        }

        protected string ConnectionString
        {
            get { return _connString; }
        }

        public abstract object ExecuteQuery(string pQuery);

        public abstract object GetDBStructure();

        /// <summary>
        /// A partir de uma query qualquer, retorna a lista de comandos a ser executada
        /// </summary>
        /// <param name="pQuery">Texto da query</param>
        /// <returns>Lista de comandos que será executada separadamente</returns>
        protected List<String> ProcessQueryText(string pQuery)
        {
            bool isString = false;
            int indexAnterior = 0;
            int indexCont = 0;
            List<String> listOperacoes = new List<string>();

            pQuery = pQuery.Trim();

            if (string.IsNullOrEmpty(pQuery))
                return listOperacoes;

            string strSQL = pQuery;

            for (int j = 0; j < strSQL.Length; j++)
            {
                indexCont++;
                char chrAtual = strSQL[j];
                if (chrAtual.Equals('\''))
                {
                    isString = !isString;
                }
                else if (chrAtual.Equals(';'))
                {
                    if (!isString)
                    {
                        listOperacoes.Add(strSQL.Substring(indexAnterior, indexCont - 1).Replace('\n', ' ').Replace("\r", string.Empty));

                        indexCont = 0;
                        indexAnterior = j + 1;
                    }
                }
                else if (chrAtual.Equals('-'))
                {
                    if (!isString)
                    {
                        if (strSQL[j + 1].Equals('-'))
                        {
                            int nextLineBreak = strSQL.IndexOf('\n', j + 2);
                            if (nextLineBreak == -1)
                            {
                                strSQL.Remove(j);

                            }
                            else
                            {
                                strSQL = strSQL.Remove(j, nextLineBreak - j + 1);
                            }

                            indexCont--;
                            j--;
                        }
                    }
                }
                else if (chrAtual.Equals('/'))
                {
                    if (!isString)
                    {
                        if (strSQL[j + 1].Equals('*'))
                        {
                            int nextCloseBlockComment = strSQL.IndexOf("*/", j + 2);
                            if (nextCloseBlockComment == -1)
                            {
                                strSQL.Remove(j);

                            }
                            else
                            {
                                strSQL = strSQL.Remove(j, nextCloseBlockComment - j + 2);
                            }

                            indexCont--;
                            j--;
                        }
                    }
                }
            }

            if (!strSQL[strSQL.Length - 1].Equals(';'))
            {
                listOperacoes.Add(strSQL.Substring(indexAnterior, indexCont));
            }

            return listOperacoes;
        }

        public IEnumerable<Dictionary<string, object>> SerializeDataTable(DataTable dt)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            foreach (DataColumn col in dt.Columns)
                cols.Add(col.ColumnName);

            foreach (DataRow row in dt.Rows)
            {
                results.Add(SerializeRow(cols, row));
            }
            return results;
        }

        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, DataRow row)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
            {
                result.Add(col, row[col]);
            }
            return result;
        }

        protected class InfoDataBase
        {
            public InfoDataBase()
            {
                Schemas = new List<Schema>();
            }

            public List<Schema> Schemas { get; set; }

            public class Schema
            {
                public string Name { get; set; }
                public List<Tabela> Tabelas { get; set; }
            }

            public class Tabela
            {
                public string Schema { get; set; }
                public string Nome { get; set; }
                public List<Coluna> Colunas { get; set; }
            }

            public class Coluna
            {
                public string Nome { get; set; }
                public string Type { get; set; }
                public int Size { get; set; }
                public bool Nullable { get; set; }

                public string DescNullable
                {
                    get { return (Nullable ? "" : "not ") + "null"; }
                }
            }
        }
    }
}