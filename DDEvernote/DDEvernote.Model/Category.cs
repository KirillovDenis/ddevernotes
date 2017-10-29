using System;
using System.ComponentModel.DataAnnotations;

namespace DDEvernote.Model
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }
    }
}
