// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Management.Automation;
using AutoMapper;
using Microsoft.Azure.Commands.Tags.Model;
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Commands.Network.Models;

using MNM = Microsoft.Azure.Management.Network.Models;

namespace Microsoft.Azure.Commands.Network
{
    using System.Collections.Generic;

    [Cmdlet(VerbsCommon.Set, "AzureRmVirtualNetwork1")]
    [OutputType(typeof(PSVirtualNetwork))]
    [CliCommandAlias("vnet1 set")]
    public class SetAzureVirtualNetwork1Command : VirtualNetworkBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The virtualNetwork",
            ParameterSetName = "object")]
        public PSVirtualNetwork VirtualNetwork { get; set; }

        [Alias("ResourceName")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource name.",
            ParameterSetName = "clu")]
        [ValidateNotNullOrEmpty]
        public virtual string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group name.",
            ParameterSetName = "clu")]
        [ValidateNotNullOrEmpty]
        public virtual string ResourceGroupName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "The address prefixes of the virtual network", ParameterSetName = "clu")]
        [ValidateNotNullOrEmpty]
        public List<string> AddressPrefix { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The list of Dns Servers",
            ParameterSetName = "clu")]
        public List<string> DnsServer { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (this.ParameterSetName.Equals("object"))
            {
                if (!this.IsVirtualNetworkPresent(this.VirtualNetwork.ResourceGroupName, this.VirtualNetwork.Name))
                {
                    throw new ArgumentException(Microsoft.Azure.Commands.Network.Properties.Resources.ResourceNotFound);
                }

                // Map to the sdk object
                var vnetModel = Mapper.Map<MNM.VirtualNetwork>(this.VirtualNetwork);
                vnetModel.Tags = TagsConversionHelper.CreateTagDictionary(this.VirtualNetwork.Tag, validate: true);

                // Execute the Create VirtualNetwork call
                this.VirtualNetworkClient.CreateOrUpdate(
                    this.VirtualNetwork.ResourceGroupName,
                    this.VirtualNetwork.Name,
                    vnetModel);

                var getVirtualNetwork = this.GetVirtualNetwork(
                    this.VirtualNetwork.ResourceGroupName,
                    this.VirtualNetwork.Name);
                WriteObject(getVirtualNetwork);
            }
            else
            {
                WriteObject(this.CreateVirtualNetwork());
            }
        }

        private PSVirtualNetwork CreateVirtualNetwork()
        {
            var vnet = this.GetVirtualNetwork(this.ResourceGroupName, this.Name);

            vnet.Name = this.Name;
            vnet.ResourceGroupName = this.ResourceGroupName;
            vnet.AddressSpace = new PSAddressSpace();
            vnet.AddressSpace.AddressPrefixes = this.AddressPrefix;

            if (this.DnsServer != null)
            {
                vnet.DhcpOptions = new PSDhcpOptions();
                vnet.DhcpOptions.DnsServers = this.DnsServer;
            }

            // Map to the sdk object
            var vnetModel = Mapper.Map<MNM.VirtualNetwork>(vnet);
            
            // Execute the Create VirtualNetwork call
            this.VirtualNetworkClient.CreateOrUpdate(this.ResourceGroupName, this.Name, vnetModel);

            var getVirtualNetwork = this.GetVirtualNetwork(this.ResourceGroupName, this.Name);

            return getVirtualNetwork;
        }
    }
}
