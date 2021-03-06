﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer.Admin.Core.Entities.ApiResource;
using IdentityServer.Admin.Core.Entities.Enums;
using IdentityServer.Admin.Core.Extensions;
using IdentityServer.Admin.Infrastructure.Mappers;
using IdentityServer.Admin.Models.ApiResource;
using IdentityServer.Admin.Services.ApiResource;
using IdentityServer.Admin.Services.Localization;

namespace IdentityServer.Admin.Controllers
{
    public class ApiResourceSecretController : BaseController
    {
        private readonly IApiResourceSecretService _apiResourceSecretService;
        private readonly ILocalizationService _localizationService;

        public ApiResourceSecretController(IApiResourceSecretService apiResourceSecretService, ILocalizationService localizationService)
        {
            _apiResourceSecretService = apiResourceSecretService;
            _localizationService = localizationService;
        }

        public async Task<IActionResult> Index(int apiResourceId, int? page)
        {
            var pagedApiResourceSecret = await _apiResourceSecretService.GetPagedAsync(apiResourceId, page ?? 1);

            ViewBag.PagedApiResourceSecret = pagedApiResourceSecret;

            return View(new ApiResourceSecretModel
            {
                ApiResourceId = pagedApiResourceSecret.ApiResourceId,
                ApiResourceName = pagedApiResourceSecret.ApiResourceName
            });
        }

        [NonAction]
        private static void HashClientSharedSecret(ApiResourceSecretModel model)
        {
            if (model.Type != "SharedSecret")
                return;
            if (model.HashTypeEnum == HashType.Sha256)
            {
                model.Value = model.Value.Sha256();
            }
            else
            {
                if (model.HashTypeEnum != HashType.Sha512)
                    return;
                model.Value = model.Value.Sha512();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiResourceSecretModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), new { apiResourceId = model.ApiResourceId });
            }

            HashClientSharedSecret(model);

            await _apiResourceSecretService.InsertApiResourceSecret(CommonMappers.Mapper.Map<ApiResourceSecret>(model));

            SuccessNotification(await _localizationService.GetResourceAsync("ApiResourceSecret.Added"));

            return RedirectToAction(nameof(Index), new { apiResourceId = model.ApiResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var apiResourceSecret = await _apiResourceSecretService.GetApiResourceSecretById(id);

            if (apiResourceSecret == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = CommonMappers.Mapper.Map<ApiResourceSecretModel>(apiResourceSecret);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApiResourceSecretModel model)
        {
            if (model.Id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _apiResourceSecretService.DeleteApiResourceSecret(CommonMappers.Mapper.Map<ApiResourceSecret>(model));

            if (result)
            {
                SuccessNotification(await _localizationService.GetResourceAsync("ApiResourceSecret.Deleted"));
                return RedirectToAction(nameof(Index), new { apiResourceId = model.ApiResourceId });
            }

            return View(model);
        }
    }
}
