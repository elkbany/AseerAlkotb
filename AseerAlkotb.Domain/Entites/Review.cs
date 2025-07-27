using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Review: Entity<int>
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // Assuming rating is an integer value

    }
    
    
}
