﻿﻿﻿﻿﻿﻿﻿﻿using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PublishersController : Controller
    {
        private readonly IPublisherServices _publisherService;
        private readonly IConfiguration _configuration;

        public PublishersController(IPublisherServices publisherService, IConfiguration configuration)
        {
            _publisherService = publisherService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var request = new GetAllPublishersPaginatedRequest(pageNumber, pageSize, search);
            var result = await _publisherService.GetAllPublishersPaginatedAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Failed to load publishers";
                return View(new List<GetAllPublisherPaginatedResponse>());
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.SearchTerm = search;
            return View(result.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = new GetPublisherByIdRequest(id);
            var result = await _publisherService.GetPublisherByIdAsync(request);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Publisher not found";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPublisherRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Extract English fields from form
                    var nameEn = Request.Form["EnglishName"].ToString();
                    var descriptionEn = Request.Form["EnglishDescription"].ToString();
                    
                    // Create new request with English fields
                    var updatedRequest = request with 
                    { 
                        Name_en = !string.IsNullOrWhiteSpace(nameEn) ? nameEn : null,
                        Description_en = !string.IsNullOrWhiteSpace(descriptionEn) ? descriptionEn : null
                    };
                    
                    var result = await _publisherService.AddPublisherAsync(updatedRequest);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Publisher created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Handle validation errors from FluentValidation
                    if (result.Errors != null && result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            foreach (var error2 in error.Value)
                                ModelState.AddModelError(string.Empty, error2);
                        }
                    }
                    else
                    {
                        TempData["Error"] = result.Message ?? "Failed to create publisher. Please check your input and try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the publisher. Please try again.";
                // Log the exception if you have logging configured
            }
            
            return View(request);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _publisherService.GetPublisherByIdAsync(new GetPublisherByIdRequest(id));

            if (!response.Succeeded || response.Data == null)
            {
                return NotFound();
            }


            var request = new UpdatePublisherResponse(
                response.Data.Id,
                response.Data.Name,
                response.Data.Name_en,
                response.Data.Description,
                response.Data.Description_en,
                response.Data.LogoUrl,
                response.Data.ContactEmail
            );


            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePublisherRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Extract English fields from form
                    var nameEn = Request.Form["EnglishName"].ToString();
                    var descriptionEn = Request.Form["EnglishDescription"].ToString();
                    
                    // Create new request with English fields
                    var updatedRequest = request with 
                    { 
                        Name_en = !string.IsNullOrWhiteSpace(nameEn) ? nameEn : null,
                        Description_en = !string.IsNullOrWhiteSpace(descriptionEn) ? descriptionEn : null
                    };
                    
                    var result = await _publisherService.UpdatePublisherAsync(updatedRequest);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Publisher updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    // Handle validation errors from FluentValidation
                    if (result.Errors != null && result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            foreach (var error2 in error.Value)
                                ModelState.AddModelError(string.Empty, error2);
                        }
                    }
                    else
                    {
                        TempData["Error"] = result.Message ?? "Failed to update publisher. Please check your input and try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the publisher. Please try again.";
                // Log the exception if you have logging configured
            }
            
            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(new GetPublisherByIdRequest(id));
            if (!publisher.Succeeded || publisher.Data == null)
            {
                return NotFound();
            }

            return View(publisher.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _publisherService.DeletePublisherAsync(new DeletePublisherRequest(id));
            if (result.Succeeded)
            {
                TempData["Success"] = "Publisher deleted successfully.";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}