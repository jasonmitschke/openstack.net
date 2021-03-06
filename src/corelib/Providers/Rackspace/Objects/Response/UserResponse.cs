using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class UserResponse
    {
        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}