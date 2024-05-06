using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Exceptions
{
    public class InvalidPasswordFormateException : Exception
    {
        public InvalidPasswordFormateException(string message) : base(message) { }
    }
}
