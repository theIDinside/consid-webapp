using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.mvc.Models {
    public class Category {
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }

        [Column("CategoryName")]
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }
}