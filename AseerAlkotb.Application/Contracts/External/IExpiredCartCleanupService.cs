using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts.External
{
    public interface IExpiredCartCleanupService
    {
        Task CleanExpiredCartsAsync();
    }
}
