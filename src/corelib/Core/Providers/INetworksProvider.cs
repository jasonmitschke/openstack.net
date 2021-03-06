﻿using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Providers
{
    /// <summary>
    /// Provides simple access to the Rackspace Cloud Network Services.
    /// </summary>
    public interface INetworksProvider
    {
        /// <summary>
        /// List the networks configured for the account.
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>A list of networks <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        IEnumerable<CloudNetwork> ListNetworks(string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Create a network with the given IP block.
        /// </summary>
        /// <param name="cidr">The IP block from which to allocate the network. For example, 172.16.0.0/24 or 2001:DB8::/64</param>
        /// <param name="label">The name of the new network. For example, my_new_network.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Details for the newly created network <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        CloudNetwork CreateNetwork(string cidr, string label, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Retrieve details for the specified network
        /// </summary>
        /// <param name="networkId">ID (uuid) of the network to retrieve</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Details for the specified network <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        CloudNetwork ShowNetwork(string networkId, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Deletes the specified network. <remarks>You cannot delete an isolated network unless the network is not attached to any server.</remarks>
        /// </summary>
        /// <param name="networkId">ID (uuid) of the network to delete</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the delete was successful</returns>
        bool DeleteNetwork(string networkId, string region = null, CloudIdentity identity = null);
    }
}
