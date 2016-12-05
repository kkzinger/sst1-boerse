using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace BoerseClient
{
    class Program
    {
        

        static void Main(string[] args)
        {


            Customer C = new Customer("Tobi","Mayer");
            Customer D = new Customer("Gerold", "Katzinger");
            Customer E = new Customer("Hubert Alois", "O'Donnell");
            Customer F = new Customer("Josef \"Sepp\"", "Mayr-Huber");

            //foreach(Customer _customer in DataControl.Instance.LoadAllCustomers())
            //{
            //    Console.WriteLine(_customer.ID);
            //    Console.WriteLine(_customer.FirstName);
            //    Console.WriteLine(_customer.LastName);
            //}
            Stock S1 = new Stock();
            Stock S2 = new Stock();
            Stock S3 = new Stock();
            Stock S4 = new Stock();

            Depot Depot = new Depot(E);
            Depot.AddStock(S2);
            Depot Depot2 = new Depot(F);
            List<Stock> stockstoadd = new List<Stock>();
            stockstoadd.Add(S1);
            stockstoadd.Add(S3);
            stockstoadd.Add(S4);
            Depot2.AddStocks(stockstoadd);

        }
    }
}
