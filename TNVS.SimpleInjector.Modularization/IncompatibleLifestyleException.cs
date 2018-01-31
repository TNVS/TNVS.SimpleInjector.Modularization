using System;

namespace TNVS.SimpleInjector.Modularization
{
    public class IncompatibleLifestyleException : Exception
    {
        public IncompatibleLifestyleException(string message) : base(message)
        {
        }

        public IncompatibleLifestyleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
