using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{
    public class Depot
    {
        private string id { get; set; }

        public string ID
        {
            get
            {
                return id;
            }
        }

        private Customer owner;

        public Customer Owner
        {
            get
            {
                return owner;
            }
        }

        private List<Stock> stocks = new List<Stock>();

        public List<Stock> Stocks
        {
            get
            {
                return stocks;
            }
        }

        public void AddStock(Stock _stock)
        {
            this.stocks.Add(_stock);
            DataControl.Instance.SaveDepot(this);
            
        }

        public void AddStocks(List<Stock> _stocks)
        {
            foreach (Stock s in _stocks)
            {
                this.stocks.Add(s);
            }
            DataControl.Instance.SaveDepot(this);

        }


        public Depot(Customer _owner)
        {
            id = getUDID();
            this.owner = _owner;

            DataControl.Instance.SaveDepot(this);

        }

        private static string getUDID()
        {

            return "D-" + Guid.NewGuid().ToString("N");

        }
    }
}
