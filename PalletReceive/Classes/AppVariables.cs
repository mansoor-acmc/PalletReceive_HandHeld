using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using PalletReceive.DMServices;

namespace PalletReceive
{
    public enum RoleType
    {
        SortingLine,
        FinishedGoods,
        Admin
    }

    public static class AppVariables
    {
        public static string DeviceName { get; set; }
        public static string UpdatedBy { get; set; }
        public static RoleType RoleName { get; set; }
        public static bool WithoutInternet { get; set; }        
        public static string DeviceIP { get; set; }
        //public static string DefaultLocation { get; set; }
        public static List<WmsLocationContract> WarehouseLocations { get; set; }
        

        public static readonly string VersionNumber = "Version 2.5.1.0";
        public static readonly string ProjectName = "PalletReceived";
        public static readonly string NetworkDown = "Network/WiFi is down. Please contact Network administrator.";        
    }
}
