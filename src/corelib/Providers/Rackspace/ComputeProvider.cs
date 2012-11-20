﻿using System;
using System.Collections.Generic;
using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Mapping;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Mapping;

namespace net.openstack.Providers.Rackspace
{
    public class ComputeProvider : ProviderBase, IComputeProvider
    {
        #region Private/Protected instance members

        private readonly IJsonObjectMapper<Metadata> _metadataMapper;
        private readonly IJsonObjectMapper<CreateServerRequest> _createServerRequestMapper;

        #endregion

        public ComputeProvider()
            : this(new IdentityProvider(), new JsonRestServices()) { }

        public ComputeProvider(IIdentityProvider identityProvider, IRestService restService) 
            : this(identityProvider, restService, new MetaDataJsonMapper(), new CreateServerRequestJsonMapper()) {}

        public ComputeProvider(IIdentityProvider identityProvider, IRestService restService, IJsonObjectMapper<Metadata> metadataMapper, IJsonObjectMapper<CreateServerRequest> createServerRequestMapper)
            : base(identityProvider, restService)
        {
            _metadataMapper = metadataMapper;
            _createServerRequestMapper = createServerRequestMapper;
        }

        #region Servers

        public IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListServersResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/detail", GetServiceEndpoint(identity, region)));
            
            var response = ExecuteRESTRequest<ListServersResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public NewServer CreateServer(CloudIdentity identity, string cloudServerName, string friendlyName, string imageName, string flavor, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity, region)));

            var requestJson = _createServerRequestMapper.ToJson(new CreateServerRequest(){DiskConfig = "AUTO", Flavor = flavor, ImageName = imageName, Name = cloudServerName, FriendlyName = friendlyName});
            var response = ExecuteRESTRequest<CreateServerResponse>(urlPath, HttpMethod.POST, requestJson, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return null; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);
            
            return response.Data.Server;
        }

        public ServerDetails GetDetails(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest<ServerDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            response.Data.Server.Metadata = _metadataMapper.FromJson(response.RawBody);
            return response.Data.Server;
        }

        public bool UpdateServer(CloudIdentity identity, string cloudServerId, string name, string ipV4Address, string ipV6Address, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var requestJson = new UpdateServerRequest(name, ipV4Address, ipV6Address);
            var response = ExecuteRESTRequest<ServerDetailsResponse>(urlPath, HttpMethod.PUT, requestJson, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return false;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        public bool DeleteServer(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var defaultSettings = BuildDefaultRequestSettings(new [] {404});
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity, requestSettings: defaultSettings);

            if (response.StatusCode != 200 && response.StatusCode != 204)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        #endregion

        #region Server Addresses

        public ServerAddresses ListAddresses(CloudIdentity identity, string serverId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/ips", GetServiceEndpoint(identity, region), serverId));

            var response = ExecuteRESTRequest<ListAddressesResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Addresses;
        }

        public Network ListAddressesByNetwork(CloudIdentity identity, string serverId, string network, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/ips/{2}", GetServiceEndpoint(identity, region), serverId, network));

            var response = ExecuteRESTRequest<ListAddressesByNetworkResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        #endregion

        #region Server Actions

        #endregion

        #region Volume Attachment Actions

        #endregion

        #region Flavors

        public IEnumerable<Flavor> ListFlavors(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerID = null, int limit = 0, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListFlavorsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavors;
        }

        public IEnumerable<FlavorDetails> ListFlavorsWithDetails(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerID = null, int limit = 0, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors/detail", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListFlavorDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavors;
        }

        public FlavorDetails GetFlavor(CloudIdentity identity, string id, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors/{1}", GetServiceEndpoint(identity, region), id));

            var response = ExecuteRESTRequest<FlavorDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavor;
        }

        #endregion

        #region Images

        public IEnumerable<ServerImage> ListImages(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = new DateTime(), string markerId = null, int limit = 0, ImageType imageType = ImageType.NONE, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListImagesResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Images;
        }

        public IEnumerable<ServerImageDetails> ListImagesWithDetails(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = new DateTime(), string markerId = null, int limit = 0, ImageType imageType = ImageType.NONE, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/detail", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListImagesDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Images;
        }

        public ServerImageDetails GetImage(CloudIdentity identity, string imageId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}", GetServiceEndpoint(identity, region), imageId));

            var response = ExecuteRESTRequest<GetImageDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Image;
        }

        public bool DeleteImage(CloudIdentity identity, string imageId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}", GetServiceEndpoint(identity, region), imageId));

            var defaultSettings = BuildDefaultRequestSettings(new[] { 404 });
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity, requestSettings: defaultSettings);

            if (response.StatusCode != 200 && response.StatusCode != 203)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        #endregion

        #region Metadata

        public Metadata ListMetadata(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest<MetaDataResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null)
                return null;

            return _metadataMapper.FromJson(response.RawBody);
        }

        #endregion

        #region Private methods
        
        protected string GetServiceEndpoint(CloudIdentity identity, string region = null)
        {
            return base.GetServiceEndpoint(identity, "cloudServersOpenStack", region);
        }

        #endregion
    }
}