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

        public double CalcCurrentWorth(Stock[] _AllStocks)
        {
            this.worth = 0d;
            foreach (KeyValuePair<Stock,uint> KPV in this.Stocks)
            {
                foreach(Stock _S in _AllStocks)
                {
                    if(KPV.Key.ID == _S.ID)
                    {
                        KPV.Key.Price = _S.Price;
                        this.worth += KPV.Key.Price * KPV.Value;
                    }
                }
            }

            DataControl.Instance.UpdateWorthInDepot(this, this.worth);

            return this.worth;
        }

        private List<Order> _IssuedOrders = new List<Order>();
        public List<Order> IssuedOrders
        {
            get
            {
                return _IssuedOrders ;
            }
            set
            {
                _IssuedOrders = value;
            }
        }

        public void AddSellOrder(Order _order)
        {
            this._IssuedOrders.Add(_order);
            DataControl.Instance.SaveIssuedOrderToDepot(this,_order);
        }



        private List<KeyValuePair<Stock, uint>> stocks = new List<KeyValuePair<Stock, uint>>();

        public List<KeyValuePair<Stock, uint>> Stocks
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

        public void AddStock(Stock _stock, uint _amount)
        {

            this.stocks.Add(new KeyValuePair<Stock, uint>(_stock, _amount));
            List<KeyValuePair<Stock, uint>> StockList = new List<KeyValuePair<Stock, uint>>();
            KeyValuePair<Stock, uint> KVP = new KeyValuePair<Stock, uint>(_stock, _amount);
            StockList.Add(KVP);
            DataControl.Instance.SaveStocksToDepot(this, StockList);
            
        }

        public void AddStocks(List<KeyValuePair<Stock, uint>> _stocks)
        {
            foreach ( KeyValuePair<Stock, uint> s in _stocks)
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
