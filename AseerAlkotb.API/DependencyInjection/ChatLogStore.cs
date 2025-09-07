using System.Collections.Generic;
using System.Collections.Concurrent;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Chat.Requests;

namespace AseerAlkotb.API.DependencyInjection
{
    public class InMemoryChatLogStore : IChatLogStore
    {
        private readonly ConcurrentQueue<ChatLogRequest> logs = new ConcurrentQueue<ChatLogRequest>();

        public void Append(ChatLogRequest log)
        {
            logs.Enqueue(log);
            while (logs.Count > 1000 && logs.TryDequeue(out _)) { }
        }

        public IReadOnlyList<ChatLogRequest> GetAll()
        {
            return Array.AsReadOnly(logs.ToArray());
        }
    }
}


