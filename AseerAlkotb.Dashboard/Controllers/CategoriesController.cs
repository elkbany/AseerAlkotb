using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryServices _categoryServices;

        public CategoriesController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        // GET: Categories
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var request = new GetAllCategoriesPaginatedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search
            };

            var result = await _categoryServices.GetAllCategoriesPaginatedAsync(request);
            
            if (result.Succeeded)
            {
                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.SearchTerm = search;
                return View(result.Data);
            }

            TempData["Error"] = "حدث خطأ أثناء جلب التصنيفات";
            return View(new List<GetAllCategoriesPaginatedResponse>());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var request = new GetCategoryByIdRequest(id);
            var result = await _categoryServices.GetCategoryByIdAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Category not found";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            // Get parent categories for dropdown
            var parentCategoriesRequest = new GetAllCategoriesPaginatedRequest
            {
                PageNumber = 1,
                PageSize = 100,
                Search = ""
            };
            
            var parentCategories = await _categoryServices.GetAllCategoriesPaginatedAsync(parentCategoriesRequest);
            ViewBag.ParentCategories = new SelectList(parentCategories.Data, "Id", "Name");
            
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCategoryRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryServices.AddCategoryAsync(request);
                
                if (result.Succeeded)
                {
                    try
                    {
                        var id = result.Data.Id;
                        var keyName = $"Category_{id}_Name";
                        var arName = Request.Form["Name"].ToString();
                        var enName = Request.Form["EnglishName"].ToString();

                        // Fallbacks: if English not provided, reuse Arabic for both
                        if (string.IsNullOrWhiteSpace(enName)) enName = arName;

                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource(keyName, arName, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource(keyName, enName, "en");

                        // Description keys if provided
                        var descAr = Request.Form["Description"].ToString();
                        var descEn = Request.Form["EnglishDescription"].ToString();
                        if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
                        {
                            var keyDesc = $"Category_{id}_Description";
                            if (string.IsNullOrWhiteSpace(descEn)) descEn = descAr;
                            if (string.IsNullOrWhiteSpace(descAr)) descAr = descEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource(keyDesc, descAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource(keyDesc, descEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
                    TempData["Success"] = "تم إضافة التصنيف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = result.Message ?? "حدث خطأ أثناء إضافة التصنيف";
            }

            // Reload parent categories for dropdown
            var parentCategoriesRequest = new GetAllCategoriesPaginatedRequest
            {
                PageNumber = 1,
                PageSize = 100,
                Search = ""
            };
            
            var parentCategories = await _categoryServices.GetAllCategoriesPaginatedAsync(parentCategoriesRequest);
            ViewBag.ParentCategories = new SelectList(parentCategories.Data, "Id", "Name");
            
            return View(request);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var request = new GetCategoryByIdRequest(id);
            var result = await _categoryServices.GetCategoryByIdAsync(request);
            
            if (result.Succeeded)
            {
                var updateRequest = new UpdateCategoryRequest(
                    result.Data.Id,
                    result.Data.Name,
                    result.Data.Description,
                    result.Data.IsActive
                );

                // Get parent categories for dropdown
                var parentCategoriesRequest = new GetAllCategoriesPaginatedRequest
                {
                    PageNumber = 1,
                    PageSize = 100,
                    Search = ""
                };
                
                var parentCategories = await _categoryServices.GetAllCategoriesPaginatedAsync(parentCategoriesRequest);
                ViewBag.ParentCategories = new SelectList(parentCategories.Data, "Id", "Name");
                
                return View(updateRequest);
            }

            TempData["Error"] = "التصنيف غير موجود";
            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryRequest request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _categoryServices.UpdateCategoryAsync(request);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = "تم تحديث التصنيف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                
                TempData["Error"] = result.Message ?? "حدث خطأ أثناء تحديث التصنيف";
            }

            // Reload parent categories for dropdown
            var parentCategoriesRequest = new GetAllCategoriesPaginatedRequest
            {
                PageNumber = 1,
                PageSize = 100,
                Search = ""
            };
            
            var parentCategories = await _categoryServices.GetAllCategoriesPaginatedAsync(parentCategoriesRequest);
            ViewBag.ParentCategories = new SelectList(parentCategories.Data, "Id", "Name");
            
            return View(request);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var request = new GetCategoryByIdRequest(id);
            var result = await _categoryServices.GetCategoryByIdAsync(request);
            
            if (result.Succeeded)
            {
                return View(result.Data);
            }

            TempData["Error"] = "التصنيف غير موجود";
            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = new DeleteCategoryRequest(id);
            var result = await _categoryServices.DeleteCategoryAsync(request);
            
            if (result.Succeeded)
            {
                TempData["Success"] = "تم حذف التصنيف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["Error"] = result.Message ?? "حدث خطأ أثناء حذف التصنيف";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/SubCategories/5
        public async Task<IActionResult> SubCategories(int parentId, int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var request = new GetAllSubCategoriesPaginatedRequest(
                parentId,
                pageNumber,
                pageSize,
                search
            );

            var result = await _categoryServices.GetAllSubCategoriesPaginatedAsync(request);
            
            if (result.Succeeded)
            {
                // Get parent category info
                var parentCategoryRequest = new GetCategoryByIdRequest(parentId);
                var parentCategory = await _categoryServices.GetCategoryByIdAsync(parentCategoryRequest);
                
                ViewBag.ParentCategoryId = parentId;
                ViewBag.ParentCategoryName = parentCategory.Succeeded ? parentCategory.Data.Name : "غير محدد";
                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.SearchTerm = search;
                return View(result.Data);
            }

            TempData["Error"] = "حدث خطأ أثناء جلب التصنيفات الفرعية";
            return View(new List<GetAllSubCategoriesPaginatedResponse>());
        }

        // GET: Categories/CreateSubCategory
        public async Task<IActionResult> CreateSubCategory(int parentId)
        {
            var parentCategoryRequest = new GetCategoryByIdRequest(parentId);
            var parentCategory = await _categoryServices.GetCategoryByIdAsync(parentCategoryRequest);
            
            if (parentCategory.Succeeded)
            {
                ViewBag.ParentCategory = parentCategory.Data;
                return View();
            }

            TempData["Error"] = "التصنيف الأب غير موجود";
            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/CreateSubCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubCategory(int parentId, AddSubCategoryRequest request)
        {
            if (ModelState.IsValid)
            {
                request = request with { ParentCategoryId = parentId };
                var result = await _categoryServices.AddSubCategoryAsync(request);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = "تم إضافة التصنيف الفرعي بنجاح";
                    return RedirectToAction(nameof(SubCategories), new { parentId });
                }
                
                TempData["Error"] = result.Message ?? "حدث خطأ أثناء إضافة التصنيف الفرعي";
            }

            var parentCategoryRequest = new GetCategoryByIdRequest(parentId);
            var parentCategory = await _categoryServices.GetCategoryByIdAsync(parentCategoryRequest);
            ViewBag.ParentCategory = parentCategory.Data;
            
            return View(request);
        }
    }
} 