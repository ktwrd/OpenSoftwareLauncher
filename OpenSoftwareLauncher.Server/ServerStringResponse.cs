using System.Text.Json;

namespace OpenSoftwareLauncher.Server
{
    public static class ServerStringResponse
    {
        public static string InvalidCredential = "ServerResponse_InvalidCredentials";
        public static string InvalidPermission = "ServerResponse_InvalidPermission";
        public static string UnsupportedMediaType = "ServerResponse_MediaTypeUnsupported";
        public static string AccountNotFound(string username)
            => $"Could not find account with username of \"{username}\"";

        public static string InvalidParameter(string parameterName)
            => $"Invalid parameter \"{parameterName}\"";
        public static string InvalidBody => "ServerResponse_InvalidBody";

        public static string ExpectedValueOnProperty(string propertyName, object expectedValue, object recievedValue)
            => $"Expected {propertyName} to be \"{JsonSerializer.Serialize(expectedValue)}\" but got \"{JsonSerializer.Serialize(recievedValue)}\" instead";

        public static string AccountDisabled => $"ServerResponse_AccountDisabled";
        public static string SerializationFailure => $"ServerResponse_BodySerializationFailure";
    }
}
