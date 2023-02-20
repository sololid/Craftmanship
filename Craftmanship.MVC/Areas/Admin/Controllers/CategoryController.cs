using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;
using Craftmanship.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Craftmanship.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categories category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.Save();
                TempData["Success"] = "Ny kategori är skapad!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var categoryDetalis = _unitOfWork.Category.GetFirstOrDefalut(x => x.Id == id);

            var result = new Categories()
            {
                Id = categoryDetalis.Id,
                Name = categoryDetalis.Name
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Categories category)
        {
            if (id != category.Id) return View();

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                await _unitOfWork.Save();
                TempData["success"] = "Kategorin är uppdaterad!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = _unitOfWork.Category.GetFirstOrDefalut(x => x.Id == id);
            if (id == 0) return View("NotFound");
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = _unitOfWork.Category.GetFirstOrDefalut(x => x.Id == id);
            await _unitOfWork.Category.DeleteAsync(id);
            await _unitOfWork.Save();
            TempData["error"] = "Kategorin är raderad";
            return RedirectToAction("Index");
        }
    }
}
