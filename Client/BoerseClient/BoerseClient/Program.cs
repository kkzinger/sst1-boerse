﻿using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace BoerseClient
{
    static class Program
    {
        static string AllStocksURL = "http://boerse.fnord.cc/stock";
        static string OrdersURL = "http://boerse.fnord.cc/order?orderId=";
        static string PLACEOrderURL = "http://boerse.fnord.cc/order";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Stock Deal App of dieBank!");

                List<Customer> Customers = DataControl.Instance.LoadAllCustomers();
                while (Customers.Count <= 0)
                {
                    String FirstName = "";
                    String LastName = "";
                    Console.WriteLine("No customers found, please enter First Name and Last Name to create a customer: ");
                    while (FirstName.Length < 3)
                    {
                        Console.WriteLine("Enter First Name (atleast 3 characters): ");
                        FirstName = Console.ReadLine();
                    }

                    while (LastName.Length < 3)
                    {
                        Console.WriteLine("Enter Last Name (atleast 3 characters): ");
                        LastName = Console.ReadLine();
                    }

                    Customers.Add(new Customer(FirstName, LastName));
                }

                Console.WriteLine("Please choose a customer from the list or create a new customer: ");

                uint customercounter = 0;
                foreach (Customer _customer in Customers)
                {
                    Console.WriteLine("Customer Nr.: " + (customercounter++));
                    Console.WriteLine("Internal Customer Nr.: " + _customer.ID);
                    Console.WriteLine("First Name: " + _customer.FirstName);
                    Console.WriteLine("Last Name: " + _customer.LastName);
                }
                Console.WriteLine("Please enter the Customer Nr. (0-" + (--customercounter) + "): ");
                string CID_str = Console.ReadLine();

                uint CID = 0;



                while ((!uint.TryParse(CID_str, out CID)) || (CID < 0) || (CID > customercounter))
                {
                    Console.WriteLine("Please enter the Customer Nr. (0-" + (--customercounter) + "): ");
                    CID_str = Console.ReadLine();

                }
                CID = uint.Parse(CID_str);
                Console.WriteLine("Chosen Customer: ");
                Customer CC = DataControl.Instance.LoadAllCustomers()[(int)CID];
                Console.WriteLine("Customer Nr.: " + CID);
                Console.WriteLine("Internal Customer Nr.: " + CC.ID);
                Console.WriteLine("First Name: " + CC.FirstName);
                Console.WriteLine("Last Name: " + CC.LastName);

                List<Depot> DepotsOfCustomers = DataControl.Instance.GetDepotsByCustomer(CC);

                if (DepotsOfCustomers.Count > 0)
                {
                    Console.WriteLine("Depots: ");
                    foreach (Depot _D in DepotsOfCustomers)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine(_D.ID);
                        Console.WriteLine(_D.Owner);
                        foreach (KeyValuePair<Stock, int> _S in _D.Stocks)
                        {
                            Console.WriteLine(_S.Key.ID + " (" + _S.Value.ToString() + ")");
                        }
                        Console.WriteLine("--------------------------------------");

                    }
                    Console.WriteLine("TODO: CHOOSE DEPOT");
                }
                else
                {
                    Console.WriteLine(CC.FirstName + " " + CC.LastName + " has no Depots");
                    string CreateDepot = "";
                    while((CreateDepot != "yes")&& (CreateDepot != "no"))
                    {
                        Console.WriteLine("Create Depot? Enter yes or no: ");
                        CreateDepot = Console.ReadLine();
                    }

                    if(CreateDepot=="yes")
                    {
                        new Depot(CC);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            //Stock S1 = new Stock();
            //Stock S2 = new Stock();

            //Stock S3 = new Stock();
            //Stock S4 = new Stock();
            //Customer E = DataControl.Instance.LoadAllCustomers()[0];
            //Customer F = DataControl.Instance.LoadAllCustomers()[1];
            //Depot Depot = new Depot(E);
            //Depot.AddStock(S2, 2);
            //Depot.AddStock(S1, 1);
            //Console.WriteLine("TODO: Einlesen der gespeicherten Depots je Customer");
            //Depot Depot2 = new Depot(E);
            //List<KeyValuePair<Stock, int>> stockstoadd = new List<KeyValuePair<Stock,int>>();
            ////stockstoadd.Add(S1);
            //KeyValuePair<Stock, int> KPV1 = new KeyValuePair<Stock, int>(S3, 23);
            //KeyValuePair<Stock, int> KPV2 = new KeyValuePair<Stock, int>(S4, 4);

            //stockstoadd.Add(KPV1);
            //stockstoadd.Add(KPV2);

            //Depot2.AddStocks(stockstoadd);

            //List<Depot> DepotsVonF = DataControl.Instance.GetDepotsByCustomer(F);
            //if (DepotsVonF.Count > 0)
            //{
            //    foreach (Depot _D in DepotsVonF)
            //    {
            //        Console.WriteLine("--------------------------------------");
            //        Console.WriteLine(_D.ID);
            //        Console.WriteLine(_D.Owner);
            //        foreach (KeyValuePair<Stock, int> _S in _D.Stocks)
            //        {
            //            Console.WriteLine(_S.Key.ID + " (" + _S.Value.ToString() + ")");
            //        }
            //        Console.WriteLine("--------------------------------------");

            //    }
            //}
            //else
            //{
            //    Console.WriteLine(F.FirstName + " " + F.LastName + " has no Depots");
            //}

            //List<Depot> DepotsVonE = DataControl.Instance.GetDepotsByCustomer(E);
            //if (DepotsVonE.Count > 0)
            //{
            //    foreach (Depot _D in DepotsVonE)
            //    {
            //        Console.WriteLine("--------------------------------------");
            //        Console.WriteLine(_D.ID);
            //        Console.WriteLine(_D.Owner.ID);
            //        foreach (KeyValuePair<Stock, int> _S in _D.Stocks)
            //        {
            //            Console.WriteLine(_S.Key.ID + " (" + _S.Value.ToString() + ")");
            //        }
            //        Console.WriteLine("--------------------------------------");

            //    }
            //}
            //else
            //{
            //    Console.WriteLine(E.FirstName + " " + E.LastName + " has no Depots");
            //}



            //GetAllStocks();
            //GetAllOrders();
            //Order OD = new Order();
            //OD.timestamp = DateTimeOffset.Now;
            //OD.amount = 2;
            //OD.idBank = "dieBank";
            //OD.idBoerse = "datBoerse";
            //OD.idCustomer = "C-259261f5-21b0-486b-ba48-bad81dd3d824";
            //OD.idStock = "";
            //OD.price = 200d;
            //OD.signature = OD.idCustomer;
            //OD.type = "sell";

            //SendOrder(OD);

        }

        // Returns JSON string
        static string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        static Stock[] GetAllStocks()
        {
           Stock[] Stocks = JsonConvert.DeserializeObject<Stock[]>(GET(AllStocksURL));
            //foreach (Stock st in Stocks)
            //{
            //    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            //    Console.WriteLine("ID: " + st.ID);
            //    Console.WriteLine("Name: " + st.Name);
            //    Console.WriteLine("Price: " + st.Price);
            //    Console.WriteLine("Boerse: " + st.IDBOERSE);
            //    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");

            //}
            return Stocks;
        }

        static Order[] GetAllOrders()
        {
            Order[] Orders = JsonConvert.DeserializeObject<Order[]>(GET(OrdersURL));
            //foreach (Order od in Orders)
            //{
            //    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            //    Console.WriteLine("ID: " + od.id);
            //    Console.WriteLine("Time: "+od.timestamp);
            //    Console.WriteLine("Type: " + od.type);
            //    Console.WriteLine("Boerse: " + od.idBoerse);
            //    Console.WriteLine("Customer: " + od.idCustomer);
            //    Console.WriteLine("Stock: " + od.idStock);
            //    Console.WriteLine("Amount: " + od.amount);
            //    Console.WriteLine("Price: " + od.price);
            //    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");

            //}
            return Orders;
        }

        static void SendOrder(Order _order)
        {
            string serializedObject = JsonConvert.SerializeObject(_order);
            HttpWebRequest request = WebRequest.CreateHttp(PLACEOrderURL);
            Console.WriteLine(serializedObject);
            request.Method = "PUT";
            request.AllowWriteStreamBuffering = false;
            request.ContentType = "application/json";
            request.Accept = "Accept=application/json";
            request.SendChunked = false;
            request.ContentLength = serializedObject.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(serializedObject);
            }
            var response = request.GetResponse() as HttpWebResponse;

        }



    }







        }

