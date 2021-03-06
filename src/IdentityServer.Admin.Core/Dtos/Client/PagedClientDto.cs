﻿
namespace IdentityServer.Admin.Core.Dtos.Client
{
    public class PagedClientDto : BasePagedDto<ClientPagedListForPage>
    {
    }

    public class ClientPagedListForPage
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }
    }
}
