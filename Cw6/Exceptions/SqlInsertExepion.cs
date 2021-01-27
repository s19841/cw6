using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw6.Exceptions
{
    public class SqlInsertException : Exception
    {
        public SqlInsertException() { }
        public SqlInsertException(string message) : base(message) { }
    }
}
