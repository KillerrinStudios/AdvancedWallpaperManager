using System;
using System.Collections.Generic;
using System.Text;

namespace KillerrinStudiosToolkit
{
    public enum APIResponse
    {
        ContinuingExecution,
        Successful,
        Failed,

        //-- Standard Errors
        APIError,
        NetworkError,
        ServerError,
        UnknownError,

        //-- Specialized Errors
        NotSupported,
        InvalidAuthorizationKey,

        // Login
        InfoNotEntered,
        InvalidCredentials,
    }

    public static class APIResponseHelpers
    {
        public static bool IsAPIResponseFailed(APIResponse response)
        {
            if (response == APIResponse.Successful) return false;
            else if (response == APIResponse.ContinuingExecution) return false;
            return true;
        }
    }
}
