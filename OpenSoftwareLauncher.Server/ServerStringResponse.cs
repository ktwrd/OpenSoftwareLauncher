using System.Text.Json;

namespace OpenSoftwareLauncher.Server
{
    public static class ServerStringResponse
    {
        public static string InvalidCredential = "Invalid Credential";
        public static string InvalidPermission = "Invalid Permissions";
        public static string UnsupportedMediaType = "Unsupported Media Type";
        public static string AccountNotFound(string username)
            => $"Could not find account with username of \"{username}\"";

        public static string InvalidParameter(string parameterName)
            => $"Invalid parameter \"{parameterName}\"";
        public static string InvalidBody => "Invalid Body";

        public static string ExpectedValueOnProperty(string propertyName, object expectedValue, object recievedValue)
            => $"Expected {propertyName} to be \"{JsonSerializer.Serialize(expectedValue)}\" but got \"{JsonSerializer.Serialize(recievedValue)}\" instead";

        public static string AccountDisabled => $"Your account is disabled. Please contact licensing@minalyze.com";
        public static string SerializationFailure => $"Failed to serialize data";
    }
}
