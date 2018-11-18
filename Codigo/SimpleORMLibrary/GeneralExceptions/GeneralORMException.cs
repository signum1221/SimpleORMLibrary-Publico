using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleORMLibrary.GeneralExceptions
{
    public class GeneralORMException : System.Exception
    {
        public GeneralORMException() : base() { }
        public GeneralORMException(string message) : base(message) { }
        public GeneralORMException(string message, System.Exception inner) : base(message, inner) { }
    }
}
