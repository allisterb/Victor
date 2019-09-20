using System;
using System.Collections.Generic;
using System.Text;

namespace Victor
{
    public class ApiNotInitializedException : Exception
    {
        public ApiNotInitializedException(Api api) : base($"The {api.GetType().Name} Api is not initialized.") {}
    }

    public class AudioApiException : Exception
    {
        public string ApiName { get; }
        public AudioApiException(string apiName, string message) : base(message)
        {
            ApiName = apiName;
        }
    }
}
