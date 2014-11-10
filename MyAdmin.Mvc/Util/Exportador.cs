using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace MyAdmin.Mvc
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
        private string separator
        {
            get { return ";"; }
        }

        private string columnNames(DataTable dtSchemaTable, string delimiter)
        {
            string strOut = "";

            for (int i = 0; i < dtSchemaTable.Rows.Count; i++)
            {
                strOut += dtSchemaTable.Rows[i][0].ToString();
                if (i < dtSchemaTable.Rows.Count - 1)
                    strOut += delimiter;
            }
            return strOut;
        }

        public void exportToCSVfile(string fileOut, SqlDataReader dr)
        {
            // Creates the CSV file as a stream, using the given encoding.
            StreamWriter sw = new StreamWriter(fileOut, false, Encoding.UTF8);
            try
            {
                // Write the data into the file
                sw.Write(generateCSVString(dr));
                // Closes the text stream
                sw.Close();
            }catch(Exception)
            {
                sw.Close();
                throw;
            }
        }

        public string generateCSVString(SqlDataReader dr)
        {
            // Retrieves the schema of the table.
            DataTable dtSchema = dr.GetSchemaTable();

            // Declares a StringBuilder to mount the file
            StringBuilder sb = new StringBuilder();

            // represents a full row
            string strRow;

            // Writes the column headers if the user previously asked that.
            sb.AppendLine(columnNames(dtSchema, this.separator));

            // Reads the rows one by one from the SqlDataReader
            // transfers them to a string with the given separator character and
            // writes it to the file.
            while (dr.Read())
            {
                strRow = "";
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    strRow += dr[i].ToString().Replace(this.separator, ",");
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