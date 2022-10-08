﻿using System.Text.Json;

namespace OSLCommon
{
    public static class ServerStringResponse
    {
        public static string InvalidCredential = "ServerResponse_InvalidCredentials";
        public static string InvalidPermission = "ServerResponse_InvalidPermission";

        public static string AccountNotFound = "ServerRespones_Account_NotFound";
        public static string AccountDisabled => $"ServerResponse_AccountDisabled";
        public static string AccountTokenGrantFailed = "ServerResponse_Account_TokenGrantFailed";
        public static string AccountTokenGranted = "ServerResponse_Account_TokenGranted";

        public static string UnsupportedMediaType = "ServerResponse_MediaTypeUnsupported";

        public static string InvalidParameter(string parameterName)
            => $"Invalid parameter \"{parameterName}\"";
        public static string InvalidBody => "ServerResponse_InvalidBody";

        public static string ExpectedValueOnProperty(string propertyName, object expectedValue, object recievedValue)
            => $"Expected {propertyName} to be \"{JsonSerializer.Serialize(expectedValue)}\" but got \"{JsonSerializer.Serialize(recievedValue)}\" instead";

        public static string SerializationFailure => $"ServerResponse_BodySerializationFailure";
    }
}