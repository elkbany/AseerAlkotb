using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Features.Roles.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAdminServices _adminServices;

        public UsersController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }

        // GET: Users - Show All Users (Admins + Clients)
        public async Task<IActionResult> Index()
        {
            try
            {
                var allUsersResult = await _adminServices.GetAllUsers();
                var allUsers = allUsersResult.Data ?? new List<GetAllAdminResponse>();
                var adminsResult = await _adminServices.GetAllAdmins();
                var admins = adminsResult.Data ?? new List<GetAllAdminResponse>();
                var clientsResult = await _adminServices.GetAllClients();
                var clients = clientsResult.Data ?? new List<GetAllClientResponse>();
                ViewBag.Admins = admins;
                ViewBag.Clients = clients;
                ViewBag.AllUsers = allUsers;
                ViewBag.TotalUsers = allUsers.Count;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading users: " + ex.Message;
                return View();
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Get user details with all related data
                var userDetailsResult = await _adminServices.GetUserDetailsWithOrders(id);
                
                if (!userDetailsResult.Succeeded)
                {
                    TempData["Error"] = userDetailsResult.Message;
                    return RedirectToAction(nameof(Index));
                }

                var userDetails = userDetailsResult.Data;
                var user = userDetails.User;

                // Determine user role
                var admins = await _adminServices.GetAllAdmins();
                var isAdmin = admins.Data?.Any(a => a.Id == id) ?? false;
                
                if (isAdmin)
                {
                    ViewBag.UserRole = "Admin";
                    ViewBag.CurrentRole = "Admin";
                }
                else
                {
                    ViewBag.UserRole = "Client";
                    ViewBag.CurrentRole = "Client";
                }

                // Pass additional data to view
                ViewBag.UserDetails = userDetails;
                ViewBag.Orders = userDetails.Orders;
                ViewBag.Reviews = userDetails.Reviews;
                ViewBag.Quotes = userDetails.Quotes;
                ViewBag.Wishlist = userDetails.Wishlist;

                return View("Details", user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdminAccountRequest request)
        {
            try
            {
                // Handle file upload
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files["ProfilePictureUrl"];
                    if (file != null && file.Length > 0)
                    {
                        request = request with { ProfilePictureUrl = file };
                    }
                }

                if (ModelState.IsValid)
                {
                    var result = await _adminServices.createAdminAccount(request);
                    
                    if (result.Succeeded)
                    {
                        TempData["Success"] = $"User created successfully as {request.UserRole}";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (result.Errors != null)
                        {
                            foreach (var error in result.Errors)
                            {
                                foreach (var errorMessage in error.Value)
                                {
                                    ModelState.AddModelError("", errorMessage);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", result.Message ?? "Failed to create user");
                        }
                    }
                }
                else
                {
                    // Log ModelState errors for debugging
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"ModelState Error: {modelError.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating user: " + ex.Message);
            }

            return View(request);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Try to find user in admins first
                var admins = await _adminServices.GetAllAdmins();
                var admin = admins.Data?.FirstOrDefault(a => a.Id == id);

                if (admin != null)
                {
                    ViewBag.UserRole = "Admin";
                    ViewBag.CurrentProfilePicture = admin.ProfilePictureUrl;
                    var updateRequest = new UpdateAdminAccountRequest
                    {
                        FirstName = admin.FirstName,
                        LastName = admin.LastName,
                        UserName = admin.UserName,
                        Email = admin.Email,
                        PhoneNumber = admin.PhoneNumber,
                        Nationality = admin.Nationality,
                        Gender = admin.Gender
                    };
                    return View("Edit", updateRequest);
                }

                // If not found in admins, try clients
                var clients = await _adminServices.GetAllClients();
                var client = clients.Data?.FirstOrDefault(c => c.Id == id);

                if (client != null)
                {
                    ViewBag.UserRole = "Client";
                    ViewBag.CurrentProfilePicture = client.ProfilePictureUrl;
                    var updateRequest = new UpdateAdminAccountRequest
                    {
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        UserName = client.UserName,
                        Email = client.Email,
                        PhoneNumber = client.PhoneNumber,
                        Nationality = client.Nationality,
                        Gender = client.Gender
                    };
                    return View("Edit", updateRequest);
                }

                TempData["Error"] = "User not found";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user for edit: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAdminAccountRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Map uploaded file (if any) to the request's IFormFile property
                    if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files["profilePictureFile"];
                        if (file != null && file.Length > 0)
                        {
                            request.ProfilePictureUrl = file;
                        }
                    }
                    var result = await _adminServices.UpdateAdminAccount(id, request);
                    
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "User updated successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (result.Errors != null)
                        {
                            foreach (var error in result.Errors)
                            {
                                foreach (var errorMessage in error.Value)
                                {
                                    ModelState.AddModelError("", errorMessage);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to update user");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating user: " + ex.Message);
            }

            return View(request);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Try to find user in admins first
                var admins = await _adminServices.GetAllAdmins();
                var admin = admins.Data?.FirstOrDefault(a => a.Id == id);

                if (admin != null)
                {
                    ViewBag.UserRole = "Admin";
                    ViewBag.CurrentRole = "Admin";
                    return View("Delete", admin);
                }

                // If not found in admins, try clients
                var clients = await _adminServices.GetAllClients();
                var client = clients.Data?.FirstOrDefault(c => c.Id == id);

                if (client != null)
                {
                    ViewBag.UserRole = "Client";
                    ViewBag.CurrentRole = "Client";
                    var vm = new GetAllAdminResponse(
                        client.Id,
                        client.FirstName,
                        client.LastName,
                        client.UserName,
                        client.Email,
                        client.Gender,
                        client.IsActive,
                        client.PhoneNumber,
                        client.Nationality,
                        client.ProfilePictureUrl,
                        client.CreatedAt,
                        client.UpdatedAt
                    );
                    return View("Delete", vm);
                }

                TempData["Error"] = "User not found";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user for deletion: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var deleteRequest = new DeleteAdminAccountRequest(id);
                var result = await _adminServices.DeleteAdminAccount(deleteRequest);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = "User deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting user: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/AssignRole/5
        public async Task<IActionResult> AssignRole(int id)
        {
            try
            {
                // Try to find user in admins first
                var admins = await _adminServices.GetAllAdmins();
                var admin = admins.Data?.FirstOrDefault(a => a.Id == id);

                if (admin != null)
                {
                    ViewBag.UserRole = "Admin";
                    ViewBag.CurrentRole = "Admin";
                    return View("AssignRole", admin);
                }

                // If not found in admins, try clients
                var clients = await _adminServices.GetAllClients();
                var client = clients.Data?.FirstOrDefault(c => c.Id == id);

                if (client != null)
                {
                    ViewBag.UserRole = "Client";
                    ViewBag.CurrentRole = "Client";
                    return View("AssignRole", client);
                }

                TempData["Error"] = "User not found";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user for role assignment: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/AssignRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int id, string role)
        {
            try
            {
                // Convert string to Roles enum
                if (!Enum.TryParse<Roles>(role, out var roleEnum))
                {
                    TempData["Error"] = "Invalid role specified";
                    return RedirectToAction(nameof(Index));
                }

                var assignRequest = new AssignRoleRequest(id, roleEnum);
                var result = await _adminServices.AssignRole(assignRequest);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Role assigned successfully. User is now {role}";
                }
                else
                {
                    TempData["Error"] = "Failed to assign role";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error assigning role: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/RemoveRole/5
        public async Task<IActionResult> RemoveRole(int id)
        {
            try
            {
                // Try to find user in admins first
                var admins = await _adminServices.GetAllAdmins();
                var admin = admins.Data?.FirstOrDefault(a => a.Id == id);

                if (admin != null)
                {
                    ViewBag.UserRole = "Admin";
                    ViewBag.CurrentRole = "Admin";
                    return View("RemoveRole", admin);
                }

                // If not found in admins, try clients
                var clients = await _adminServices.GetAllClients();
                var client = clients.Data?.FirstOrDefault(c => c.Id == id);

                if (client != null)
                {
                    ViewBag.UserRole = "Client";
                    ViewBag.CurrentRole = "Client";
                    return View("RemoveRole", client);
                }

                TempData["Error"] = "User not found";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user for role removal: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/RemoveRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(int id, string role)
        {
            try
            {
                // Convert string to Roles enum
                if (!Enum.TryParse<Roles>(role, out var roleEnum))
                {
                    TempData["Error"] = "Invalid role specified";
                    return RedirectToAction(nameof(Index));
                }

                var removeRequest = new RemoveRoleRequest(id, roleEnum);
                var result = await _adminServices.RemoveRole(removeRequest);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Role removed successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to remove role";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error removing role: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/ClientDetails/5
        public async Task<IActionResult> ClientDetails(int id)
        {
            try
            {
                var clientResult = await _adminServices.GetClientDetails(id);
                
                if (!clientResult.Succeeded)
                {
                    TempData["Error"] = clientResult.Message;
                    return RedirectToAction(nameof(Index));
                }

                var client = clientResult.Data;
                ViewBag.UserRole = "Client";
                ViewBag.CurrentRole = "Client";
                
                return View("Details", client);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading client details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
