﻿// ----------------------------------------------------------------------------------
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

using System.Management.Automation;
using Microsoft.Azure.Commands.Network.Models;

namespace Microsoft.Azure.Commands.Network
{
    [Cmdlet(VerbsCommon.New, "AzureRmVirtualNetwork1Subnet1", DefaultParameterSetName = "SetByResource"), OutputType(typeof(PSSubnet))]
    [CliCommandAlias("vnet1 subnet1 create")]
    public class NewAzureVirtualNetwork1Subnet1ConfigCommand : AzureVirtualNetworkSubnetConfigBase
    {
        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (string.Equals(ParameterSetName, Microsoft.Azure.Commands.Network.Properties.Resources.SetByResource))
            {
                if (this.NetworkSecurityGroup != null)
                {
                    this.NetworkSecurityGroupId = this.NetworkSecurityGroup.Id;
                }

                if (this.RouteTable != null)
                {
                    this.RouteTableId = this.RouteTable.Id;
                }
            }

            var subnet = new PSSubnet();
            subnet.Name = this.Name;
            subnet.AddressPrefix = this.AddressPrefix;

            if (!string.IsNullOrEmpty(this.NetworkSecurityGroupId))
            {
                subnet.NetworkSecurityGroup = new PSNetworkSecurityGroup();
                subnet.NetworkSecurityGroup.Id = this.NetworkSecurityGroupId;
            }

            if (!string.IsNullOrEmpty(this.RouteTableId))
            {
                subnet.RouteTable = new PSRouteTable();
                subnet.RouteTable.Id = this.RouteTableId;
            }

            WriteObject(subnet);
        }
    }
}
