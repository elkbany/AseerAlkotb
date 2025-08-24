using AseerAlkotb.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Base
{
    public interface IUnitOfWork
    {
        //public IEntityRepository EntityRepository {get;}
        public IAuthorRepository Authors { get; }
        public ICategoryRepository Categories { get; }

        public IBookRepository Books { get; }


        public ICartRepository Carts { get; }


        public IPublisherRepository Publishers { get;}


        public IReviewRepository Reviews { get; }
        
        IWishlistRepository Wishlists { get; }

<<<<<<< HEAD
        public IQuoteRepository Quotes { get; }


=======
        public IOrderRepository Orders { get; }
        public IPaymentRepository Payments { get; }
        public INotificationRepository Notifications { get; }
>>>>>>> 44eb7d1b58575d970a9903428ade810eb1c279d2
        public Task<int> CommitAsync();
    }
}
