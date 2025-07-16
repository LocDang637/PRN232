using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmokeQuit.Repositories.LocDPX.ModelExtensions;
using SmokeQuit.Repositories.LocDPX.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace SmokeQuit.MVCWebApp.FE.LocDPX.Controllers
{
    [Authorize]
    public class ChatsLocDpxController : Controller
    {
        private string APIEndPoint = "https://localhost:7260/api/";

        public async Task<IActionResult> Index(string? message, string? messageType, string? sentBy, int? userId, int? coachId, int? pageNumber)
        {
            var search = new SearchChatRequest
            {
                Message = message ?? "",
                MessageType = messageType ?? "",
                SentBy = sentBy ?? "",
                CurrentPage = pageNumber ?? 1,
                PageSize = 10
            };

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (string.IsNullOrEmpty(tokenString))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                    // Use different endpoint based on filters
                    string endpoint;
                    if (userId.HasValue)
                    {
                        endpoint = $"{APIEndPoint}ChatsLocDpx/user/{userId.Value}";
                        var userResponse = await httpClient.GetAsync(endpoint);
                        if (userResponse.IsSuccessStatusCode)
                        {
                            var userContent = await userResponse.Content.ReadAsStringAsync();
                            var userChats = JsonConvert.DeserializeObject<List<ChatsLocDpx>>(userContent);
                            var userPagedList = userChats.ToPagedList(search.CurrentPage, search.PageSize);
                            ViewData["FilterInfo"] = $"Messages from User ID: {userId}";
                            return View(userPagedList);
                        }
                    }
                    else if (coachId.HasValue)
                    {
                        endpoint = $"{APIEndPoint}ChatsLocDpx/coach/{coachId.Value}";
                        var coachResponse = await httpClient.GetAsync(endpoint);
                        if (coachResponse.IsSuccessStatusCode)
                        {
                            var coachContent = await coachResponse.Content.ReadAsStringAsync();
                            var coachChats = JsonConvert.DeserializeObject<List<ChatsLocDpx>>(coachContent);
                            var coachPagedList = coachChats.ToPagedList(search.CurrentPage, search.PageSize);
                            ViewData["FilterInfo"] = $"Messages from Coach ID: {coachId}";
                            return View(coachPagedList);
                        }
                    }
                    else
                    {
                        // Use search endpoint for general filtering
                        endpoint = $"{APIEndPoint}ChatsLocDpx/Search";
                        using (var response = await httpClient.PostAsJsonAsync(endpoint, search))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                var result = JsonConvert.DeserializeObject<PaginationResult<List<ChatsLocDpx>>>(content);

                                if (result != null)
                                {
                                    var pagedList = new StaticPagedList<ChatsLocDpx>(
                                        result.Items,
                                        result.CurrentPage,
                                        result.PageSize,
                                        result.TotalItems
                                    );
                                    return View(pagedList);
                                }
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                TempData["Error"] = "Failed to load chat messages. Please try again.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return View(new List<ChatsLocDpx>().ToPagedList(search.CurrentPage, search.PageSize));
        }

        // GET: ChatsLocDpx/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["CoachId"] = new SelectList(await GetCoaches(), "CoachesLocDpxid", "FullName");
                ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading create form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ChatsLocDpx/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChatsLocDpx chat)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                        if (string.IsNullOrEmpty(tokenString))
                        {
                            return RedirectToAction("Login", "Account");
                        }

                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                        var chatDto = new
                        {
                            Message = chat.Message?.Trim(),
                            MessageType = chat.MessageType ?? "text",
                            UserId = chat.UserId,
                            CoachId = chat.CoachId,
                            SentBy = chat.SentBy ?? "user",
                            AttachmentUrl = chat.AttachmentUrl,
                            IsRead = chat.IsRead,
                            ResponseTime = chat.ResponseTime
                        };

                        using (var response = await httpClient.PostAsJsonAsync(APIEndPoint + "ChatsLocDpx", chatDto))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                TempData["Success"] = "Chat message created successfully!";
                                return RedirectToAction(nameof(Index));
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                TempData["Error"] = $"Failed to create message: {errorContent}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred: {ex.Message}";
                }
            }

            // Reload dropdowns on error
            try
            {
                ViewData["CoachId"] = new SelectList(await GetCoaches(), "CoachesLocDpxid", "FullName", chat.CoachId);
                ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName", chat.UserId);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return View(chat);
        }

        // GET: ChatsLocDpx/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            ChatsLocDpx chat = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (string.IsNullOrEmpty(tokenString))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                    var response = await httpClient.GetAsync(APIEndPoint + "ChatsLocDpx/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        chat = JsonConvert.DeserializeObject<ChatsLocDpx>(content);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading chat: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            if (chat == null) return NotFound();

            try
            {
                ViewData["CoachId"] = new SelectList(await GetCoaches(), "CoachesLocDpxid", "FullName", chat.CoachId);
                ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName", chat.UserId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading form data: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            return View(chat);
        }

        // POST: ChatsLocDpx/Edit/5 - COMPLETELY FIXED VERSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChatsLocDpx chat)
        {
            if (id != chat.ChatsLocDpxid)
            {
                return BadRequest();
            }

            // Clear model state for non-editable fields to avoid validation errors
            ModelState.Remove("UserId");
            ModelState.Remove("CoachId");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                        if (string.IsNullOrEmpty(tokenString))
                        {
                            return RedirectToAction("Login", "Account");
                        }

                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                        // FIXED: Only send the editable fields - DO NOT send UserId, CoachId, CreatedAt
                        var updateDto = new
                        {
                            Message = chat.Message?.Trim(),
                            MessageType = chat.MessageType,
                            SentBy = chat.SentBy,
                            AttachmentUrl = chat.AttachmentUrl,
                            IsRead = chat.IsRead,
                            ResponseTime = chat.ResponseTime
                            // IMPORTANT: NOT sending UserId, CoachId, or CreatedAt
                        };

                        using (var response = await httpClient.PutAsJsonAsync(APIEndPoint + "ChatsLocDpx/" + id, updateDto))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                TempData["Success"] = "Chat message updated successfully!";
                                return RedirectToAction(nameof(Index));
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                TempData["Error"] = $"Failed to update message: {errorContent}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                // Log validation errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = $"Validation errors: {string.Join(", ", errors)}";
            }

            // Reload dropdowns on error
            try
            {
                ViewData["CoachId"] = new SelectList(await GetCoaches(), "CoachesLocDpxid", "FullName", chat.CoachId);
                ViewData["UserId"] = new SelectList(await GetUsers(), "UserAccountId", "UserName", chat.UserId);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return View(chat);
        }

        // GET: ChatsLocDpx/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            ChatsLocDpx chat = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (string.IsNullOrEmpty(tokenString))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                    var response = await httpClient.GetAsync(APIEndPoint + "ChatsLocDpx/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        chat = JsonConvert.DeserializeObject<ChatsLocDpx>(content);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading chat details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            if (chat == null) return NotFound();
            return View(chat);
        }

        // GET: ChatsLocDpx/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            ChatsLocDpx chat = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (string.IsNullOrEmpty(tokenString))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                    var response = await httpClient.GetAsync(APIEndPoint + "ChatsLocDpx/" + id);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        chat = JsonConvert.DeserializeObject<ChatsLocDpx>(content);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading chat for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            if (chat == null) return NotFound();
            return View(chat);
        }

        // POST: ChatsLocDpx/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (string.IsNullOrEmpty(tokenString))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);

                    using (var response = await httpClient.DeleteAsync(APIEndPoint + "ChatsLocDpx/" + id))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["Success"] = "Chat message deleted successfully!";
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            TempData["Error"] = "You don't have permission to delete this message.";
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            TempData["Error"] = $"Failed to delete message: {errorContent}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to get coaches
        private async Task<List<CoachesLocDpx>> GetCoaches()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (!string.IsNullOrEmpty(tokenString))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);
                    }

                    using (var response = await httpClient.GetAsync(APIEndPoint + "CoachLocDpx"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<List<CoachesLocDpx>>(content);
                            return result ?? new List<CoachesLocDpx>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting coaches: {ex.Message}");
            }

            return new List<CoachesLocDpx>();
        }

        // Helper method to get users
        private async Task<List<SystemUserAccount>> GetUsers()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenString = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "TokenString").Value;

                    if (!string.IsNullOrEmpty(tokenString))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);
                    }

                    using (var response = await httpClient.GetAsync(APIEndPoint + "SystemUserAccount"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<List<SystemUserAccount>>(content);
                            return result ?? new List<SystemUserAccount>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
            }

            return new List<SystemUserAccount>();
        }
    }
}