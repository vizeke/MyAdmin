using System;
using System.Data.OracleClient;
using System.Text;
using MyAdmin.Mvc.Servicos.Base;

namespace MyAdmin.Mvc.Servicos
{
    public class ExecuteOracle : ExecuteQueryBase
    {
        public ExecuteOracle(string connString, bool saveFile = false, string fileName = "", string appPath = "")
            : base(connString, saveFile, fileName, appPath)
        {

        }

        public override object ExecuteQuery(string pQuery)
        {
            using (OracleConnection objConn = new OracleConnection(this.ConnectionString))
            {
                //fazer uma tentativa para filtrar o resultado ainda na funcao do siarhes
                //retornar a quantidade total de registros. talvez retornar menos..
                //ex: existem 10000. retornar apenas 100 e informar para melhorar a busca
                //configuro a conexao

                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objConn;
                objCmd.CommandText = pQuery;
                objCmd.CommandType = System.Data.CommandType.Text;

                //OracleCommand objCmd = new OracleCommand();
                //objCmd.Connection = objConn;
                //objCmd.CommandText = "GET_DADOS_PM";
                //objCmd.CommandType = CommandType.StoredProcedure;

                //objCmd.Parameters.Add("filtro_rg", System.Data.OracleClient.OracleType.VarChar).Value = "212842";
                //objCmd.Parameters.Add("filtro_nome", System.Data.OracleClient.OracleType.VarChar).Value = "";
                //objCmd.Parameters.Add("c_result", OracleType.Cursor).Direction = ParameterDirection.ReturnValue;

                StringBuilder sbResponse = new StringBuilder();
                string strResult = string.Empty;
                try
                {
                    objConn.Open();
                    OracleDataReader dsResult = objCmd.ExecuteReader();

                    //GridView gv = new GridView();
                    //gv.ID = "gvDados";// + j.ToString();
                    //gv.DataSource = dsResult;
                    //gv.DataBind();
                    //sbResponse.Append(this.GridViewToString(gv));
                }

                catch (OracleException ex)
                {
                    strResult += ex.Message + System.Environment.NewLine;
                }
                finally
                {
                    objConn.Close();
                }

                return new
                {
                    msg = strResult,
                    data = sbResponse.ToString()
                };
            }
        }


        public override object GetDBStructure()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}