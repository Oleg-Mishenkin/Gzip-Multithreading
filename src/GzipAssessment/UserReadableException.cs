using System;

namespace GzipAssessment
{
    public class UserReadableException : Exception
    {
        private readonly string _errorMessage;

        public UserReadableException(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public string ErrorMessage { get { return _errorMessage; }}
    }
}
