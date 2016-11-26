using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoerseClient
{
    public class Customer
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


        string firstname;

        public string FirstName
        {
            get
            {
                return firstname;
            }
            set
            {
                firstname = value;
            }
        }


        string lastname;

        public string LastName
        {
            get
            {
                return lastname;
            }
            set
            {
                lastname = value;
            }
        }

        public Customer(string _firstname, string _lastname)
        {
            id = getUCID();
            firstname =  _firstname;
            lastname = _lastname;

            DataControl.Instance.SaveCustomer(this);

        }

        public Customer()
        {

        }

        private static string getUCID()
        {

            return "C-" + Guid.NewGuid().ToString("D");

        }
    }
}
