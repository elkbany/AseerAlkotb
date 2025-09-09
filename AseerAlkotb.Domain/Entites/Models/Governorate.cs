using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Governorate : Entity<int>
    {
        public string Name { get; set; } = string.Empty;

        #region Navigation Properties
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        #endregion

        public Governorate()
        {
            
        }
    }
}