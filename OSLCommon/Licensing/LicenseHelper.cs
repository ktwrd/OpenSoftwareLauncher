using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OSLCommon.Licensing
{
    static class LicenseHelper
    {
        public const int LicenseKeyItemLength = 5;
        public const int LicenseKeyItemCount = 5;
        public const int LicenseKeyLength = (LicenseKeyItemLength * LicenseKeyItemCount) + (LicenseKeyItemCount - 1);
        public static Regex LicenseKeyRegex
        {
            get
            {
                return new Regex(@"(([A-Z0-9]{5}\-){4}[A-Z0-9]{5})");
            }
        }
        public static Regex LicenseIdRegex => new Regex(@"[A-Z0-9]{16}");
        public const int GroupIdLength = 18;
        public const int LicenseIdLength = 16;
        public static string GenerateLicenseKeyString()
        {
            string[] arr = new string[LicenseKeyItemCount];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = GeneralHelper.GenerateToken(LicenseKeyItemLength);
            return string.Join("-", arr);
        }
    }
}
