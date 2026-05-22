using System.Net;
using System.Net.Http.Headers;
using TrainTicketWinForms.Models;

namespace TrainTicketWinForms.Services;

public static class TokenService
{
    private static readonly object SyncLock = new();

    public static string AccessToken { get; private set; } = string.Empty;
    public static DateTime ExpiresAtUtc { get; private set; }
    public static string UserName { get; private set; } = string.Empty;
    public static string Role { get; private set; } = string.Empty;

    public static void Set(LoginResponse session)
    {
        lock (SyncLock)
        {
            AccessToken = session.AccessToken ?? string.Empty;
            ExpiresAtUtc = session.ExpiresAtUtc;
            UserName = session.UserName ?? string.Empty;
            Role = session.Role ?? string.Empty;
        }
    }

    public static void Clear()
    {
        lock (SyncLock)
        {
            AccessToken = string.Empty;
            ExpiresAtUtc = default;
            UserName = string.Empty;
            Role = string.Empty;
        }
    }
}

public static class AuthService
{
    public static event Action? UnauthorizedReceived;

    public static void SetSession(LoginResponse session)
    {
        TokenService.Set(session);
    }

    public static void RaiseUnauthorized()
    {
        TokenService.Clear();
        UnauthorizedReceived?.Invoke();
    }
}

public static class ApiService
{
    private const string ApiBaseUrl = "http://localhost:5216/";

    private static readonly HttpClient ClientInstance = new(new AuthHeaderHandler())
    {
        BaseAddress = new Uri(ApiBaseUrl),
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static HttpClient Client => ClientInstance;
}

public class AuthHeaderHandler : DelegatingHandler
{
    public AuthHeaderHandler()
        : base(new HttpClientHandler())
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = TokenService.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        Console.WriteLine($"[API] {request.Method} {request.RequestUri}");
        Console.WriteLine($"[API] Authorization header attached: {request.Headers.Authorization is not null}");

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine($"[API] 401 Unauthorized: {request.RequestUri}");
            AuthService.RaiseUnauthorized();
        }

        return response;
    }
}
