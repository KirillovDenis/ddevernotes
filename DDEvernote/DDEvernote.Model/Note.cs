using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DDEvernote.Model
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public User Owner { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }

        public DateTime Changed { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<User> Shared { get; set; }
    }
}
