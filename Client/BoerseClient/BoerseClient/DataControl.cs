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
        private static string DepotStorage = "../../data /depots.xml";
        private static string StockStorage = "../../data /stocks.xml";
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
                    xmlWriter.WriteElementString("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
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
                   new XElement("LastName", _C.LastName),
                   new XElement("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())));
                xDocument.Save(CustomerStorage);
            }




        }

        public List<Customer> LoadAllCustomers()
        {
            try
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
            catch(Exception)
            {
                return new List<Customer>();
            }
        }

        public void SaveDepot(Depot _D)
        {
            if (!File.Exists(DepotStorage))
            {


                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(DepotStorage, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Depots");

                    xmlWriter.WriteStartElement("Depot");
                    xmlWriter.WriteElementString("DID", _D.ID);
                    xmlWriter.WriteElementString("Owner", _D.Owner.ID);
                    xmlWriter.WriteStartElement("Stocks");
                    

                    foreach (KeyValuePair<Stock,int> _s in _D.Stocks)
                    {
                        xmlWriter.WriteStartElement("Stock");
                        xmlWriter.WriteAttributeString("SID", _s.Key.ID);
                        xmlWriter.WriteAttributeString("Amount", _s.Value.ToString());
                        xmlWriter.WriteEndElement();
                                          
                    }
                    
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteElementString("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            else
            {
                System.Xml.Linq.XDocument xDocument = XDocument.Load(DepotStorage);
                XElement root = xDocument.Element("Depots");
                IEnumerable<XElement> rows = root.Descendants("Depot");
                IEnumerable<XElement> stockrows = rows.Descendants("Stocks");
                XElement firstRow = rows.First();
                //firstRow.AddBeforeSelf(
                //   new XElement("Depot",
                //   new XElement("DID", _D.ID),
                //   new XElement("Owner", _D.Owner.ID),
                //   new XElement("Stocks", 
                //   _D.Stocks.Select( stock => new XElement("kjasddkljsdafkj", stock.Key.ID) ),
                //   _D.Stocks.Select(stock => new XAttribute("Amount", stock.Value.ToString()))),
                //   new XElement("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())));
                firstRow.AddBeforeSelf(
                   new XElement("Depot",
                   new XElement("DID", _D.ID),
                   new XElement("Owner", _D.Owner.ID),
                   new XElement("Stocks",
                   _D.Stocks.Select(stock => new XElement("Stock",
                  new XAttribute("SID", stock.Key.ID),
                  new XAttribute("Amount",stock.Value.ToString())))),
                   new XElement("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString())));


                xDocument.Save(DepotStorage);
            }




        }


        public void SaveStocksToDepot(Depot _Depot, List<KeyValuePair<Stock, int>> _Stocks)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DepotStorage);
            XmlNodeList DepotOwnerNodes = xmlDoc.SelectNodes("//Depots/Depot/Owner");
            foreach (XmlNode DepotOwnerNode in DepotOwnerNodes)
            {

                if ((DepotOwnerNode.InnerText == _Depot.Owner.ID.ToString())&&(DepotOwnerNode.PreviousSibling.InnerText == _Depot.ID.ToString()))
                {
                  
                    foreach (KeyValuePair<Stock, int> s in _Stocks)
                    {
                        XmlAttribute sid = xmlDoc.CreateAttribute("SID");
                        sid.Value = s.Key.ID;

                        XmlAttribute amount = xmlDoc.CreateAttribute("Amount");
                        amount.Value = s.Value.ToString();

                        XmlElement EStock = xmlDoc.CreateElement("Stock");
                        EStock.Attributes.Append(sid);
                        EStock.Attributes.Append(amount);
                        DepotOwnerNode.NextSibling.AppendChild(EStock);
                    }
                }
            }
            xmlDoc.Save(DepotStorage);

        }

    public List<Depot> GetDepotsByCustomer(Customer _C)
    {
            try
            {
                List<Customer> _Customers = LoadAllCustomers();

                XDocument doc = XDocument.Load(DepotStorage);
                List<Depot> Depots = (from xnode in doc.Element("Depots").Elements("Depot")
                                      select new Depot()
                                      {
                                          ID = xnode.Element("DID").Value.ToString(),
                                          Owner = _Customers.Find(Customer => Customer.ID == xnode.Element("Owner").Value.ToString()),
                                          Stocks =
                                             (from xnodestock in xnode.Elements("Stocks").Elements("Stock")
                                              select new KeyValuePair<Stock, int>(new Stock(xnodestock.Attribute("SID").Value.ToString()), int.Parse(xnodestock.Attribute("Amount").Value.ToString()))
                                              ).ToList()
                                      }).ToList();
                List<Depot> DepotsByCustomer = new List<Depot>();
                foreach(Depot d in Depots)
                {
                    if(d.Owner.ID == _C.ID)
                    {
                        DepotsByCustomer.Add(d);
                    }

                }
                

                return DepotsByCustomer;
        }
            catch (Exception)
            {
                return new List<Depot>();
            }
}

   


    }
}
