using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.Utils
{
    public static class Constants
    {
        public static class Roles
        {
            public const string ADMIN = "ADMIN";
            public const string USER = "USER";
            public const string SUPPLIER = "SUPPLIER";
        }

        public static class AVATAR_DEFAULT
        {
            public const string MALE = "assets/images/profile/male.jpg";
            public const string FEMAILE = "assets/images/profile/female.jpg";
            public const string OTHER = "assets/images/profile/other.jpg";
        }

        public static class Providers
        {
            public const string GOOGLE = "google";
        }

        public static class GrantTypes
        {
            public const string ASSERTION = "assertion";

        }

        public static class Common
        {
            public const long MAX_PRICE = 10000000000;
        }

        public static class InvoiceInfo
        {
            public const string COMPANY_NAME = "Bonheur Pte Ltd";
            public const string COMPANY_ADDRESS = "32 Thuy Loi, Phuoc Long A, Quan 9, Thanh pho Ho Chi Minh";
            public const string PHONE_NUMBER = "0392341142";
            public const string EMAIL = "bonheur.wedding.1910@gmail.com";
            public const string WEBSITE = "https://bonheur.io.vn";
        } 
    }
}
