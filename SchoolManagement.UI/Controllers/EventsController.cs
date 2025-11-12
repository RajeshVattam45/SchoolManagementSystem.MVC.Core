using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin", "Teacher", "Student" )]
    public class EventsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _eventsApi;

        public EventsController ( IHttpClientFactory httpClientFactory,  IConfiguration configuration )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _eventsApi = configuration["ApiSettings:EventsApiBaseUrl"];
        }

        // GET: /Events
        public async Task<IActionResult> Index ( int? year, string search )
        {
            var events = await _httpClient.GetFromJsonAsync<List<Events>> ( _eventsApi );
            var selectedYear = year ?? DateTime.Now.Year;

            var availableYears = events
                .Select ( e => e.EventDate.Year )
                .Distinct ()
                .OrderByDescending ( y => y )
                .ToList ();

            var filteredEvents = events
                .Where ( e => e.EventDate.Year == selectedYear )
                .Where ( e =>
                    string.IsNullOrEmpty ( search ) ||
                    e.EventName.Contains ( search, StringComparison.OrdinalIgnoreCase ) ||
                    e.OrganizedBy.Contains ( search, StringComparison.OrdinalIgnoreCase )
                )
                .OrderBy ( e => e.EventDate )
                .ToList ();

            ViewBag.SelectedYear = selectedYear;
            ViewBag.Years = availableYears;

            return View ( filteredEvents );
        }


        // GET: /Events/Details/5
        public async Task<IActionResult> Details ( int id )
        {
            var ev = await _httpClient.GetFromJsonAsync<Events> ( $"{_eventsApi}/{id}" );
            if (ev == null)
                return NotFound ();

            return View ( ev );
        }

        // GET: /Events/Create
        public IActionResult Create ( )
        {
            return View ();
        }

        // POST: /Events/Create
        //[HttpPost]
        //public async Task<IActionResult> Create ( Events ev )
        //{
        //    if (!ModelState.IsValid)
        //        return View ( ev );

        //    var response = await _httpClient.PostAsJsonAsync ( _eventsApi, ev );

        //    if (response.IsSuccessStatusCode)
        //        return RedirectToAction ( nameof ( Index ) );

        //    return View ( ev );
        //}
        [HttpPost]
        public async Task<IActionResult> Create ( Events ev )
        {
            if (!ModelState.IsValid)
                return View ( ev );

            var response = await _httpClient.PostAsJsonAsync ( _eventsApi, ev );

            if (response.IsSuccessStatusCode)
                return RedirectToAction ( nameof ( Index ) );

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync ();

                // Parse the validation errors
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails> ( errorContent );

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var errorMessage in error.Value)
                        {
                            // This adds the error to the appropriate field
                            ModelState.AddModelError ( error.Key, errorMessage );
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError ( string.Empty, "An unexpected error occurred." );
            }

            return View ( ev );
        }

        // GET: /Events/Edit/5
        public async Task<IActionResult> Edit ( int id )
        {
            var ev = await _httpClient.GetFromJsonAsync<Events> ( $"{_eventsApi}/{id}" );
            if (ev == null)
                return NotFound ();

            return View ( ev );
        }

        // POST: /Events/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit ( int id, Events ev )
        {
            if (id != ev.Id)
                return BadRequest ();

            var response = await _httpClient.PutAsJsonAsync ( $"{_eventsApi}/{id}", ev );

            if (response.IsSuccessStatusCode)
                return RedirectToAction ( nameof ( Index ) );

            return View ( ev );
        }

        // GET: /Events/Delete/5
        public async Task<IActionResult> Delete ( int id )
        {
            var ev = await _httpClient.GetFromJsonAsync<Events> ( $"{_eventsApi}/{id}" );
            if (ev == null)
                return NotFound ();

            return View ( ev );
        }

        // POST: /Events/Delete/5
        [HttpPost, ActionName ( "Delete" )]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{_eventsApi}/{id}" );
            if (response.IsSuccessStatusCode)
                return RedirectToAction ( nameof ( Index ) );

            return BadRequest ();
        }
    }
}
