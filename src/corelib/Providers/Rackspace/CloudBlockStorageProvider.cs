﻿using System;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Validators;

namespace net.openstack.Providers.Rackspace
{
    public class CloudBlockStorageProvider : ProviderBase, ICloudBlockStorageProvider
    {

        private readonly int[] _validResponseCode = new[] { 200 };
        private readonly ICloudBlockStorageValidator _cloudBlockStorageValidator;

        public CloudBlockStorageProvider()
            : this(null) { }

        public CloudBlockStorageProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, new IdentityProvider(), new JsonRestServices(), new CloudBlockStorageValidator()) { }

        internal CloudBlockStorageProvider(IIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : this(null, identityProvider, restService, cloudBlockStorageValidator) { }

        internal CloudBlockStorageProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : base(defaultIdentity, identityProvider, restService)
        {
            _cloudBlockStorageValidator = cloudBlockStorageValidator;
        }


        public bool CreateVolume(int size, string display_description = null, string display_name = null, string snapshot_id = null, string volume_type = null, string region = null, CloudIdentity identity = null)
        {
            _cloudBlockStorageValidator.ValidateVolumeSize(size);

            var urlPath = new Uri(string.Format("{0}/volumes", GetServiceEndpoint(identity, region)));
            var requestBody = new CreateCloudBlockStorageVolumeRequest { CreateCloudBlockStorageVolumeDetails = new CreateCloudBlockStorageVolumeDetails { Size = size, DisplayDescription = display_description, DisplayName = display_name, SnapshotId = snapshot_id, VolumeType = volume_type } };
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, requestBody);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudBlockStorage", region);
        }

        #endregion
    }
}