using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Category: Entity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<Category>? SubCategory { get; set; } = new List<Category>();
        public bool IsActive { get; set; }


        // Navigation         

    }
        


}
