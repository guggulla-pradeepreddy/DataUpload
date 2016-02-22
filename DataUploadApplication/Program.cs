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
        //function to replace the datatype of a xml column  to sql column type
        public string getType(string type)
        {
            if (type == "xs:int")
            {
                return "[int]";
            }
            else if (type == "xs:string" || type == "xs:base64Binary")
            {
                return "[nvarchar](1000)";
            }
            else if (type == "xs:decimal")
            {
                return "[numeric](18,0)";
            }
            else if (type == "xs:dateTime")
            {
                return "[datetime]";
            }
            else if (type == "xs:boolean")
            {
                return "[bit]";
            }
            else
            {
                return "";
            }
        }

        [STAThread]
        static void Main(string[] args)
        {


            try
            {
               // string connString = "Data Source=DESKTOP-G6M2CKB;Initial Catalog=Tdb2;User ID=sa;Password=sqlserver;";
                //Code to read list of files from directory
                DirectoryInfo d = new DirectoryInfo(@"C:\Data");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
                // string str = "";
                foreach (FileInfo file in Files)
                {
                    XmlDocument doc = new XmlDocument();
                    string xml = File.ReadAllText("C:\\Data\\" + file.Name);
                    if (File.Exists("C:\\NewData\\" + file.Name))
                    {
                        //delete existing file
                        File.Delete("C:\\NewData\\" + file.Name);
                    }

                    string tableName = file.Name;
                    tableName = tableName.Replace(".xml", "");
                    xml = xml.Replace("\"", "'");
                    xml = xml.Replace("urn:schemas-microsoft-com:xml-msdata", "urn:schemas-microsoft-com:mapping-schema");
                    xml = xml.Replace("<xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:MainDataTable='" + tableName + "' msdata:Locale='en-US'>", "");
                    xml = xml.Replace("<xs:complexType>\r\n      <xs:choice minOccurs='0' maxOccurs='unbounded'>", "");
                    //xml = xml.Replace("<xs:choice minOccurs='0' maxOccurs='unbounded'>", "");
                    xml = xml.Replace("msdata:CaseSensitive='False'", "");
                    xml = xml.Replace("msdata:Locale='en-US'", "");
                    xml = xml.Replace("</xs:choice>\r\n    </xs:complexType>\r\n  </xs:element>", "");
                    //xml = xml.Replace("</xs:complexType>", "");
                    //xml = xml.Replace("</xs:element>", "");
                    doc.LoadXml(xml);

                    //code to save modified xml
                    doc.Save("C:\\NewData\\" + file.Name);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("C:\\NewData\\" + file.Name);
                
                    //processing the modified xml to form the query
                    //XmlNodeList parentNode = xmlDoc.GetElementsByTagName("xs:sequence");
                    //string tableName = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes["name"].Value + "sds";
                    string query1 = "IF EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = '" + tableName + "') DROP TABLE [dbo].[" + tableName + "];";
                    string query = "CREATE TABLE [dbo].[" + tableName + "]" + "(";
                    string columns = "";
                    //foreach (XmlNode childNode in parentNode.ChildNodes)
                    
                    foreach (XmlNode childNode in xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes)
                    {
                        string column = "";
                        string cName = childNode.Attributes["name"].Value;
                        string cType = new Program().getType(childNode.Attributes["type"].Value);
                        column = "[" + cName + "]" + cType + ",";
                        columns = columns + column;

                    }
                    query = query + columns + ")";
                    
                    //running the created query
                    using (SqlConnection conn = new SqlConnection())
                    {
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["DUDB"].ToString();
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query1, conn);
                        cmd.ExecuteNonQuery();
                        cmd = new SqlCommand(query, conn);
                        cmd.ExecuteNonQuery();
                    }

                    //SQLXMLBULKLOADLib.SQLXMLBulkLoad objBL = new SQLXMLBULKLOADLib.SQLXMLBulkLoad();
                    //objBL.ErrorLogFile = "error.xml";

                    //objBL.ConnectionString = "provider=sqloledb;server=DESKTOP-0NVP0AO;database=Tdb2;User ID=sa;Password=gv23;";
                    //objBL.SchemaGen = true;
                    //objBL.SGDropTables = true;
                    //objBL.KeepIdentity = false;
                    //objBL.BulkLoad = false;
                    //objBL.Execute("C:\\NewData\\" + file.Name, "Data.xml");
                }
                // xmlDoc.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

       }
}
