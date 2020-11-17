﻿using System.Collections;
using System.Collections.Generic;

namespace Web.DTO.Admin
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public IEnumerable<string> RoleNames { get; set; }
    }
}