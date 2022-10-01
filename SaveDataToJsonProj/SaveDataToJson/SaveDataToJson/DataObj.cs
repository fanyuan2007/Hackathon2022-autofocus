using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataToJson
{
    public class DataObj
    {
        public string value { get; set; }
        public string id { get; set; }

        public DataObj(string _value, string _id)
        {
            value = _value;
            id = _id;
        }
    }
}
