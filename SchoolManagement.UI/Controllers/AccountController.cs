using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ();
        }

        // GET: Login Page
        public IActionResult Login ( )
        {
            return View ();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login ( LoginViewModel model )
            
        {
            if (!ModelState.IsValid)
            {
                return View ( model );
            }

            var jsonContent = JsonConvert.SerializeObject ( model );
            var httpContent = new StringContent ( jsonContent, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync ( "https://localhost:7230/api/auth/login", httpContent );

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync ();
                var authResponse = JsonConvert.DeserializeObject<AuthResponse> ( responseBody );

                // Store session data
                HttpContext.Session.SetString ( "UserEmail", authResponse.Email );
                HttpContext.Session.SetString ( "UserRole", authResponse.Role );
                HttpContext.Session.SetString ( "JWTToken", authResponse.Token );

                // Store the JWT token in the session
                HttpContext.Session.SetString ( "JWTToken", authResponse.Token );

                // If the logged in user is a Student, redirect to the Student Details page.
                if (authResponse.Role.Equals ( "Student", StringComparison.OrdinalIgnoreCase ))
                {
                    var token = authResponse.Token;

                    var request = new HttpRequestMessage (
                        HttpMethod.Get,
                        $"https://localhost:7230/api/students/byemail?email={model.Email}"
                    );

                    if (!string.IsNullOrEmpty ( token ))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue ( "Bearer", token );
                    }

                    var studentResponse = await _httpClient.SendAsync ( request );

                    if (!studentResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError ( "", "Failed to retrieve student info." );
                        return View ( model );
                    }

                    var studentJson = await studentResponse.Content.ReadAsStringAsync ();
                    var student = JsonConvert.DeserializeObject<Student> ( studentJson );

                    HttpContext.Session.SetInt32 ( "StudentId", student.Id );
                    HttpContext.Session.SetString ( "ClassId", student.ClassId.ToString () );
                    // Assuming the API returns the StudentId for student users.
                    return RedirectToAction ( "Index", "Home" );
                }

                // For other roles, redirect to Home/Index.
                return RedirectToAction ( "Index", "Home" );
            }

            ModelState.AddModelError ( "", "Invalid Email or Password." );
            return View ( model );
        }

        // Logout
        public async Task<IActionResult> Logout ( )
        {
            var response = await _httpClient.PostAsync ( "https://localhost:7230/api/auth/logout", null );

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Clear ();
                return RedirectToAction ( "Login" );
            }

            ModelState.AddModelError ( "", "Logout failed." );
            return RedirectToAction ( "Login" );
        }

        // Helper method to attach JWT token to HttpClient for authorized requests
        private void AttachJwtTokenToHttpClient ( )
        {
            var token = HttpContext.Session.GetString ( "JWTToken" );
            if (!string.IsNullOrEmpty ( token ))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ( "Bearer", token );
            }
        }
        public IActionResult AccessDenied ( )
        {
            return View ();
        }
    }
}
