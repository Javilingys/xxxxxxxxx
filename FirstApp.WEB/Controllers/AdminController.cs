﻿using AutoMapper;
using FirstApp.Application.DTOs;
using FirstApp.Application.Interfaces;
using FirstApp.Domain.Entities;
using FirstApp.Domain.Entities.Identity;
using FirstApp.WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApp.WEB.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(IProductService productService, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _productService = productService;
            _mapper = mapper;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _productService.GetProductsAsync());
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = new SelectList(await _productService.GetProductBrandsAsync(), "Id", "Name");
            ViewBag.Types = new SelectList(await _productService.GetProductTypesAsync(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductEditViewModel productCreateVM)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<ProductEditViewModel, ProductCreateDto>(productCreateVM);
                await _productService.CreateProduct(createDto);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _productService.GetProduct(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            var productBrands = await _productService.GetProductBrandsAsync();
            var productTypes = await _productService.GetProductTypesAsync();

            ViewBag.Brands = new SelectList(await _productService.GetProductBrandsAsync(), "Id", "Name");
            ViewBag.Types = new SelectList(await _productService.GetProductTypesAsync(), "Id", "Name");



            return View(new ProductEditViewModel
            {
                ProductBrandId = productBrands.Where(b => b.Name == product.ProductBrand).Select(b => b.Id).FirstOrDefault(),
                ProductTypeId = productTypes.Where(t => t.Name == product.ProductType).Select(t => t.Id).FirstOrDefault(),
                ProductDto = product
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<ProductEditViewModel, ProductCreateDto>(productViewModel);
                await _productService.UpdateProduct(id, createDto);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AnotherUseless/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProduct(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: AnotherUseless/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            await _productService.DeleteProduct(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
