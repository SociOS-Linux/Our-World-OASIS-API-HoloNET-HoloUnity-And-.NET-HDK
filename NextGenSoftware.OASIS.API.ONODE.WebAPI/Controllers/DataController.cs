﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextGenSoftware.OASIS.API.Core.Interfaces;
using NextGenSoftware.OASIS.API.Core.Enums;
using NextGenSoftware.OASIS.API.Core.Managers;
using NextGenSoftware.OASIS.API.Core.Holons;
using NextGenSoftware.OASIS.API.Core.Helpers;
using NextGenSoftware.OASIS.API.ONODE.WebAPI.Helpers;
using NextGenSoftware.OASIS.API.ONODE.WebAPI.Models.Data;
using NextGenSoftware.OASIS.API.ONODE.WebAPI.Models;

namespace NextGenSoftware.OASIS.API.ONODE.WebAPI.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataController : OASISControllerBase
    {
        //  OASISDNA _settings;
        HolonManager _holonManager = null;

        HolonManager HolonManager
        {
            get
            {
                if (_holonManager == null)
                {
                    OASISResult<IOASISStorageProvider> result = OASISBootLoader.OASISBootLoader.GetAndActivateDefaultProvider();

                    if (result.IsError)
                        ErrorHandling.HandleError(ref result, string.Concat("Error calling OASISBootLoader.OASISBootLoader.GetAndActivateDefaultProvider(). Error details: ", result.Message), true, false, true);

                    _holonManager = new HolonManager(result.Result);
                }

                return _holonManager;
            }
        }

        public DataController()
        {

        }

        /// <summary>
        /// Load's a holon data object for the given id.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("load-holon")]
        public async Task<OASISHttpResponseMessage<Holon>> LoadHolon(LoadHolonRequest request)
        {
            OASISResult<Holon> response = new OASISResult<Holon>();
            OASISConfigResult<Holon> configResult = ConfigureOASISEngine<Holon>(request);

            if (configResult.IsError && configResult.Response != null)
                return configResult.Response;

            OASISResult<IHolon> result = await HolonManager.LoadHolonAsync(request.Id, request.LoadChildren, request.Recursive, request.MaxChildDepth, request.ContinueOnError, request.Version);
            ResetOASISSettings(request, configResult);

            OASISResultHelper<IHolon, Holon>.CopyResult(result, response);
            response.Result = (Holon)result.Result;

            return HttpResponseHelper.FormatResponse(response, System.Net.HttpStatusCode.OK, request.ShowDetailedSettings);
        }


        /// <summary>
        /// Load's a holon data object for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-holon/{id}")]
        public async Task<OASISHttpResponseMessage<Holon>> LoadHolon(Guid id)
        {
            return await LoadHolon(new LoadHolonRequest() { Id = id });
        }

        /// <summary>
        /// Load's a holon data object for the given id.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-holon/{id}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}")]
        public async Task<OASISHttpResponseMessage<Holon>> LoadHolon(Guid id, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0)
        {
            return await LoadHolon(new LoadHolonRequest()
            {
                Id = id,
                LoadChildren = loadChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                Version = version
            });
        }

        /// <summary>
        /// Load's a holon data object for the given id.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// Pass in the provider you wish to use.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="version"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-holon/{id}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}")]
        public async Task<OASISHttpResponseMessage<Holon>> LoadHolon(Guid id, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "", bool setGlobally = false)
        {
            return await LoadHolon(new LoadHolonRequest()
            {
                Id = id,
                LoadChildren = loadChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                Version = version,
                ProviderType = providerType,
                SetGlobally = setGlobally
            });
        }


      
        /// <summary>
        /// Load's a holon data object for the given id.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loadChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="version"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <param name="autoFailOverMode"></param>
        /// <param name="autoReplicationMode"></param>
        /// <param name="autoLoadBalanceMode"></param>
        /// <param name="autoFailOverProviders"></param>
        /// <param name="autoReplicationProviders"></param>
        /// <param name="autoLoadBalanceProviders"></param>
        /// <param name="waitForAutoReplicationResult"></param>
        /// <param name="showDetailedSettings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-holon/{id}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}/{autoReplicationMode}/{autoFailOverMode}/{autoLoadBalanceMode}/{autoReplicationProviders}/{autoFailOverProviders}/{autoLoadBalanceProviders}/{waitForAutoReplicationResult}/{showDetailedSettings}")]
        public async Task<OASISHttpResponseMessage<Holon>> LoadHolon(Guid id, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "Default", bool setGlobally = false, string autoReplicationMode = "DEFAULT", string autoFailOverMode = "DEFAULT", string autoLoadBalanceMode = "DEFAULT", string autoReplicationProviders = "DEFAULT", string autoFailOverProviders = "DEFAULT", string autoLoadBalanceProviders = "DEFAULT", bool waitForAutoReplicationResult = false, bool showDetailedSettings = false)
        {
            return await LoadHolon(new LoadHolonRequest()
            {
                Id = id,
                LoadChildren = loadChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                Version = version,
                ProviderType = providerType,
                SetGlobally = setGlobally,
                AutoReplicationMode = autoReplicationMode,
                AutoFailOverMode = autoFailOverMode,
                AutoLoadBalanceMode = autoLoadBalanceMode,
                AutoReplicationProviders = autoReplicationProviders,
                AutoFailOverProviders = autoFailOverProviders,
                AutoLoadBalanceProviders = autoLoadBalanceProviders,
                WaitForAutoReplicationResult = waitForAutoReplicationResult,
                ShowDetailedSettings = showDetailedSettings
            });
        }

        /// <summary>
        /// Load's all holons for the given HolonType. Use 'All' to load all holons.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("load-all-holons")]
        public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadAllHolons(LoadAllHolonsRequest request)
        {
            OASISHttpResponseMessage<IEnumerable<Holon>> response;
            (response, HolonType holonType) = ValidateHolonType<IEnumerable<Holon>>(request.HolonType);

            if (response.Result.IsError)
                return response;

            OASISConfigResult<IEnumerable<Holon>> configResult = ConfigureOASISEngine<IEnumerable<Holon>>(request);

            if (configResult.IsError && configResult.Response != null)
                return configResult.Response;

            OASISResult<IEnumerable<IHolon>> result = await HolonManager.LoadAllHolonsAsync(holonType, request.LoadChildren, request.Recursive, request.MaxChildDepth, request.ContinueOnError, request.Version);

            OASISResultHelper<IEnumerable<IHolon>, IEnumerable<Holon>>.CopyResult(result, response.Result);
            response.Result.Result = Mapper.Convert(result.Result);
            ResetOASISSettings(request, configResult);

            return HttpResponseHelper.FormatResponse(response, System.Net.HttpStatusCode.OK, request.ShowDetailedSettings);
        }


        /// <summary>
        /// Load's all holons for the given HolonType. Use 'All' to load all holons.
        /// </summary>
        /// <param name="holonType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-all-holons/{holonType}")]
        public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadAllHolons(string holonType)
        {
            return await LoadAllHolons(new LoadAllHolonsRequest() { HolonType = holonType });
            //return await LoadAllHolons(holonType, true, true, 0, true, 0);
        }

        /// <summary>
        /// Load's all holons for the given HolonType. Use 'All' to load all holons.
        /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
        /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
        /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
        /// </summary>
        /// <param name="holonType"></param>
        /// <param name="loadChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("load-all-holons/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}")]
        public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadAllHolons(string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0)
        {
            return await LoadAllHolons(new LoadAllHolonsRequest()
            {
                HolonType = holonType,
                LoadChildren = loadChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                Version = version
            });

            //OASISResult<IEnumerable<Holon>> response = new OASISResult<IEnumerable<Holon>>();
            //OASISResult<IEnumerable<IHolon>> result = await HolonManager.LoadAllHolonsAsync(holonType, loadChildren, recursive, maxChildDepth, continueOnError, version);

            //OASISResultHelper<IEnumerable<IHolon>, IEnumerable<Holon>>.CopyResult(result, response);
            //response.Result = Mapper.Convert(result.Result);

            //return HttpResponseHelper.FormatResponse(response);
        }

       
      /// <summary>
      /// Load's all holons for the given HolonType. Use 'All' to load all holons.
      /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
      /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
      /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
      /// Pass in the provider you wish to use.
      /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
      /// </summary>
      /// <param name="holonType"></param>
      /// <param name="loadChildren"></param>
      /// <param name="recursive"></param>
      /// <param name="maxChildDepth"></param>
      /// <param name="continueOnError"></param>
      /// <param name="version"></param>
      /// <param name="providerType">Pass in the provider you wish to use.</param>
      /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
      /// <returns></returns>
      [Authorize]
      [HttpGet("load-all-holons/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}")]
      public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadAllHolons(string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "Default", bool setGlobally = false)
      {
          return await LoadAllHolons(new LoadAllHolonsRequest()
          {
              HolonType = holonType,
              LoadChildren = loadChildren,
              Recursive = recursive,
              MaxChildDepth = maxChildDepth,
              ContinueOnError = continueOnError,
              Version = version,
              ProviderType = providerType,
              SetGlobally = setGlobally
          });
      }

      /// <summary>
      /// Load's all holons for the given HolonType. Use 'All' to load all holons.
      /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
      /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
      /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
      /// Pass in the provider you wish to use.
      /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
      /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
      /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
      /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
      /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
      /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
      /// </summary>
      /// <param name="holonType"></param>
      /// <param name="loadChildren"></param>
      /// <param name="recursive"></param>
      /// <param name="maxChildDepth"></param>
      /// <param name="continueOnError"></param>
      /// <param name="version"></param>
      /// <param name="providerType">Pass in the provider you wish to use.</param>
      /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
      /// <param name="autoFailOverMode"></param>
      /// <param name="autoReplicationMode"></param>
      /// <param name="autoLoadBalanceMode"></param>
      /// <param name="autoFailOverProviders"></param>
      /// <param name="autoReplicationProviders"></param>
      /// <param name="autoLoadBalanceProviders"></param>
      /// <param name="waitForAutoReplicationResult"></param>
      /// <param name="showDetailedSettings"></param>
      /// <returns></returns>
      [Authorize]
      [HttpGet("load-all-holons/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}/{autoReplicationMode}/{autoFailOverMode}/{autoLoadBalanceMode}/{autoReplicationProviders}/{autoFailOverProviders}/{autoLoadBalanceProviders}/{waitForAutoReplicationResult}/{showDetailedSettings}")]
      public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadAllHolons(string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "Default", bool setGlobally = false, string autoReplicationMode = "DEFAULT", string autoFailOverMode = "DEFAULT", string autoLoadBalanceMode = "DEFAULT", string autoReplicationProviders = "DEFAULT", string autoFailOverProviders = "DEFAULT", string autoLoadBalanceProviders = "DEFAULT", bool waitForAutoReplicationResult = false, bool showDetailedSettings = false)
      {
          return await LoadAllHolons(new LoadAllHolonsRequest()
          {
              HolonType = holonType,
              LoadChildren = loadChildren,
              Recursive = recursive,
              MaxChildDepth = maxChildDepth,
              ContinueOnError = continueOnError,
              Version = version,
              ProviderType = providerType,
              SetGlobally = setGlobally,
              AutoReplicationMode = autoReplicationMode,
              AutoFailOverMode = autoFailOverMode,
              AutoLoadBalanceMode = autoLoadBalanceMode,
              AutoReplicationProviders = autoReplicationProviders,
              AutoFailOverProviders = autoFailOverProviders,
              AutoLoadBalanceProviders = autoLoadBalanceProviders,
              WaitForAutoReplicationResult = waitForAutoReplicationResult,
              ShowDetailedSettings = showDetailedSettings
          });
      }

      /// <summary>
      /// Load's all holons for the given parent and the given HolonType. Use 'All' to load all holons.
      /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
      /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
      /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
      /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
      /// Pass in the provider you wish to use.
      /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
      /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
      /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
      /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
      /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
      /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
      /// </summary>
      /// <returns></returns>
      [Authorize]
      [HttpPost("load-holons-for-parent")]
      public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadHolonsForParent(LoadHolonsForParentRequest request)
      {
          OASISHttpResponseMessage<IEnumerable<Holon>> response;
          (response, HolonType holonType) = ValidateHolonType<IEnumerable<Holon>>(request.HolonType);

          if (response.Result.IsError)
              return response;

          OASISConfigResult<IEnumerable<Holon>> configResult = ConfigureOASISEngine<IEnumerable<Holon>>(request);

          if (configResult.IsError && configResult.Response != null)
              return configResult.Response;

          OASISResult<IEnumerable<IHolon>> result = await HolonManager.LoadHolonsForParentAsync(request.Id, holonType, request.LoadChildren, request.Recursive, request.MaxChildDepth, request.ContinueOnError, request.Version);

          OASISResultHelper<IEnumerable<IHolon>, IEnumerable<Holon>>.CopyResult(result, response.Result);
          response.Result.Result = Mapper.Convert(result.Result);
          ResetOASISSettings(request, configResult);

          return HttpResponseHelper.FormatResponse(response, System.Net.HttpStatusCode.OK, request.ShowDetailedSettings);

          //(OASISHttpResponseMessage<IEnumerable<Holon>> response, HolonType holonType) = ValidateHolonType<IEnumerable<Holon>>(request.HolonType);

          //if (response.Result.IsError)
          //    return response;

          //return await LoadHolonsForParent(request.Id, holonType, request.LoadChildren, request.Recursive, request.MaxChildDepth, request.ContinueOnError, request.Version);
      }

      /// <summary>
      /// Load's all holons for the given parent and the given HolonType. Use 'All' to load all holons.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="holonType"></param>
      /// <returns></returns>
      [Authorize]
      [HttpGet("load-holons-for-parent/{id}/{holonType}")]
      public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadHolonsForParent(Guid id, string holonType)
      {
          return await LoadHolonsForParent(new LoadHolonsForParentRequest() { Id = id, HolonType = holonType });
          //return await LoadHolonsForParent(id, holonType, true, true, 0, true, 0);
      }


      
     /// <summary>
     /// Load's all holons for the given parent and the given HolonType. Use 'All' to load all holons.
     /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
     /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
     /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
     /// </summary>
     /// <param name="id"></param>
     /// <param name="holonType"></param>
     /// <param name="loadChildren"></param>
     /// <param name="recursive"></param>
     /// <param name="maxChildDepth"></param>
     /// <param name="continueOnError"></param>
     /// <param name="version"></param>
     /// <returns></returns>
     [Authorize]
     [HttpGet("load-holons-for-parent/{id}/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}")]
     public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadHolonsForParent(Guid id, string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0)
     {
         return await LoadHolonsForParent(new LoadHolonsForParentRequest()
         {
             Id = id,
             HolonType = holonType,
             LoadChildren = loadChildren,
             Recursive = recursive,
             MaxChildDepth = maxChildDepth,
             ContinueOnError = continueOnError,
             Version = version
         });

         //OASISResult<IEnumerable<Holon>> response = new OASISResult<IEnumerable<Holon>>();
         //OASISResult<IEnumerable<IHolon>> result = await HolonManager.LoadHolonsForParentAsync(id, holonType, loadChildren, recursive, maxChildDepth, continueOnError, version);

         //OASISResultHelper<IEnumerable<IHolon>, IEnumerable<Holon>>.CopyResult(result, response);
         //response.Result = Mapper.Convert(result.Result);

         //return HttpResponseHelper.FormatResponse(response);
     }

     /// <summary>
     /// Load's all holons for the given parent and the given HolonType. Use 'All' to load all holons.
     /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
     /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
     /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
     /// Pass in the provider you wish to use.
     /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
     /// </summary>
     /// <param name="id"></param>
     /// <param name="holonType"></param>
     /// <param name="loadChildren"></param>
     /// <param name="recursive"></param>
     /// <param name="maxChildDepth"></param>
     /// <param name="continueOnError"></param>
     /// <param name="version"></param>
     /// <param name="providerType">Pass in the provider you wish to use.</param>
     /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
     /// <returns></returns>
     [Authorize]
     [HttpGet("load-holons-for-parent/{id}/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}")]
     public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadHolonsForParent(Guid id, string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "Default", bool setGlobally = false)
     {
         return await LoadHolonsForParent(new LoadHolonsForParentRequest()
         {
             Id = id,
             HolonType = holonType,
             LoadChildren = loadChildren,
             Recursive = recursive,
             MaxChildDepth = maxChildDepth,
             ContinueOnError = continueOnError,
             Version = version,
             ProviderType = providerType,
             SetGlobally = setGlobally
         });

         //GetAndActivateProvider(providerType, setGlobally);
         //return await LoadHolonsForParent(id, holonType, loadChildren, recursive, maxChildDepth, continueOnError, 0);
     }

     /// <summary>
     /// Load's all holons for the given parent and the given HolonType. Use 'All' to load all holons.
     /// Set the loadChildren flag to true to load all the holon's child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the Recursive flag to true to load all the child's holon's recursively, or false to only load the first level of child holon's. This defaults to true.
     /// If loadChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to load, it defaults to 0, which means it will load to infinite depth.
     /// Set the continueOnError flag to true if you wish it to continue loading child holon's even if an error has occured, this defaults to true.
     /// Set the Version int to the version of the holon you wish to load (defaults to 0) which means the latest version.
     /// Pass in the provider you wish to use.
     /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
     /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
     /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
     /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
     /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
     /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
     /// </summary>
     /// <param name="id"></param>
     /// <param name="holonType"></param>
     /// <param name="loadChildren"></param>
     /// <param name="recursive"></param>
     /// <param name="maxChildDepth"></param>
     /// <param name="continueOnError"></param>
     /// <param name="version"></param>
     /// <param name="providerType">Pass in the provider you wish to use.</param>
     /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
     /// <param name="autoFailOverMode"></param>
     /// <param name="autoReplicationMode"></param>
     /// <param name="autoLoadBalanceMode"></param>
     /// <param name="autoFailOverProviders"></param>
     /// <param name="autoReplicationProviders"></param>
     /// <param name="autoLoadBalanceProviders"></param>
     /// <param name="waitForAutoReplicationResult"></param>
     /// <param name="showDetailedSettings"></param>
     /// <returns></returns>
     [Authorize]
     [HttpGet("load-holons-for-parent/{id}/{holonType}/{loadChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{version}/{providerType}/{setGlobally}/{autoReplicationMode}/{autoFailOverMode}/{autoLoadBalanceMode}/{autoReplicationProviders}/{autoFailOverProviders}/{autoLoadBalanceProviders}/{waitForAutoReplicationResult}/{showDetailedSettings}")]
     public async Task<OASISHttpResponseMessage<IEnumerable<Holon>>> LoadHolonsForParent(Guid id, string holonType, bool loadChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, int version = 0, string providerType = "Default", bool setGlobally = false, string autoReplicationMode = "DEFAULT", string autoFailOverMode = "DEFAULT", string autoLoadBalanceMode = "DEFAULT", string autoReplicationProviders = "DEFAULT", string autoFailOverProviders = "DEFAULT", string autoLoadBalanceProviders = "DEFAULT", bool waitForAutoReplicationResult = false, bool showDetailedSettings = true)
     {
         return await LoadHolonsForParent(new LoadHolonsForParentRequest()
         {
             Id = id,
             HolonType = holonType,
             LoadChildren = loadChildren,
             Recursive = recursive,
             MaxChildDepth = maxChildDepth,
             ContinueOnError = continueOnError,
             Version = version,
             ProviderType = providerType,
             SetGlobally = setGlobally,
             AutoReplicationMode = autoReplicationMode,
             AutoFailOverMode = autoFailOverMode,
             AutoLoadBalanceMode = autoLoadBalanceMode,
             AutoReplicationProviders = autoReplicationProviders,
             AutoFailOverProviders = autoFailOverProviders,
             AutoLoadBalanceProviders = autoLoadBalanceProviders,
             WaitForAutoReplicationResult = waitForAutoReplicationResult,
             ShowDetailedSettings = showDetailedSettings
         });
     }

     /// <summary>
     /// Save's a holon data object.
     /// Set the saveChildren flag to true to save all the holon's child holon's. This defaults to true.
     /// If saveChildren is set to true, you can set the Recursive flag to true to save all the child's holon's recursively, or false to only save the first level of child holon's. This defaults to true.
     /// If saveChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to save, it defaults to 0, which means it will save to infinite depth.
     /// Set the continueOnError flag to true if you wish it to continue saving child holon's even if an error has occured, this defaults to true.
     /// Pass in the provider you wish to use.
     /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
     /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
     /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
     /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
     /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
     /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
     /// </summary>
     /// <returns></returns>
     [Authorize]
     [HttpPost("save-holon")]
     public async Task<OASISHttpResponseMessage<IHolon>> SaveHolon(SaveHolonRequest request)
     {
         OASISConfigResult<IHolon> configResult = ConfigureOASISEngine<IHolon>(request);

         if (configResult.IsError && configResult.Response != null)
             return configResult.Response;

         OASISResult<IHolon> response = await HolonManager.SaveHolonAsync(request.Holon, request.SaveChildren, request.Recursive, request.MaxChildDepth, request.ContinueOnError);
         ResetOASISSettings(request, configResult);

         return HttpResponseHelper.FormatResponse(response, System.Net.HttpStatusCode.OK, request.ShowDetailedSettings);

         //OASISResult<Holon> response = new OASISResult<Holon>();
         //OASISResult<IHolon> result = await HolonManager.SaveHolonAsync(request.Holon);

         //OASISResultHelper<IHolon, Holon>.CopyResult(result, response);
         //response.Result = (Holon)result.Result;

         //return HttpResponseHelper.FormatResponse(response);
     }

       
   /// <summary>
   /// Save's a holon data object.
   /// </summary>
   /// <param name="holon"></param>
   /// <returns></returns>
   [Authorize]
   [HttpPost("save-holon/{holon}")]
   public async Task<OASISHttpResponseMessage<IHolon>> SaveHolon(Holon holon)
   {
       return await SaveHolon(new SaveHolonRequest() { Holon = holon });

       //OASISResult<Holon> response = new OASISResult<Holon>();
       //OASISResult<IHolon> result = await HolonManager.SaveHolonAsync(holon);

       //OASISResultHelper<IHolon, Holon>.CopyResult(result, response);
       //response.Result = (Holon)result.Result;

       //return HttpResponseHelper.FormatResponse(response);
   }



        /// <summary>
        /// Save's a holon data object.
        /// Set the saveChildren flag to true to save all the holon's child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the Recursive flag to true to save all the child's holon's recursively, or false to only save the first level of child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to save, it defaults to 0, which means it will save to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue saving child holon's even if an error has occured, this defaults to true.
        /// </summary>
        /// <param name="saveChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="holon"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("save-holon/{saveChildren}/{recursive}/{maxChildDepth}/{continueOnError}")]
        public async Task<OASISHttpResponseMessage<IHolon>> SaveHolon(Holon holon, bool saveChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true)
        {
            return await SaveHolon(new SaveHolonRequest()
            {
                Holon = holon,
                SaveChildren = saveChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError
            });

            //GetAndActivateProvider(providerType, setGlobally);
            //return await SaveHolon(holon);
        }

        /// <summary>
        /// Save's a holon data object.
        /// Set the saveChildren flag to true to save all the holon's child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the Recursive flag to true to save all the child's holon's recursively, or false to only save the first level of child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to save, it defaults to 0, which means it will save to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue saving child holon's even if an error has occured, this defaults to true.
        /// Pass in the provider you wish to use.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// </summary>
        /// <param name="holon"></param>
        /// <param name="saveChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("save-holon/{saveChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{providerType}/{setGlobally}")]
        public async Task<OASISHttpResponseMessage<IHolon>> SaveHolon(Holon holon, bool saveChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, string providerType = "Default", bool setGlobally = false)
        {
            return await SaveHolon(new SaveHolonRequest() 
            { 
                Holon = holon,
                SaveChildren = saveChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                ProviderType = providerType,
                SetGlobally = setGlobally
            });

            //GetAndActivateProvider(providerType, setGlobally);
            //return await SaveHolon(holon);
        }

        /// <summary>
        /// Save's a holon data object.
        /// Set the saveChildren flag to true to save all the holon's child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the Recursive flag to true to save all the child's holon's recursively, or false to only save the first level of child holon's. This defaults to true.
        /// If saveChildren is set to true, you can set the maxChildDepth value to a custom int of how many levels down you wish to save, it defaults to 0, which means it will save to infinite depth.
        /// Set the continueOnError flag to true if you wish it to continue saving child holon's even if an error has occured, this defaults to true.
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <param name="holon"></param>
        /// <param name="saveChildren"></param>
        /// <param name="recursive"></param>
        /// <param name="maxChildDepth"></param>
        /// <param name="continueOnError"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <param name="autoFailOverMode"></param>
        /// <param name="autoReplicationMode"></param>
        /// <param name="autoLoadBalanceMode"></param>
        /// <param name="autoFailOverProviders"></param>
        /// <param name="autoReplicationProviders"></param>
        /// <param name="autoLoadBalanceProviders"></param>
        /// <param name="waitForAutoReplicationResult"></param>
        /// <param name="showDetailedSettings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("save-holon/{saveChildren}/{recursive}/{maxChildDepth}/{continueOnError}/{providerType}/{setGlobally}/{autoReplicationMode}/{autoFailOverMode}/{autoLoadBalanceMode}/{autoReplicationProviders}/{autoFailOverProviders}/{AutoLoadBalanceProviders}/{waitForAutoReplicationResult}/{showDetailedSettings}")]
        public async Task<OASISHttpResponseMessage<IHolon>> SaveHolon(Holon holon, bool saveChildren = true, bool recursive = true, int maxChildDepth = 0, bool continueOnError = true, string providerType = "Default", bool setGlobally = false, string autoReplicationMode = "DEFAULT", string autoFailOverMode = "DEFAULT", string autoLoadBalanceMode = "DEFAULT", string autoReplicationProviders = "DEFAULT", string autoFailOverProviders = "DEFAULT", string autoLoadBalanceProviders = "DEFAULT", bool waitForAutoReplicationResult = false, bool showDetailedSettings = false)
        {
            return await SaveHolon(new SaveHolonRequest()
            {
                Holon = holon,
                SaveChildren = saveChildren,
                Recursive = recursive,
                MaxChildDepth = maxChildDepth,
                ContinueOnError = continueOnError,
                ProviderType = providerType,
                SetGlobally = setGlobally,
                AutoReplicationMode = autoReplicationMode,
                AutoFailOverMode = autoFailOverMode,
                AutoLoadBalanceMode = autoLoadBalanceMode,
                AutoReplicationProviders = autoReplicationProviders,
                AutoFailOverProviders = autoFailOverProviders,
                AutoLoadBalanceProviders = autoLoadBalanceProviders,
                WaitForAutoReplicationResult = waitForAutoReplicationResult,
                ShowDetailedSettings = showDetailedSettings
            });
        }



        
        /// <summary>
        /// Save's a holon data object (meta data) to the given off-chain provider and then links its hash to the on-chain provider.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("save-holon-off-chain")]
        public async Task<OASISHttpResponseMessage<Holon>> SaveHolonOffChain(SaveHolonRequest request)
        {
            return HttpResponseHelper.FormatResponse(new OASISResult<Holon>
            {
                IsError = false,
                Message = "COMING SOON..."
            });
        }

        /// <summary>
        /// Delete a holon for the given id. Set SoftDelete to true if you wish this holon to be kept (can be un-deleted later) or to false to permanently delete (cannot be recovered).
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-holon")]
        public async Task<OASISHttpResponseMessage<bool>> DeleteHolon(DeleteHolonRequest request)
        {
            OASISConfigResult<bool> configResult = ConfigureOASISEngine<bool>(request);

            if (configResult.IsError && configResult.Response != null)
                return configResult.Response;

            OASISResult<bool> response = await HolonManager.DeleteHolonAsync(request.Id, request.SoftDelete);
            ResetOASISSettings(request, configResult);

            return HttpResponseHelper.FormatResponse(response, System.Net.HttpStatusCode.OK, request.ShowDetailedSettings);
        }

        /// <summary>
        /// Delete a holon for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-holon/{id}")]
        public async Task<OASISHttpResponseMessage<bool>> DeleteHolon(Guid id)
        {
            return await DeleteHolon(new DeleteHolonRequest() { Id = id });
        }

        /// <summary>
        /// Delete a holon for the given id. Set SoftDelete to true if you wish this holon to be kept (can be un-deleted later) or to false to permanently delete (cannot be recovered).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="softDelete"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-holon/{id}/{softDelete}")]
        public async Task<OASISHttpResponseMessage<bool>> DeleteHolon(Guid id, bool softDelete = true)
        {
            return await DeleteHolon(new DeleteHolonRequest() { Id = id, SoftDelete = softDelete });
        }

        /// <summary>
        /// Delete a holon for the given id. Set SoftDelete to true if you wish this holon to be kept (can be un-deleted later) or to false to permanently delete (cannot be recovered).
        /// Pass in the provider you wish to use.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="softDelete"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-holon/{id}/{softDelete}/{providerType}/{setGlobally}")]
        public async Task<OASISHttpResponseMessage<bool>> DeleteHolon(Guid id, bool softDelete = true, string providerType = "", bool setGlobally = false)
        {
            return await DeleteHolon(new DeleteHolonRequest()
            {
                Id = id,
                SoftDelete = softDelete,
                ProviderType = providerType,
                SetGlobally = setGlobally
            });
        }

        /// <summary>
        /// Delete a holon for the given id. Set SoftDelete to true if you wish this holon to be kept (can be un-deleted later) or to false to permanently delete (cannot be recovered).
        /// Pass in the provider you wish to use.
        /// Set the autoFailOverMode to 'ON' if you wish this call to work through the the providers in the auto-failover list until it succeeds. Set it to OFF if you do not or to 'DEFAULT' to default to the global OASISDNA setting.
        /// Set the autoReplicationMode to 'ON' if you wish this call to auto-replicate to the providers in the auto-replication list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the autoLoadBalanceMode to 'ON' if you wish this call to use the fastest provider in your area from the auto-loadbalance list. Set it to OFF if you do not or to UseGlobalDefaultInOASISDNA to 'DEFAULT' to the global OASISDNA setting.
        /// Set the waitForAutoReplicationResult flag to true if you wish for the API to wait for the auto-replication to complete before returning the results.
        /// Set the setglobally flag to false to use these settings only for this request or true for it to be used for all future requests.
        /// Set the showDetailedSettings flag to true to view detailed settings such as the list of providers in the auto-failover, auto-replication &amp; auto-load balance lists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="softDelete"></param>
        /// <param name="providerType">Pass in the provider you wish to use.</param>
        /// <param name="setGlobally"> Set this to false for this provider to be used only for this request or true for it to be used for all future requests too.</param>
        /// <param name="autoFailOverMode"></param>
        /// <param name="autoReplicationMode"></param>
        /// <param name="autoLoadBalanceMode"></param>
        /// <param name="autoFailOverProviders"></param>
        /// <param name="autoReplicationProviders"></param>
        /// <param name="autoLoadBalanceProviders"></param>
        /// <param name="waitForAutoReplicationResult"></param>
        /// <param name="showDetailedSettings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-holon/{id}/{softDelete}/{providerType}/{setGlobally}/{autoReplicationMode}/{autoFailOverMode}/{autoLoadBalanceMode}/{autoReplicationProviders}/{autoFailOverProviders}/{autoLoadBalanceProviders}/{waitForAutoReplicationResult}/{showDetailedSettings}")]
        public async Task<OASISHttpResponseMessage<bool>> DeleteHolon(Guid id, bool softDelete = true, string providerType = "", bool setGlobally = false, string autoReplicationMode = "DEFAULT", string autoFailOverMode = "DEFAULT", string autoLoadBalanceMode = "DEFAULT", string autoReplicationProviders = "DEFAULT", string autoFailOverProviders = "DEFAULT", string autoLoadBalanceProviders = "DEFAULT", bool waitForAutoReplicationResult = false, bool showDetailedSettings = false)
        {
            return await DeleteHolon(new DeleteHolonRequest()
            {
                Id = id,
                SoftDelete = softDelete,
                ProviderType = providerType,
                SetGlobally = setGlobally,
                AutoReplicationMode = autoReplicationMode,
                AutoFailOverMode = autoFailOverMode,
                AutoLoadBalanceMode = autoLoadBalanceMode,
                AutoReplicationProviders = autoReplicationProviders,
                AutoFailOverProviders = autoFailOverProviders,
                AutoLoadBalanceProviders = autoLoadBalanceProviders,
                WaitForAutoReplicationResult = waitForAutoReplicationResult,
                ShowDetailedSettings = showDetailedSettings
            });
        }

        private (OASISHttpResponseMessage<T>, HolonType) ValidateHolonType<T>(string holonType)
        {
            object holonTypeObject = null;

            if (!string.IsNullOrEmpty(holonType) && !Enum.TryParse(typeof(HolonType), holonType, out holonTypeObject))
                return (HttpResponseHelper.FormatResponse(new OASISResult<T>() { IsError = true, Message = $"The HolonType {holonType} is not valid. It must be one of the following values: {EnumHelper.GetEnumValues(typeof(HolonType), EnumHelperListType.ItemsSeperatedByComma)}" }), HolonType.All);
            else
                return (HttpResponseHelper.FormatResponse(new OASISResult<T>()), (HolonType)holonTypeObject);
        }
    }
}
