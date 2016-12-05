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


       
        public Stock()
        {
           id= getUSID();
        }

        private static string getUSID()
        {

            return "S-" + Guid.NewGuid().ToString("N");

        }
    }
}
