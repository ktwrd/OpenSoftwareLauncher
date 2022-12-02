# OSL Authentication Provider
Currently OSL depends on an external Authentication Provider for granting tokens to users. The base URL for the Authentication Provider can be set in `Config/config.ini` at `Authentication.Provider`.

There are only one endpoint that is required for the Authentication Provider to work;
- `/api/login?email&password`

In order to validate `jane.doe@example.com` a request will be sent from the server to the Authentication Provider with the following URL (assuming that the Authentication Provider is set to `https://auth.example.com`)
`https://auth.example.com/api/login?email=jane.doe@example.com&password=unsafepassword`

When the credentials are accepted as valid credentials by the server, it will respond with a JSON with the following content;
```json
{
    "message": "success"
}
```

But when the credentials are rejected by the server, it will respond with the following JSON;
```json
{
    "message": "Failed to login"
}
```
