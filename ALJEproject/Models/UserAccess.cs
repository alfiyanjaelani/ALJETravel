using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class UserAccess
    {
        [Key]
        public int UserAccessID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        public int MenuID { get; set; }

        [DisplayName("Views")]
        public bool Views { get; set; }

        [DisplayName("Inserts")]
        public bool Inserts { get; set; }

        [DisplayName("Edits")]
        public bool Edits { get; set; }

        [DisplayName("Deletes")]
        public bool Deletes { get; set; }

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
