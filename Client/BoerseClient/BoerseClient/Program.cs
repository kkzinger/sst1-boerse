using System;
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

        static void Main(string[] args)
        {


            //Customer C = new Customer("Tobi","Mayer");
            //Customer D = new Customer("Gerold", "Katzinger");
            //Customer E = new Customer("Hubert Alois", "O'Donnell");
            //Customer F = new Customer("Josef \"Sepp\"", "Mayr-Huber");

            //foreach(Customer _customer in DataControl.Instance.LoadAllCustomers())
            //{
            //    Console.WriteLine(_customer.ID);
            //    Console.WriteLine(_customer.FirstName);
            //    Console.WriteLine(_customer.LastName);
            ////}
            //Stock S1 = new Stock();
            //Stock S2 = new Stock();
            //Stock S3 = new Stock();
            //Stock S4 = new Stock();

            //Depot Depot = new Depot(E);
            //Depot.AddStock(S2);
            //Depot Depot2 = new Depot(F);
            //List<Stock> stockstoadd = new List<Stock>();
            //stockstoadd.Add(S1);
            //stockstoadd.Add(S3);
            //stockstoadd.Add(S4);
            //Depot2.AddStocks(stockstoadd);

            GetAllStocks();



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

        static void GetAllStocks()
        {
            Stock[] Stocks = JsonConvert.DeserializeObject<Stock[]>(GET(AllStocksURL));
            foreach (Stock st in Stocks)
            {
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("ID: " + st.ID);
                Console.WriteLine("Name: " + st.Name);
                Console.WriteLine("Price: " + st.Price);
                Console.WriteLine("Boerse: " + st.IDBOERSE);
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");

            }
        }

        static void GetAllOrders()
        {
            dynamic[] Orders = JsonConvert.DeserializeObject<dynamic[]>(GET(OrdersURL));
            foreach (dynamic od in Orders)
            {
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine(od.id);
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");

            }
        }



    }







        }

