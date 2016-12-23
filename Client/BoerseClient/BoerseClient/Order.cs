using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{

    public class Txhistory
    {
        public int amount { get; set; }
        public double price { get; set; }
    }

    public class Order
    {

        public Order()
        {
           
        }

        public Order(string _id, Order[] _orders, string _amount)
        {
            foreach(Order _o in _orders)
            {
                if(_o.id == _id)
                {
                    this.id = _o.id;
                    this.idBank = _o.idBank;
                    this.idBoerse = _o.idBoerse;
                    this.idStock = _o.idStock;
                    this.price = _o.price;
                    this.amount = uint.Parse(_amount);
                    this.type = _o.type;
                    this.idCustomer = _o.idCustomer;
                    this.signature = _o.signature;
                    this.timestamp = _o.timestamp;
                    this.txhistory = _o.txhistory;
                }
            }

        }

        private string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("x2"));

            }

            return sb.ToString();

        }



        DateTimeOffset _timestamp;
        public DateTimeOffset timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }


        uint _amount;
        public uint amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }


        //List<Txhistory> _txhistory;
        // public List<Txhistory> txhistory
        //  {
        //      get
        //      {
        //          return _txhistory;
        //      }
        //      set
        //      {
        //          _txhistory = value;
        //      }
        //  }
        public List<Txhistory> txhistory { get; set; }

        string _idBoerse;
        public string idBoerse
        {
            get
            {
                return _idBoerse;
            }
            set
            {
                _idBoerse = value;
            }
        }


        string _type;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }


        string _idBank;
        public string idBank
        {
            get
            {
                return _idBank;
            }
            set
            {
                _idBank = value;
            }
        }

        double _price;
        public double price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }


        string _signature;
        public string signature
        {
            get
            {
                return _signature;
            }
            set
            {
                _signature = CalculateMD5Hash(value);
            }
        }

        string _idStock;
        public string idStock
        {
            get
            {
                return _idStock;
            }
            set
            {
                _idStock = value;
            }
        }



        string _idCustomer;
        public string idCustomer
        {
            get
            {
                return _idCustomer;
            }
            set
            {
                _idCustomer = value;
            }
        }


        string _id;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }

        }

        //private static string getUOID()
        //{

        //    return "O-" + Guid.NewGuid().ToString("N");

        //}

    }
}
