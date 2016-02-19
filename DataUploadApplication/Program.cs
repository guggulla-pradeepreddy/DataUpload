using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Xml;

namespace DataUploadApplication
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {                
                DirectoryInfo d = new DirectoryInfo(@"C:\Data");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
               // string str = "";
                foreach (FileInfo file in Files)
                {
                    XmlDocument doc = new XmlDocument();
                    string xml = File.ReadAllText("C:\\Data\\"+file.Name);
                    if (!File.Exists("C:\\NewData\\" + file.Name))
                    {
                        string tableName = file.Name;
                        tableName = tableName.Replace(".xml", "");
                        xml = xml.Replace("\"", "'");
                        xml = xml.Replace("urn:schemas-microsoft-com:xml-msdata", "urn:schemas-microsoft-com:mapping-schema");
                        xml = xml.Replace("<xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:MainDataTable='"+tableName+"' msdata:Locale='en-US'>", "");
                        xml = xml.Replace("<xs:complexType>\r\n      <xs:choice minOccurs='0' maxOccurs='unbounded'>", "");
                        //xml = xml.Replace("<xs:choice minOccurs='0' maxOccurs='unbounded'>", "");
                        xml = xml.Replace("msdata:CaseSensitive='False'", "");
                        xml = xml.Replace("msdata:Locale='en-US'", "");
                        xml = xml.Replace("</xs:choice>\r\n    </xs:complexType>\r\n  </xs:element>", "");
                        //xml = xml.Replace("</xs:complexType>", "");
                        //xml = xml.Replace("</xs:element>", "");
                        doc.LoadXml(xml);
                        doc.Save("C:\\NewData\\" + file.Name);
                    }
                    SQLXMLBULKLOADLib.SQLXMLBulkLoad objBL = new SQLXMLBULKLOADLib.SQLXMLBulkLoad();
                    objBL.ErrorLogFile = "error.xml";

                    objBL.ConnectionString = "provider=sqloledb;server=DESKTOP-0NVP0AO;database=DataUploads;User ID=sa;Password=gv23;";
                    objBL.SchemaGen = true;
                    objBL.SGDropTables = true;
                    objBL.KeepIdentity = false;
                    objBL.BulkLoad = false;
                    objBL.Execute("C:\\NewData\\" + file.Name, "Data.xml");                     
                }
                Console.ReadLine();
            
                
            }
            catch (Exception e)
            {
              Console.WriteLine(e.ToString());
            }

            

        //static void Main(string[] args)
        //{
        //    using (SqlConnection conn = new SqlConnection())
        //    {
        //        conn.ConnectionString = ConfigurationManager.ConnectionStrings["DUDB"].ToString();
        //        conn.Open();
        //        var query = "Create table Artrial(ltype int,vendor int,transact int,type varchar(50),ptransact varchar(50),tdate datetime,adate datetime,checkno varchar(50),iamt decimal,pamt decimal,_current decimal,d30 decimal,d60 decimal,d90 decimal,d120 decimal,payamt decimal,checkamt decimal,pay bit,note varchar(50),msg varchar(50),document varchar(50))";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        cmd.ExecuteNonQuery();
               
        //    }
        }
    }
}
