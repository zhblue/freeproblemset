using System;

namespace EasyFPSViewer.Models
{
    public class NotFPSFileException : ApplicationException
    {
        public NotFPSFileException(string message) : base(message)
        { }
    }
}
