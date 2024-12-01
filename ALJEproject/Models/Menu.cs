using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class Menu
    {
        [Key]
        public int MenuID { get; set; }

        [Required]
        [DisplayName("Controller Name")]
        public string ControllerName { get; set; }

        [Required]
        [DisplayName("Menu Name")]
        public string MenuName { get; set; }

        [DisplayName("Menu Description")]
        public string MenuDesc { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        public bool Active { get; set; }
        public string MenuURL { get; set; } // New Field

        public int MenuOrder { get; set; }

        public int? ParentMenuID { get; set; }

        // Relasi Parent-Child
        public virtual Menu ParentMenu { get; set; }
        public virtual ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
    }
}
