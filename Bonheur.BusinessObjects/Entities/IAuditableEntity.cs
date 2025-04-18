﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Entities
{
    public interface IAuditableEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
