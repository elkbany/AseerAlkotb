using System.Threading;

namespace AseerAlkotb.Infrastructure.AI
{
    public static class GeminiConcurrencyGate
    {
        public static readonly SemaphoreSlim Gate = new(initialCount: 2, maxCount: 2);
    }
}
