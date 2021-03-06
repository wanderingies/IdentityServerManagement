﻿using System.Threading.Tasks;
using IdentityServer.Admin.Core.Constants;
using IdentityServer.Admin.Core.Dtos.ApiResource;
using IdentityServer.Admin.Core.Entities.ApiResource;
using IdentityServer.Admin.Dapper.Repositories.ApiResource;

namespace IdentityServer.Admin.Services.ApiResource
{
    public class ApiResourcePropertyService : IApiResourcePropertyService
    {
        private readonly IApiResourcePropertyRepository _repository;

        public ApiResourcePropertyService(IApiResourcePropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResourceProperty> GetApiResourcePropertyById(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetAsync(id);
        }

        public async Task<PagedApiResourcePropertyDto> GetPagedAsync(int apiResourceId, int page = 1, int pageSize = PageConstant.PageSize)
        {
            return await _repository.GetPagedAsync(apiResourceId, page, pageSize);
        }

        public async Task<int> InsertApiResourceProperty(ApiResourceProperty apiResourceProperty)
        {
            return await _repository.InsertAsync(apiResourceProperty);
        }

        public async Task<bool> UpdateApiResourceProperty(ApiResourceProperty apiResourceProperty)
        {
            return await _repository.DeleteAsync(apiResourceProperty);
        }

        public async Task<bool> DeleteApiResourceProperty(ApiResourceProperty apiResourceProperty)
        {
            return await _repository.DeleteAsync(apiResourceProperty);
        }
    }
}
