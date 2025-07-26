using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public abstract class Entity<Key> where Key : struct
    {
        public Key Id { get; set; }
        public Entity()
        {
            Id = default;
        }
    }
}
