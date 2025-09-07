using System.Collections.Generic;
using AseerAlkotb.Application.Features.Chat.Requests;

namespace AseerAlkotb.Application.Contracts
{
    public interface IChatLogStore
    {
        void Append(ChatLogRequest log);
        IReadOnlyList<ChatLogRequest> GetAll();
    }
}



