using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cw6.Models
{
    public class LogData
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string QueryString { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
