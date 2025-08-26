using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Quote : Entity<int>
    {
        public string Comment { get; set; }


        #region Navigation Properties
        public QuoteFor QuoteFor { get; set; }
        public int? BookId { get; set; }
        public virtual Book Book { get; set; }
        public int? AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        #endregion


    }
}