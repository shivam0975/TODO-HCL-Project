using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backendMVC.Models;

namespace backendMVC.Controllers
{
    public class TasksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TasksController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration     = configuration;
        }

        private sealed class CreateTaskInputModel
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string Priority { get; set; } = "Medium";
            public string? Category { get; set; }
            public DateTime? DueDate { get; set; }
        }

        // GET: /Tasks  — reads Bearer token from session, fetches user's tasks from API
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync("api/tasks");
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = BuildApiUnavailableMessage();
                return RedirectToAction("Login");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load tasks. Please log in again.";
                return RedirectToAction("Login");
            }

            var json  = await response.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<TaskViewModel>();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(tasks);
        }

        // POST: /Tasks/Create
        [HttpPost]
        public async Task<IActionResult> Create(string title, string? description, string priority, string? category, DateTime? dueDate)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(title))
            {
                TempData["Error"] = "Title is required to create a task.";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var payload = new CreateTaskInputModel
            {
                Title = title.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                Priority = string.IsNullOrWhiteSpace(priority) ? "Medium" : priority,
                Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
                DueDate = dueDate
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("api/tasks", content);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = BuildApiUnavailableMessage();
                return RedirectToAction("Index");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to create task.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Task created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Tasks/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Task ID is required.";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync($"api/tasks/{id}");
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = BuildApiUnavailableMessage();
                return RedirectToAction("Index");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load task for editing.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<TaskViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (task == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // POST: /Tasks/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(TaskViewModel model)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.Error = "Task ID and Title are required.";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                title = model.Title.Trim(),
                description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                status = string.IsNullOrWhiteSpace(model.Status) ? "Pending" : model.Status,
                priority = string.IsNullOrWhiteSpace(model.Priority) ? "Medium" : model.Priority,
                category = string.IsNullOrWhiteSpace(model.Category) ? null : model.Category.Trim(),
                dueDate = model.DueDate
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PutAsync($"api/tasks/{model.Id}", content);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = BuildApiUnavailableMessage();
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to update task.";
                return View(model);
            }

            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Tasks/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Task ID is required.";
                return RedirectToAction("Index");
            }

            var client = _httpClientFactory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await client.DeleteAsync($"api/tasks/{id}");
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = BuildApiUnavailableMessage();
                return RedirectToAction("Index");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete task.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Task deleted successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Tasks/Login
        public IActionResult Login()
        {
            if (TempData["Error"] is string error)
            {
                ViewBag.Error = error;
            }

            return View();
        }

        // POST: /Tasks/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var client  = _httpClientFactory.CreateClient("TaskApi");
            var payload = new { email, password };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("api/auth/login", content);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = BuildApiUnavailableMessage();
                return View();
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            var json   = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthApiResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (string.IsNullOrWhiteSpace(result?.Token))
            {
                ViewBag.Error = "Login response from API did not include a token.";
                return View();
            }

            var userName = result.User?.FullName;
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = email;
            }

            HttpContext.Session.SetString("JwtToken", result.Token);
            HttpContext.Session.SetString("UserName", userName);

            return RedirectToAction("Index");
        }

        // GET: /Tasks/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string BuildApiUnavailableMessage()
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5137/";
            return $"Cannot reach API at {baseUrl}. Start the backend API and try again.";
        }

        private sealed class AuthApiResponse
        {
            public string? Token { get; set; }
            public AuthApiUser? User { get; set; }
        }

        private sealed class AuthApiUser
        {
            public string? FullName { get; set; }
        }
    }
}
