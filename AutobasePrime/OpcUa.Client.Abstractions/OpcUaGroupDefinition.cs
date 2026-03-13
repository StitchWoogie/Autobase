using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Abstractions
{
    public sealed class OpcUaGroupDefinition
    {
        public string GroupName;
        public int PublishingInterval;
        public List<OpcUaItemDefinition> Items = new List<OpcUaItemDefinition>();
    }

}
