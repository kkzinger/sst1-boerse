using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{
    public class Depot
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

        private Customer owner;

        public Customer Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

        private List<Stock> shares;

        public List<Stock> Shares
        {
            get
            {
                return shares;
            }
            set
            {
                shares = value;
            }
        }


        public Depot()
        {
            id = getUDID();
        }

        private static string getUDID()
        {

            return "D-" + Guid.NewGuid().ToString("X");

        }
    }
}
