using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALJEproject.Models
{
    [Table("vw_UserRoles")]
    public class UserRoleView
    {
        [Key]
        public int UserId { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Email")]
        public string EmailAddress { get; set; }        
        public string Phone { get; set; }
        public int RoleID { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
