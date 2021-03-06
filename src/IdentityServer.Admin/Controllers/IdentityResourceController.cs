﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer.Admin.Core.Entities.IdentityResource;
using IdentityServer.Admin.Core.Extensions;
using IdentityServer.Admin.Infrastructure.Mappers;
using IdentityServer.Admin.Models.IdentityResource;
using IdentityServer.Admin.Services.IdentityResource;
using IdentityServer.Admin.Services.Localization;

namespace IdentityServer.Admin.Controllers
{
    public class IdentityResourceController : BaseController
    {
        private readonly IIdentityResourceService _identityResourceService;
        private readonly ILocalizationService _localizationService;

        public IdentityResourceController(IIdentityResourceService identityResourceService,ILocalizationService localizationService)
        {
            _identityResourceService = identityResourceService;
            _localizationService = localizationService;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            var pagedIdentitySources = await _identityResourceService.GetPagedAsync(search, page ?? 1);

            ViewBag.Search = search;

            return View(pagedIdentitySources);
        }

        public IActionResult Create()
        {
            var model = new IdentityResourceModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityResourceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UserClaims = model.UserClaimsItems.Deserialize<List<string>>()?.Select(x => new IdentityResourceClaimModel
            {
                Type = x
            }).ToList();
            var insertedResult = await _identityResourceService.InsertIdentityResourceAsync(CommonMappers.Mapper.Map<IdentityResource>(model));
            if (insertedResult > 0)
            {
                SuccessNotification(await _localizationService.GetResourceAsync("IdentityResource.Added"));

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var identityResource = await _identityResourceService.GetIdentityResourceByIdAsync(id);

            if (identityResource == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(CommonMappers.Mapper.Map<IdentityResourceModel>(identityResource));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityResourceModel model)
        {
            if (model.Id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var identityResource = await _identityResourceService.GetIdentityResourceByIdAsync(model.Id);

            if (identityResource == null)
            {
                return RedirectToAction(nameof(Index));
            }

            model.UserClaims = model.UserClaimsItems.Deserialize<List<string>>()?.Select(x => new IdentityResourceClaimModel
            {
                Type = x
            }).ToList();

            identityResource = CommonMappers.Mapper.Map<IdentityResource>(model);

            var updatedResult = await _identityResourceService.UpdateIdentityResourceAsync(identityResource);

            if (updatedResult)
            {
                SuccessNotification(await _localizationService.GetResourceAsync("IdentityResource.Updated"));
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var identityResource = await _identityResourceService.GetIdentityResourceByIdAsync(id);

            if (identityResource == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = CommonMappers.Mapper.Map<IdentityResourceModel>(identityResource);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IdentityResourceModel model)
        {
            if (model.Id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _identityResourceService.DeleteIdentityResourceAsync(CommonMappers.Mapper.Map<IdentityResource>(model));

            if (result)
            {
                SuccessNotification(await _localizationService.GetResourceAsync("IdentityResource.Deleted"));
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
