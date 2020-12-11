﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer.Admin.Core.Entities.Clients;
using IdentityServer.Admin.Core.Entities.Enums;
using IdentityServer.Admin.Core.Extensions;
using IdentityServer.Admin.Infrastructure.Mappers;
using IdentityServer.Admin.Models.Client;
using IdentityServer.Admin.Services.Client;

namespace IdentityServer.Admin.Controllers
{
    public class ClientSecretController : BaseController
    {
        private readonly IClientSecretService _clientSecretService;

        public ClientSecretController(IClientSecretService clientSecretService)
        {
            _clientSecretService = clientSecretService;
        }

        public async Task<IActionResult> Index(int id, int? page)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Client");
            }

            var pagedClientSecrets = await _clientSecretService.GetPagedAsync(id, page ?? 1);

            var model = new ClientSecretModel
            {
                ClientId = pagedClientSecrets.ClientId,
                ClientName = pagedClientSecrets.ClientName
            };

            ViewBag.PagedClientSecrets = pagedClientSecrets;

            return View(model);
        }

        [NonAction]
        private static void HashClientSharedSecret(ClientSecretModel model)
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
        public async Task<IActionResult> Create(ClientSecretModel model)
        {
            HashClientSharedSecret(model);

            await _clientSecretService.InsertClientSecretAsync(ClientMappers.Mapper.Map<ClientSecret>(model));
            SuccessNotification($"客户端 【{model.ClientName}】 密钥添加成功", "成功");

            return RedirectToAction(nameof(Index), new { Id = model.ClientId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string name)
        {
            var clientSecret = await _clientSecretService.GetClientSecretById(id);

            if (clientSecret == null)
            {
                return RedirectToAction("Index", "Client");
            }

            var model = ClientMappers.Mapper.Map<ClientSecretModel>(clientSecret);

            model.ClientName = name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ClientSecretModel model)
        {
            var result = await _clientSecretService.DeleteClientSecretAsync(ClientMappers.Mapper.Map<ClientSecret>(model));

            if (result)
            {
                SuccessNotification($"客户端 【{model.ClientName}】 密钥删除成功", "成功");
            }

            return RedirectToAction(nameof(Index), new { Id = model.ClientId });
        }
    }
}
