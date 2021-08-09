using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PalletReceive
{
    public static class Options
    {
        public static bool IsLocationRequired { get; set; }
        public static bool IsSlApprovalReq { get; set; }
        public static short NumberOfRowsUpload { get; set; }
        public static bool HasManualEntryAllowed { get; set; }
    }
}
