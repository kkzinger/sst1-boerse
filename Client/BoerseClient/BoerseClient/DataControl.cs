﻿using System;
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
                    

                    foreach (KeyValuePair<Stock,uint> _s in _D.Stocks)
                    {
                        xmlWriter.WriteStartElement("Stock");
                        xmlWriter.WriteAttributeString("SID", _s.Key.ID);
                        xmlWriter.WriteAttributeString("Amount", _s.Value.ToString());
                        xmlWriter.WriteEndElement();
                                          
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("IssuedOrders");


                    foreach (Order _o in _D.IssuedOrders)
                    {
                        xmlWriter.WriteStartElement("IssuedOrder");
                        xmlWriter.WriteAttributeString("OID", _o.id);
                        xmlWriter.WriteAttributeString("BoerseID", _o.idBoerse);
                        xmlWriter.WriteAttributeString("Type", _o.type);
                        xmlWriter.WriteAttributeString("Amount", _o.amount.ToString());
                        xmlWriter.WriteEndElement();

                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteElementString("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                    xmlWriter.WriteElementString("Worth", _D.Worth.ToString());
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
                    new XElement("IssuedOrders",
                   _D.IssuedOrders.Select(order => new XElement("IssuedOrder",
                  new XAttribute("OID", order.id),
                  new XAttribute("BoerseID", order.idBoerse),
                  new XAttribute("Type", order.type),
                  new XAttribute("Amount", order.amount)))),
                   new XElement("TimeStamp", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()),
                   new XElement("Worth", _D.Worth.ToString())));


                xDocument.Save(DepotStorage);
            }




        }


        public void SaveStocksToDepot(Depot _Depot, List<KeyValuePair<Stock, uint>> _Stocks)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DepotStorage);
            XmlNodeList DepotOwnerNodes = xmlDoc.SelectNodes("//Depots/Depot/Owner");
            foreach (XmlNode DepotOwnerNode in DepotOwnerNodes)
            {

                if ((DepotOwnerNode.InnerText == _Depot.Owner.ID.ToString())&&(DepotOwnerNode.PreviousSibling.InnerText == _Depot.ID.ToString()))
                {

                        foreach (KeyValuePair<Stock, uint> s in _Stocks)
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

        public void UpdateStockInDepot(Depot _Depot, KeyValuePair<Stock, uint> _Stock, int _diff)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DepotStorage);
            XmlNodeList DepotOwnerNodes = xmlDoc.SelectNodes("//Depots/Depot/Owner");
            foreach (XmlNode DepotOwnerNode in DepotOwnerNodes)
            {

                if ((DepotOwnerNode.InnerText == _Depot.Owner.ID.ToString()) && (DepotOwnerNode.PreviousSibling.InnerText == _Depot.ID.ToString()))
                {
                    XmlNode EStock = DepotOwnerNode.NextSibling;
                    bool updated = false;
                    foreach (XmlNode StockNode in EStock)
                    {
                            if ((StockNode.Attributes["SID"].Value.ToString() == _Stock.Key.ID) && (uint.Parse(StockNode.Attributes["Amount"].Value.ToString() )== _Stock.Value))
                            {
 
                            int _new_amt = (int)_Stock.Value - _diff;
                                if ((_new_amt > 0)&&(updated == false))
                                {
                                    StockNode.Attributes["Amount"].Value = _new_amt.ToString();
                                    updated = true;
                                }
                                else if ((_new_amt <= 0) && (updated == false))
                                {
                                     EStock.RemoveChild(StockNode);
                                     updated = true;
                                }   
                            }
                        
                    }

                }
            }

            xmlDoc.Save(DepotStorage);
        }




        public void UpdateWorthInDepot(Depot _Depot, double _new_worth)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DepotStorage);
            XmlNodeList DepotOwnerNodes = xmlDoc.SelectNodes("//Depots/Depot/Owner");
            foreach (XmlNode DepotOwnerNode in DepotOwnerNodes)
            {

                if ((DepotOwnerNode.InnerText == _Depot.Owner.ID.ToString()) && (DepotOwnerNode.PreviousSibling.InnerText == _Depot.ID.ToString()))
                {
                    XmlNode EStock = DepotOwnerNode.NextSibling;
                    XmlNode EOrder = EStock.NextSibling;
                    XmlNode ETime = EOrder.NextSibling;
                    XmlNode EWorth = ETime.NextSibling;

                    foreach (XmlNode WorthNode in EWorth)
                    {
                        WorthNode.InnerText = _new_worth.ToString();

                    }

                }
            }

            xmlDoc.Save(DepotStorage);
        }






        public void SaveIssuedOrderToDepot(Depot _Depot, Order _Order)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(DepotStorage);
            XmlNodeList DepotOwnerNodes = xmlDoc.SelectNodes("//Depots/Depot/Owner");
            foreach (XmlNode DepotOwnerNode in DepotOwnerNodes)
            {

                if ((DepotOwnerNode.InnerText == _Depot.Owner.ID.ToString()) && (DepotOwnerNode.PreviousSibling.InnerText == _Depot.ID.ToString()))
                {

                    if (_Order.amount > 0)
                    {
                        XmlAttribute oid = xmlDoc.CreateAttribute("OID");
                        oid.Value = _Order.id;

                        XmlAttribute boerseid = xmlDoc.CreateAttribute("BoerseID");
                        boerseid.Value = _Order.idBoerse;

                        XmlAttribute type = xmlDoc.CreateAttribute("Type");
                        type.Value = _Order.type;

                        XmlAttribute amount = xmlDoc.CreateAttribute("Amount");
                        amount.Value = _Order.amount.ToString();

                        XmlElement EOrder = xmlDoc.CreateElement("IssuedOrder");
                        EOrder.Attributes.Append(oid);
                        EOrder.Attributes.Append(boerseid);
                        EOrder.Attributes.Append(type);
                        EOrder.Attributes.Append(amount);
                        XmlNode EStock = DepotOwnerNode.NextSibling;
                        EStock.NextSibling.AppendChild(EOrder);
                    }
                    else
                    {
                        XmlNode EStock = DepotOwnerNode.NextSibling;
                        XmlNode EOrder = EStock.NextSibling;
                        foreach (XmlNode OrderNode in EOrder)
                        {
                            if(OrderNode.Attributes["OID"].Value.ToString()==_Order.id)
                            {
                                EOrder.RemoveChild(OrderNode);
                            }
                        }
                           
                    }
                    
                }
            }
            xmlDoc.Save(DepotStorage);

        }

        public List<Depot> GetDepotsByCustomer(Customer _C, Order[] _orders)
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
                                              select new KeyValuePair<Stock, uint>(new Stock(xnodestock.Attribute("SID").Value.ToString()), uint.Parse(xnodestock.Attribute("Amount").Value.ToString()))
                                              ).ToList(),
                                          IssuedOrders =
                                             (from xnodeorder in xnode.Elements("IssuedOrders").Elements("IssuedOrder")
                                              select new Order(xnodeorder.Attribute("OID").Value.ToString(), _orders, xnodeorder.Attribute("Amount").Value.ToString())
                                              ).ToList(),
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
