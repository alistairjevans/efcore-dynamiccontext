﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreDynamicContext.DataModels
{
    [Table("USERROLES", Schema = "maestro")]
    public class UserRoleModel
    {
        [Key]
        public string Link { get; set; }
    }
}
