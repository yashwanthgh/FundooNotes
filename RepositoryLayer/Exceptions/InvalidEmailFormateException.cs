using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Exceptions
{
    public class InvalidEmailFormateException : Exception
    {
        public InvalidEmailFormateException(string message) : base(message) { }
    }
}
