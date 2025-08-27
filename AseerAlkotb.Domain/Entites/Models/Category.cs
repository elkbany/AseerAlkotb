using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Category: Entity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } // nullable when it's subctegory
        public bool IsActive { get; set; } = true;

        #region Navigation Properties
        public int? ParentCategoryId { get; set; } // used when it's subcategory
        public virtual Category? ParentCategory { get; set; } // parent category if existed
        public virtual ICollection<Category>? SubCategory { get; set; } = new List<Category>(); // subcategories of this category if existed
        #endregion

        public Category()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
