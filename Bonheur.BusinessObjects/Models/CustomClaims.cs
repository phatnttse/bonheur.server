using Bonheur.BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Models
{
    public class CustomClaims
    {
        ///<summary>A claim that specifies the full name of an entity</summary>
        public const string FullName = "fullname";

        ///<summary>A claim that specifies the configuration/customizations of an entity</summary>
        public const string Configuration = "configuration";

        ///<summary>A claim that specifies the permission of an entity</summary>
        public const string Permission = "permission";

        ///<summary>A claim that specifies the gender of an entity</summary>
        public const string Gender = "gender";

        ///<summary>A claim that specifies the email confirm status of an entity</summary>
        public const string EmailConfirmed = "emailConfirmed";

        ///<summary>A claim that specifies the picture url of an entity</summary>
        public const string PictureUrl = "pictureUrl";
    }
}
