using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class UserAccessView
    {
        public int UserAccessID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public bool Views { get; set; }
        public bool Inserts { get; set; }
        public bool Edits { get; set; }
        public bool Deletes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
