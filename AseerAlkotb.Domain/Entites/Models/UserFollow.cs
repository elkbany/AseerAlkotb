using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Domain.Entites.Models
{
   public class UserFollow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
        public FollowType FollowType { get; set; }

        #region Navigation
        public virtual User User { get; set; }
        public virtual Author? Author { get; set; }
        public virtual Publisher? Publisher { get; set; }
        #endregion
    }
}
