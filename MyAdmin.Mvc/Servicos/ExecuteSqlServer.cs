using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MyAdmin.Mvc.Servicos.Base;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

namespace MyAdmin.Mvc.Servicos
{
    public class ExecuteSqlServer : ExecuteQueryBase, IExecuteQuery
    {
        private string _strResult;
        private int _fileCount = 0;
        private bool _fileSaved = false;

        public ExecuteSqlServer(string connString, bool saveFile = false, string fileName = "", string appPath = "")
            : base(connString, saveFile, fileName, appPath)
        {

        }

        public override object ExecuteQuery(string pQuery)
        {
            List<object> dataTables = new List<object>();

            Regex regex = new Regex(@"[\r\n\s]+[gG][oO][\s\r\n]+");

            MatchCollection matches = regex.Matches(pQuery);

            int prevIndex = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();

                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match m = matches[i];

                        ExecuteQueryCore(pQuery, dataTables, prevIndex, conn, m.Index - prevIndex);

                        prevIndex = m.Index + m.Length;
                    }

                    ExecuteQueryCore(pQuery, dataTables, prevIndex, conn, pQuery.Length - prevIndex);
                }
            }
            catch (SqlException ex)
            {
                _strResult += "The following error occured while executing the query:<br>\n" +
                    String.Format("Server: Msg {0}, Level {1}, State {2}, Line {3}<br>\n", new object[] { ex.Number, ex.Class, ex.State, ex.LineNumber }) +
                    System.Web.HttpContext.Current.Server.HtmlEncode(ex.Message).Replace("\n", "<br>") + "<br>\n";
            }
            catch (Exception ex)
            {
                _strResult += ex.Message + System.Environment.NewLine;
            }

            return new
            {
                msg = _strResult,
                dataTables = dataTables,
                fileSaved = _fileSaved
            };
        }

        private void ExecuteQueryCore(string pQuery, List<object> dataTables, int prevIndex, SqlConnection conn, int nextIndex)
        {
            string tquery;
            tquery = pQuery.Substring(prevIndex, nextIndex);

            if (tquery.Trim().Length > 0)
            {

                SqlCommand comm = new SqlCommand(tquery, conn);
                var dr = comm.ExecuteReader();
                do
                {
                    if (_saveFile)
                    {
                        dataTables.Add(SqlExportFile(dr));
                    }
                    else
                    {
                        dataTables.Add(this.SqlSerialize(dr));
                    }
                } while (dr.NextResult());

                if (dr.RecordsAffected != -1)
                    _strResult += string.Format("({0} row(s) affected) <br/>", dr.RecordsAffected);

                if (!dr.IsClosed)
                    dr.Close();
            }
        }

        private string SqlExportFile(SqlDataReader dr)
        {
            Exportador export = new Exportador();
            string fileName = "App_Data\\tmp\\" + _fileName + "_" + _fileCount + ".csv";
            string fullFileName = _appPath + fileName;
            export.exportToCSVfile(fullFileName, dr);

            _strResult += "Arquivo " + fileName + " gerado com sucesso. <br/>";
            _fileSaved = true;
            _fileCount++;
            return fileName;
        }

        private object SqlSerialize(SqlDataReader reader)
        {
            var datatable = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            var rowCount = 0;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                cols.Add(GetColumnName(cols, reader.GetName(i)));
            }

            while (reader.Read())
            {
                rowCount++;
                datatable.Add(SerializeRow(cols, reader));
            }

            return new
            {
                rowsAffected = rowCount,
                dataTable = datatable
            };
        }

        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            for (int i = 0; i < cols.Count(); i++)
            {
                result.Add(cols.ElementAt(i), reader[i]);
            }
            return result;
        }

        private string GetColumnName(List<string> cols, string pName, int index = 1)
        {
            var name = pName;
            foreach (string col in cols)
            {
                if (col == name)
                {
                    name = index == 1 ? name + index.ToString() : name.Substring(0, name.Length - (index - 1).ToString().Length) + index.ToString();
                    return GetColumnName(cols, name, ++index);
                }
            }
            return name;
        }

        public override object GetDBStructure()
        {
            InfoDataBase dataResult = new InfoDataBase();

            string structSQL = @"select so.name as tableName, sc.name as columnName, st.name as type, sc.max_length as size, sc.is_nullable
                                 from sys.objects so
                                 inner join sys.columns sc on sc.object_id = so.object_id
                                 inner join sys.types st on st.system_type_id = sc.system_type_id
                                 where so.type = 'U' 
                                 order by so.name";

            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand comm = new SqlCommand(structSQL, conn);
                    using (SqlDataReader dsResult = comm.ExecuteReader())
                    {
                        string table = "";
                        while (dsResult.Read())
                        {
                            string currentTable = dsResult["tableName"].ToString();
                            string currentColumn = dsResult["columnName"].ToString();
                            string currentType = dsResult["type"].ToString();
                            int currentSize = Convert.ToInt32(dsResult["size"]);
                            bool currentNullable = Convert.ToBoolean(dsResult["is_nullable"]);

                            if (table != currentTable)
                            {
                                dataResult.Tabelas.Add(new InfoDataBase.Tabela() { Nome = currentTable, Colunas = new List<InfoDataBase.Coluna>() });
                            }

                            dataResult.Tabelas[dataResult.Tabelas.Count - 1].Colunas
                                .Add(new InfoDataBase.Coluna()
                                {
                                    Nome = currentColumn,
                                    Type = currentType,
                                    Size = currentSize,
                                    Nullable = currentNullable
                                });

                            table = currentTable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _strResult += ex.Message;
                }
            }

            return new
            {
                msg = _strResult,
                data = dataResult
            };
        }
    }
}