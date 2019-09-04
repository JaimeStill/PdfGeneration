using System;

namespace PdfGeneration.Core
{
    public enum ExceptionType
    {
        Validation,
        Authorization
    }

    public class AppException : Exception
    {
        public ExceptionType ExceptionType { get; set; }

        public AppException(string message, ExceptionType exceptionType) : base(message)
        {
            ExceptionType = exceptionType;
        }
    }
}