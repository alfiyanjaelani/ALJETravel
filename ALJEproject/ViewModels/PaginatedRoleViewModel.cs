using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class PaginatedRoleViewModel
    {
        public IEnumerable<RoleView> Roles { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
