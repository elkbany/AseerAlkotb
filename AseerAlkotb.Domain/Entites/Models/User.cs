using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 


        #region Navigation Properties
        public virtual Cart Cart { get; set; }
        //public int CartId { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];

        public virtual Wishlist Wishlist { get; set; }
        public virtual ICollection<LikeDisLike> LikeDisLikes { get; set; } = [];
        public virtual ICollection<UserFollow> Following { get; set; } = [];
        #endregion

    }
}