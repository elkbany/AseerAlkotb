using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class City : Entity<int>
    {
        public string Name { get; set; } = string.Empty;
        public int GovernorateId { get; set; }

        #region Navigation Properties
        public virtual Governorate Governorate { get; set; }
        #endregion

        public City()
        {
            
        }
    }
}