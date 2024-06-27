using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.Permify
{
    public class PermifyException : Exception
    {
        public PermifyError? Error { get; set; }

        public PermifyException(string message)
            : base(message)
        {
        }

        public PermifyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PermifyException(PermifyError error)
            : base(error.Message)
        {
            Error = error;
        }

        public PermifyException(PermifyError error, string message)
            : base(message)
        {
            Error = error;
        }

        public PermifyException(PermifyError error, Exception innerException)
            : base(error.Message, innerException)
        {
            Error = error;
        }

        public PermifyException(PermifyError error, string message, Exception innerException)
            : base(message, innerException)
        {
            Error = error;
        }
    }
}
