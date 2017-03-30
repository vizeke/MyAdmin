using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace MyAdmin.Application.Services
{
    public class Exportador
    {
        /*public void ExportarRelatorioParaArquivoCsv()
        {
            SRE_DAL sre_dal = new SRE_DAL();

            List<SRE_info> listaSre = sre_dal.ObterTodos();

            foreach (SRE_info sre in listaSre)
            {
                SqlDataReader dr = sre_dal.ObterRelatorio(sre.ID_SRE);

                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Exportador)).Location;
                this.exportToCSVfile(fullPath + sre.NOME + ".csv", dr);

                dr.Close();
            }
        }

        public string GerarStringCSV_porSRE(int ID_SRE)
        {
            SRE_DAL sre_dal = new SRE_DAL();
            SqlDataReader dr = sre_dal.ObterRelatorio(ID_SRE);
            string resultado = generateCSVString(dr);
            dr.Close();
            return resultado;
        }
        */

        /// <summary>
        /// TODO: Deixar o separador e o replace que ser� utilizado caso exista um separador no campo como par�metros do sistema
        /// caso n�o exista parametros utilizar o padr�o ';' e ',' 
        /// </summary>
        private string separator
        {
            get { return ";"; }
        }

        private string replacer
        {
            get
            {
                return ",";
            }
        }

        private string columnNames(SqlDataReader dr, string delimiter)
        {
            string strOut = "";
            int columnCount = dr.FieldCount;
            for (int i = 0; i < columnCount; i++)
            {
                strOut += dr.GetName(i);
                if (i < columnCount - 1) { strOut += delimiter; }
            }
            return strOut;
        }

        public void exportToCSVfile(string fileOut, SqlDataReader dr)
        {
            // Creates the CSV file as a stream, using the given encoding.
            using (StreamWriter sw = new StreamWriter(File.Create(fileOut), Encoding.UTF8))
            {
                // Write the data into the file
                sw.Write(generateCSVString(dr));
            }
        }

        public string generateCSVString(SqlDataReader dr)
        {
            // Declares a StringBuilder to mount the file
            StringBuilder sb = new StringBuilder();

            // represents a full row
            string strRow;

            // Reads the rows one by one from the SqlDataReader
            // transfers them to a string with the given separator character and
            // writes it to the file.
            bool firstLine = true;
            while (dr.Read())
            {
                if (firstLine)
                {
                    sb.AppendLine(columnNames(dr, this.separator));
                    firstLine = false;
                }
                strRow = "";
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    strRow += dr[i].ToString().Replace(this.separator, this.replacer);
                    if (i < dr.FieldCount - 1)
                        strRow += this.separator;
                }
                sb.AppendLine(strRow);
            }

            // returns the string
            return sb.ToString();
        }
    }
}