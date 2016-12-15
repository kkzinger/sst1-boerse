using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{
    public class Order
    {
        DateTimeOffset timestamp;
        public DateTimeOffset Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }


        uint amount;
        public uint Amount
        {
            get
            {
                return Amount;
            }
            set
            {
                Amount = value;
            }
        }




    }
}
