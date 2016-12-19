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


        private double worth;
        public double Worth
        {
            get
            {
                return worth;
            }

            set
            {
                worth = value - value;
            }
        }

        private List<Order> _IssuedSellOrders = new List<Order>();
        public List<Order> IssuedSellOrders
        {
            get
            {
                return _IssuedSellOrders ;
            }
        }

        public void AddSellOrder(Order _sellorder)
        {
            this._IssuedSellOrders.Add(_sellorder);
        }

        private List<KeyValuePair<Stock, int>> stocks = new List<KeyValuePair<Stock, int>>();

        public List<KeyValuePair<Stock, int>> Stocks
        {
            get
            {
                return stocks;
            }

            set
            {
                stocks = value;
            }
        }

        public void AddStock(Stock _stock, int _amount)
        {

            this.stocks.Add(new KeyValuePair<Stock, int>(_stock, _amount));
            List<KeyValuePair<Stock, int>> StockList = new List<KeyValuePair<Stock, int>>();
            KeyValuePair<Stock, int> KVP = new KeyValuePair<Stock, int>(_stock, _amount);
            StockList.Add(KVP);
            DataControl.Instance.SaveStocksToDepot(this, StockList);
            
        }

        public void AddStocks(List<KeyValuePair<Stock, int>> _stocks)
        {
            foreach ( KeyValuePair<Stock, int> s in _stocks)
            {
                this.stocks.Add(s);
            }
            DataControl.Instance.SaveStocksToDepot(this, _stocks);

        }


        public Depot(Customer _owner)
        {
            id = getUDID();
            this.owner = _owner;
            
            //this.AddStock(new Stock(), 33);
            DataControl.Instance.SaveDepot(this);

        }

        public Depot()
        {

        }

        private static string getUDID()
        {

            return "D-" + Guid.NewGuid().ToString("N");

        }
    }
}
