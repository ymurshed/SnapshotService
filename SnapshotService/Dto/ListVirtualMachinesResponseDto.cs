using System;
using System.Collections.Generic;

namespace SnapshotService.Dto
{
    public class ListVirtualMachinesResponseDto
    {
        public ListVirtualMachinesResponse ListVirtualMachinesResponse { get; set; }
    }

    public class ListVirtualMachinesResponse 
    {
        public int Count { get; set; }
        public List<VirtualMachine> VirtualMachine { get; set; }
    }

    public class VirtualMachine
    {
        public int CpuNumber { get; set; }
        public string RootDeviceType { get; set; }
        public List<Tags> Tags { get; set; }
        public string Hypervisor { get; set; }
        public string ServiceOfferingName { get; set; }
        public int CpuSpeed { get; set; }
        public string Name { get; set; }
        public int Memory { get; set; }
        public DateTime Created { get; set; }
        public string TemplateName { get; set; }
        public string CpuUsed { get; set; }
        public string DisplayName { get; set; } // Instance Name
        public string Id { get; set; } // Instance Id/Virtual Machine Id
    }
}
