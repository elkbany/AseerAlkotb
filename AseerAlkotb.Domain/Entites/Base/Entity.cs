using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Base
{
    public abstract class Entity<Key> where Key : struct
    {
        public Key Id { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 


        public Entity()
        {
            Id = default;
        }
    }
}
