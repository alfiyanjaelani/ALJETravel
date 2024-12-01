using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ALJEproject.Models
{
    public class Option
    {
        [Key]
        public int OptionsID { get; set; }  // Unique identifier

        [Required]
        [MaxLength(100)]
        [DisplayName("Field Name")]
        public string FieldName { get; set; }

        [MaxLength(500)]
        [DisplayName("Field Value")]
        public string FieldValue { get; set; }

        [MaxLength(200)]
        [DisplayName("Long Name")]
        public string LongName { get; set; }

        [MaxLength(50)]
        [DisplayName("Short Name")]
        public string ShortName { get; set; }
         
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [MaxLength(100)]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(100)]
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }

        public bool Active { get; set; }
    }
}
