using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.FastEndpoints.Events
{
    public class GetAllEventsEndpoint : EndpointWithoutRequest
    {
        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GetAllEventsEndpoint> _logger;

        public GetAllEventsEndpoint ( IEventService eventService, IMemoryCache cache, ILogger<GetAllEventsEndpoint> logger )
        {
            _eventService = eventService;
            _cache = cache;
            _logger = logger;
        }

        public override void Configure ( )
        {
            Get ( "api/school/events" );
            AuthSchemes ( "Bearer" );
        }

        public override async Task HandleAsync ( CancellationToken ct )
        {
            const string cacheKey = "all-events";

            if (!_cache.TryGetValue ( cacheKey, out var events ))
            {
                _logger.LogInformation ( "Fetching events from DB (not cached)" );
                events = await _eventService.GetAllEventsAsync ();
                _cache.Set ( cacheKey, events, TimeSpan.FromMinutes ( 5 ) );
            } else
            {
                _logger.LogInformation ( "Fetching events from Cache (not DB)" );
            }

            await SendOkAsync ( events, ct );
        }
    }
}
