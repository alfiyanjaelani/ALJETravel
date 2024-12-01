using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class RoleView
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
