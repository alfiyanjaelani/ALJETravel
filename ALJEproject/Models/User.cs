using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ALJEproject.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public int RoleID { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [EmailAddress]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
    }
}
