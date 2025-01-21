﻿using System;
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


    }
}
