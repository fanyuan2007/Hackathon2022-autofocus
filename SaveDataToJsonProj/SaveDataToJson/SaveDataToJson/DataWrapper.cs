using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataToJson
{
    public class DataWrapper
    {
        public List<DataObj> data { get; set; }

        public DataWrapper(List<DataObj> data)
        {
            this.data = data;
        }
    }
}
