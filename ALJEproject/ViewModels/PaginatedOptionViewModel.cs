using System;
using System.Collections.Generic;

namespace ALJEproject.Models
{
    public class PaginatedOptionViewModel
    {
        public IEnumerable<Option> Options { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
