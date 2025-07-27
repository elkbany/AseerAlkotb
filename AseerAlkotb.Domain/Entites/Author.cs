using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Author : Entity<int>
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
