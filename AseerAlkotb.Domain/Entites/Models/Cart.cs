using AseerAlkotb.Domain.Entites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Cart : Entity<int>
    {
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
