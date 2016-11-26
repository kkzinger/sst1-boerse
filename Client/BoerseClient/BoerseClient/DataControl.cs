using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BoerseClient
{
    public class DataControl
    {
        private static DataControl instance;
        private static string CustomerStorage = "../../data /customers.xml";
        private DataControl() { }

        public static DataControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataControl();
                }
                return instance;
            }
        }
      
      
      public  void SaveCustomer(Customer _C)
        {
            if (!File.Exists(CustomerStorage))
            {
               

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(CustomerStorage, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Customers");

                    xmlWriter.WriteStartElement("Customer");
                    xmlWriter.WriteElementString("CID", _C.ID);
                    xmlWriter.WriteElementString("FirstName", _C.FirstName);
                    xmlWriter.WriteElementString("LastName", _C.LastName);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            else
            {
                System.Xml.Linq.XDocument xDocument = XDocument.Load(CustomerStorage);
                XElement root = xDocument.Element("Customers");
                IEnumerable<XElement> rows = root.Descendants("Customer");
                XElement firstRow = rows.First();
                firstRow.AddBeforeSelf(
                   new XElement("Customer",
                   new XElement("CID", _C.ID),
                   new XElement("FirstName", _C.FirstName),
                   new XElement("LastName", _C.LastName)));
                xDocument.Save(CustomerStorage);
            }




        }

        public List<Customer> LoadAllCustomers()
        {
            XDocument doc = XDocument.Load(CustomerStorage);
            List<Customer> customers = (from xnode in doc.Element("Customers").Elements("Customer")
                                        select new Customer()
                                        {
                                            ID = xnode.Element("CID").Value.ToString(),
                                            FirstName = xnode.Element("FirstName").Value.ToString(),
                                            LastName = xnode.Element("LastName").Value.ToString()
                                        }).ToList();
            return customers;
        }


    }
}
