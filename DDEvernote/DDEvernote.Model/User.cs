using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DDEvernote.Model
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
        
        public IEnumerable<Category> Categories { get; set; }
    }
}