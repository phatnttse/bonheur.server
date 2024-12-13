using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Enums
{
    public enum RequestPricingStatus
    {
        [EnumMember(Value = "PENDING")]
        PENDING,

        [EnumMember(Value = "RESPONDED")]
        RESPONDED,

        [EnumMember(Value = "REJECTED")]
        REJECTED
    }
}
