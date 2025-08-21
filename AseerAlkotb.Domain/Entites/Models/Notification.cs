using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Notification : Entity<int>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
