using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.UI.Filter;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    //[AuthorizeUser ( "Admin", "Teacher", "Student" )]

    public class ContactUsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ContactUsController ( IHttpClientFactory httpClientFactory )
        {
            //_httpClient = httpClientFactory.CreateClient ();
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _httpClient.BaseAddress = new Uri ( "https://localhost:7230/" );
        }

        // Show list to Admin
        [AuthorizeUser ( "Admin")]
        public async Task<IActionResult> Messages ( )
        {
            var response = await _httpClient.GetAsync ( "api/ContactUsApi/api/get-contact-list" );

            if (!response.IsSuccessStatusCode)
                return View ( "Error" );

            var json = await response.Content.ReadAsStringAsync ();
            var messages = JsonConvert.DeserializeObject<List<ContactRequestDto>> ( json );

            return View ( messages );
        }

        // Receive form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit ( ContactRequestDto dto )
        {
            if (!ModelState.IsValid)
                return RedirectToAction ( "Index", "Home", new { fragment = "contactBlock" } );

            var json = JsonConvert.SerializeObject ( dto );
            var content = new StringContent ( json, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync ( "api/ContactUsApi/api/contact", content );

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Your message has been sent successfully!";
            }
            else
            {
                TempData["Error"] = "There was a problem. Please try again.";
            }

            return RedirectToAction ( "Index", "Home", new { fragment = "contactBlock" } );
        }
    }
}
