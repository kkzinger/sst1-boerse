using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{
    public class Stock
    {
        string id { get; set; }

        public string ID    
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        string name { get; set; }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        double price { get; set; }

        public double Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        string idBoerse { get; set; }

        public string IDBOERSE
        {
            get
            {
                return idBoerse;
            }
            set
            {
                idBoerse = value;
            }
        }


        public Stock()
        {
            //id= getUSID();
            id =" TODO: GET THE ID FROM ALLSTOCKS";
        }

        public Stock(string _ID)
        {
            id = _ID;
        }

        //private static string getUSID()
        //{

        //    return "S-" + Guid.NewGuid().ToString("N");

        //}
    }
}
