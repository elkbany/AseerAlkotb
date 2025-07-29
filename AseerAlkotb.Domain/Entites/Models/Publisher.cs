using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Publisher : Entity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string ContactEmail { get; set; }
        #region Navigation Properties
        public  virtual ICollection<Book> Books { get; set; } = [];
        #endregion
    }
}
