﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NextGenSoftware.OASIS.API.Core.Enums;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.API.Core.Interfaces;
using NextGenSoftware.OASIS.API.Core.Managers;
using NextGenSoftware.OASIS.API.Core.Objects;

namespace NextGenSoftware.OASIS.API.ONODE.WebAPI.Controllers
{
    //  [Route("api/[search]")]
    [Route("api/[controller]")]
    [ApiController]

    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [EnableCors()]
    public class SearchController : OASISControllerBase
    {
        private SearchManager _SearchManager;

        public SearchController()
        {

        }

        private SearchManager SearchManager
        {
            get
            {
                if (_SearchManager == null)
                {
                    OASISResult<IOASISStorageProvider> result = OASISBootLoader.OASISBootLoader.GetAndActivateDefaultProvider();

                    if (result.IsError)
                        ErrorHandling.HandleError(ref result, string.Concat("Error calling OASISBootLoader.OASISBootLoader.GetAndActivateDefaultProvider(). Error details: ", result.Message), true, false, true);

                    _SearchManager = new SearchManager(result.Result);
                }

                return _SearchManager;
            }
        }

        [HttpGet("{searchParams}")]
        public async Task<OASISResult<ISearchResults>> Get(SearchParams searchParams)
        {
            return await SearchManager.SearchAsync(searchParams);
        }

        [HttpGet("{searchParams}/{providerType}/{setGlobally}")]
        public async Task<OASISResult<ISearchResults>> Get(SearchParams searchParams, ProviderType providerType, bool setGlobally = false)
        {
            GetAndActivateProvider(providerType, setGlobally);
            return await Get(searchParams);
        }
    }
}
