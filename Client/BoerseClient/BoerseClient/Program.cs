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
                    Console.WriteLine("Please enter the Customer Nr. (0-" + (customercounter) + "): ");
                    CID_str = Console.ReadLine();

                }
                CID = uint.Parse(CID_str);
                Console.WriteLine("Chosen Customer: ");
                Customer CC = DataControl.Instance.LoadAllCustomers()[(int)CID];
                Console.WriteLine("Customer Nr.: " + CID);
                Console.WriteLine("Internal Customer Nr.: " + CC.ID);
                Console.WriteLine("First Name: " + CC.FirstName);
                Console.WriteLine("Last Name: " + CC.LastName);

                List<Depot> DepotsOfCustomers = DataControl.Instance.GetDepotsByCustomer(CC, GetAllOrders());

                if (DepotsOfCustomers.Count > 0)
                {
                    uint depotcounter = 0;
                    Console.WriteLine("Depots: ");
                    foreach (Depot _D in DepotsOfCustomers)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Depot Nr.: " + (depotcounter++));
                        Console.WriteLine("Internal Depot Nr.: " + _D.ID);
                        Console.WriteLine("Internal Customer Nr.: " + _D.Owner.ID);
                        Console.WriteLine("Stocks: ");
                        foreach (KeyValuePair<Stock, uint> _S in _D.Stocks)
                        {
                            Console.WriteLine("             " + _S.Key.ID + " (" + _S.Value.ToString() + ")");
                        }
                        Console.WriteLine("Issued Orders: ");
                        foreach (Order _o in _D.IssuedOrders)
                        {
                            Console.WriteLine("             " + _o.id + " " + _o.idBoerse + " " + _o.type + " " + _o.amount);
                        }
                        _D.CalcCurrentWorth(GetAllStocks());
                        Console.WriteLine("Current Worth: " + _D.Worth);
                        Console.WriteLine("--------------------------------------");

                    }
                    Console.WriteLine("Please enter the Depot Nr. (0-" + (--depotcounter) + "): ");
                    string DID_str = Console.ReadLine();

                    uint DID = 0;



                    while ((!uint.TryParse(DID_str, out DID)) || (DID < 0) || (DID > depotcounter))
                    {
                        Console.WriteLine("Please enter the Depot Nr. (0-" + (depotcounter) + "): ");
                        DID_str = Console.ReadLine();

                    }
                    DID = uint.Parse(DID_str);
                    Console.WriteLine("Chosen Depot: ");
                    Depot _DD = DataControl.Instance.GetDepotsByCustomer(CC, GetAllOrders())[(int)DID];
                    Console.WriteLine("Depot Nr.: " + DID);
                    Console.WriteLine("Internal Depot Nr.: " + _DD.ID);
                    Console.WriteLine("Internal Customer Nr.: " + _DD.Owner.ID);
                    Console.WriteLine("Stocks: ");
                    if (_DD.IssuedOrders.Count > 0)
                    {
                        Console.WriteLine("- Evaluating Issued Orders...");
                        Order[] ALLORDERS = GetAllOrders();
                        foreach (Order _o in _DD.IssuedOrders)
                        {
                            for (int i = 0; i < ALLORDERS.Length; i++)
                            {
                                if (_o.id == ALLORDERS[i].id)
                                {
                                    if (ALLORDERS[i].amount == 0)
                                    {
                                        if (_o.type == "buy")
                                        {
                                            Stock _s = new Stock(_o.idStock);
                                            _s.IDBOERSE = _o.idBoerse;
                                            _s.Price = _o.price;

                                            _DD.AddStock(_s, _o.amount);
                                            DataControl.Instance.SaveIssuedOrderToDepot(_DD, ALLORDERS[i]);
                                            _DD.CalcCurrentWorth(GetAllStocks());
                                        }
                                        else if(_o.type == "sell")
                                        {
                                            Stock _s = new Stock(_o.idStock);
                                            _s.IDBOERSE = _o.idBoerse;
                                            _s.Price = _o.price;
                                            uint old_amount = 0;
                                            foreach (KeyValuePair<Stock, uint> _S in _DD.Stocks)
                                            {
                                                if(_s.ID == _S.Key.ID)
                                                {
                                                    old_amount = _S.Value;
                                                }
                                                
                                            }

                                            DataControl.Instance.UpdateStockInDepot(_DD, new KeyValuePair<Stock, uint>(_s, old_amount), (int)_o.amount);
                                            DataControl.Instance.SaveIssuedOrderToDepot(_DD, ALLORDERS[i]);
                                            _DD.CalcCurrentWorth(GetAllStocks());
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _DD = DataControl.Instance.GetDepotsByCustomer(CC, GetAllOrders())[(int)DID];
                    _DD.CalcCurrentWorth(GetAllStocks());

                    if (_DD.Stocks.Count > 0)
                    {
                        Console.WriteLine("The following Stocks remain in the Depot: ");
                        uint stockcounter = 0;
                        foreach (KeyValuePair<Stock, uint> _S in _DD.Stocks)
                        {
                            Console.WriteLine("Stock Nr.: "+stockcounter++);
                            Console.WriteLine("             " + _S.Key.ID + " (" + _S.Value.ToString() + ")");
                        }

                        Console.WriteLine("...Resulting in a Worth of: " + _DD.Worth);

                        string WannaSellAStock = "";
                        while ((WannaSellAStock != "yes") && (WannaSellAStock != "no"))
                        {
                            Console.WriteLine("Do you want to sell any of the remaining Stocks?");
                            WannaSellAStock = Console.ReadLine();
                        }

                        if (WannaSellAStock == "yes")
                        {




                            Console.WriteLine("Please enter the Stock Nr. (0-" + (--stockcounter) + "): ");
                            string SID_str = Console.ReadLine();

                            uint SID = 0;



                            while ((!uint.TryParse(SID_str, out SID)) || (SID < 0) || (SID > stockcounter))
                            {
                                Console.WriteLine("Please enter the Stock Nr. (0-" + (stockcounter) + "): ");
                                SID_str = Console.ReadLine();

                            }
                            SID = uint.Parse(SID_str);
                            Stock[] _ALLSTOCKS = GetAllStocks();
                            double curr_price = 0d;
                            foreach (Stock _S in _ALLSTOCKS)
                            {
                                if (_S.ID == _DD.Stocks[(int)SID].Key.ID)
                                {
                                    curr_price = _S.Price;
                                    _DD.Stocks[(int)SID].Key.Price = curr_price;
                                }
                            }






                            Console.WriteLine("Chosen Stock: ");
                            Console.WriteLine("--------------------------------------");
                            Console.WriteLine("Stock Nr.: " + SID);
                            Console.WriteLine("             " + _DD.Stocks[(int)SID].Key.ID + " (" + _DD.Stocks[(int)SID].Value.ToString() + ")");
                            Console.WriteLine("--------------------------------------");


                            Console.WriteLine("Please enter the amount of Stocks you would like to SELL (max." + _DD.Stocks[(int)SID].Value + "): ");
                            string amt_str = Console.ReadLine();

                            uint amt = 0;



                            while ((!uint.TryParse(amt_str, out amt)) || (amt < 1) || (amt > _DD.Stocks[(int)SID].Value))
                            {
                                Console.WriteLine("Please enter the amount of Stocks you would like to SELL (max." + _DD.Stocks[(int)SID].Value + "): ");
                                amt_str = Console.ReadLine();

                            }
                            amt = uint.Parse(amt_str);


                            Console.WriteLine("Please enter the price  you would like to SELL the stocks for (current: " + curr_price + "): ");
                            string prc_str = Console.ReadLine();

                            double prc = 0;



                            while ((!double.TryParse(prc_str, out prc)) || (prc < 0))
                            {

                                Console.WriteLine("Please enter the price  you would like to SELL the stocks for (current: " + curr_price + "): ");
                                prc_str = Console.ReadLine();

                            }
                            prc = double.Parse(prc_str);


                            string WannaSellStock = "";
                            while ((WannaSellStock != "yes") && (WannaSellStock != "no"))
                            {
                                Console.WriteLine("Send Order TO SELL Stock " + _DD.Stocks[(int)SID].Key.ID + " with the amount of " + amt + " for the price of " + prc + "?");
                                WannaSellStock = Console.ReadLine();
                            }

                            if (WannaSellStock == "yes")
                            {
                                Order SellOrder = new Order();
                                SellOrder.timestamp = DateTimeOffset.Now;
                                SellOrder.idBoerse = "datBoerse";
                                SellOrder.idBank = "dieBank";
                                SellOrder.idStock = _DD.Stocks[(int)SID].Key.ID;
                                SellOrder.price = prc;
                                SellOrder.amount = amt;
                                SellOrder.type = "sell";
                                SellOrder.idCustomer = CC.ID;

                                SellOrder = JsonConvert.DeserializeObject<Order>(SendOrder(SellOrder));
                                _DD.AddSellOrder(SellOrder);

                                Console.WriteLine("Order has been sent!");
                                _DD.CalcCurrentWorth(GetAllStocks());
                                continue;
                            }
                            else
                            {
                                _DD.CalcCurrentWorth(GetAllStocks());
                            }
                        }
                    }

                    if (_DD.Stocks.Count <= 0)
                    {
                        Console.WriteLine("No Stocks found in Depot - Loading available Stocks from Boerse...");
                        Order[] Orders = GetAllOrders();
                        uint sellordercounter = 0;
                        for (int i = 0; i < Orders.Length; i++)
                        {

                            if (Orders[i].type == "sell")
                            {
                                sellordercounter++;
                            }
                        }
                        Console.WriteLine("Sells: " + sellordercounter);
                        Order[] SellOrders = new Order[sellordercounter];

                        sellordercounter = 0;
                        for (int i = 0; i < Orders.Length; i++)
                        {

                            if (Orders[i].type == "sell")
                            {
                                SellOrders[sellordercounter] = Orders[i];
                                sellordercounter++;
                            }
                        }


                        uint ordercounter = 0;
                        foreach (Order od in SellOrders)
                        {

                            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                            Console.WriteLine("Order Nr.: " + ordercounter++);
                            Console.WriteLine("ID: " + od.id);
                            Console.WriteLine("Time: " + od.timestamp);
                            Console.WriteLine("Type: " + od.type);
                            Console.WriteLine("Boerse: " + od.idBoerse);
                            Console.WriteLine("Customer: " + od.idCustomer);
                            Console.WriteLine("Stock: " + od.idStock);
                            Console.WriteLine("Amount: " + od.amount);
                            Console.WriteLine("Price: " + od.price);
                            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");


                        }

                        string WannaBuyAStock = "";
                        while ((WannaBuyAStock != "yes") && (WannaBuyAStock != "no"))
                        {
                            Console.WriteLine("Do you want to buy any Stocks?");
                            WannaBuyAStock = Console.ReadLine();
                        }

                        if (WannaBuyAStock == "yes")
                        {

                            Console.WriteLine("Please enter the Order Nr. (0-" + (--ordercounter) + "): ");
                            string OID_str = Console.ReadLine();

                            uint OID = 0;



                            while ((!uint.TryParse(OID_str, out OID)) || (OID < 0) || (OID > ordercounter))
                            {
                                Console.WriteLine("Please enter the Order Nr. (0-" + (ordercounter) + "): ");
                                OID_str = Console.ReadLine();

                            }
                            OID = uint.Parse(OID_str);
                            Console.WriteLine("Chosen Order: ");
                            Order OO = SellOrders[OID];
                            Console.WriteLine("Order Nr.: " + OID);
                            Console.WriteLine("ID: " + OO.id);
                            Console.WriteLine("Time: " + OO.timestamp);
                            Console.WriteLine("Type: " + OO.type);
                            Console.WriteLine("Boerse: " + OO.idBoerse);
                            Console.WriteLine("Customer: " + OO.idCustomer);
                            Console.WriteLine("Stock: " + OO.idStock);
                            Console.WriteLine("Amount: " + OO.amount);
                            Console.WriteLine("Price: " + OO.price);


                            Console.WriteLine("Please enter the amount of Stocks you would like to buy (max." + OO.amount + "): ");
                            string amt_str = Console.ReadLine();

                            uint amt = 0;



                            while ((!uint.TryParse(amt_str, out amt)) || (amt < 1) || (amt > OO.amount))
                            {
                                Console.WriteLine("Please enter the amount of Stocks you would like to buy (max." + OO.amount + "): ");
                                amt_str = Console.ReadLine();

                            }
                            amt = uint.Parse(amt_str);


                            Console.WriteLine("Please enter the price  you would like to buy the stocks for (current: " + OO.price + "): ");
                            string prc_str = Console.ReadLine();

                            double prc = 0;



                            while ((!double.TryParse(prc_str, out prc)) || (prc < 0))
                            {

                                Console.WriteLine("Please enter the price  you would like to buy the stocks for (current: " + OO.price + "): ");
                                prc_str = Console.ReadLine();

                            }
                            prc = double.Parse(prc_str);


                            string DoSendOrder = "";
                            while ((DoSendOrder != "yes") && (DoSendOrder != "no"))
                            {
                                Console.WriteLine("Send Order for Stock " + OO.idStock + " with the amount of " + amt + " for the price of " + prc + "?");
                                DoSendOrder = Console.ReadLine();
                            }

                            if (DoSendOrder == "yes")
                            {
                                Order BuyOrder = new Order();
                                BuyOrder.timestamp = DateTimeOffset.Now;
                                BuyOrder.idBoerse = "datBoerse";
                                BuyOrder.idBank = "dieBank";
                                BuyOrder.idStock = OO.idStock;
                                BuyOrder.price = prc;
                                BuyOrder.amount = amt;
                                BuyOrder.type = "buy";
                                BuyOrder.idCustomer = CC.ID;

                                BuyOrder = JsonConvert.DeserializeObject<Order>(SendOrder(BuyOrder));
                                _DD.AddSellOrder(BuyOrder);

                                Console.WriteLine("Order has been sent!");
                                _DD.CalcCurrentWorth(GetAllStocks());
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }



                    }
                    
                }
                else
                {
                    Console.WriteLine(CC.FirstName + " " + CC.LastName + " has no Depots");
                    string CreateDepot = "";
                    while ((CreateDepot != "yes") && (CreateDepot != "no"))
                    {
                        Console.WriteLine("Create Depot? Enter yes or no: ");
                        CreateDepot = Console.ReadLine();
                    }

                    if (CreateDepot == "yes")
                    {
                        new Depot(CC);
                        continue;
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
            //    Console.WriteLine("Time: " + od.timestamp);
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

        static string SendOrder(Order _order)
        {
            string serializedObject = JsonConvert.SerializeObject(_order);
            HttpWebRequest request = WebRequest.CreateHttp(PLACEOrderURL);
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
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                return responseText;
            }
            
        }



    }







        }

