using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class JwtTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenHandler ( IHttpContextAccessor httpContextAccessor )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync ( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        var token = _httpContextAccessor.HttpContext?.Session?.GetString ( "JWTToken" );

        if (!string.IsNullOrEmpty ( token ))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue ( "Bearer", token );
        }

        return await base.SendAsync ( request, cancellationToken );
    }
}
