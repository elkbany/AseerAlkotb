using System.Collections.Concurrent;
using System.Threading.Channels;
using AseerAlkotb.Application.BackgroundJobs;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AseerAlkotb.Infrastructure.Background
{
    public class EmbeddingRefreshBackgroundService : BackgroundService, IEmbeddingRefreshJob
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<EmbeddingRefreshBackgroundService> _log;

        private readonly Channel<WorkItem> _queue = Channel.CreateUnbounded<WorkItem>();

        // volatile OK on primitive/reference types
        private volatile bool _isRunning;
        private volatile int _processed;
        private volatile int _total;
        private volatile string? _phase;
        private volatile string? _lastError;

        // NOT volatile: guard with a lock
        private readonly object _statusLock = new();
        private DateTimeOffset? _lastStartUtc;
        private DateTimeOffset? _lastFinishUtc;

        private readonly ConcurrentDictionary<int, byte> _dedupeBookIds = new();

        public EmbeddingRefreshBackgroundService(IServiceProvider sp, ILogger<EmbeddingRefreshBackgroundService> log)
        {
            _sp = sp;
            _log = log;
        }

        // ===== Public API =====
        public bool TriggerFullRebuild()
        {
            if (_isRunning) return false;
            return _queue.Writer.TryWrite(WorkItem.FullRebuild());
        }

        public bool TriggerBookUpdate(int bookId)
        {
            if (!_dedupeBookIds.TryAdd(bookId, 0)) return false;
            var ok = _queue.Writer.TryWrite(WorkItem.UpdateBook(bookId));
            if (!ok) _dedupeBookIds.TryRemove(bookId, out _);
            return ok;
        }

        public EmbeddingRefreshStatus GetStatus()
        {
            // Take a consistent snapshot
            lock (_statusLock)
            {
                return new EmbeddingRefreshStatus(
                    IsRunning: _isRunning,
                    CurrentPhase: _phase ?? "idle",
                    Processed: _processed,
                    Total: _total,
                    LastRunStartedUtc: _lastStartUtc,
                    LastRunFinishedUtc: _lastFinishUtc,
                    LastError: _lastError
                );
            }
        }

        // ===== Worker Loop =====
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () =>
            {
                // كل 24 ساعة شغّل FullRebuild (عدّلها حسب احتياجك)
                var delay = TimeSpan.FromHours(24);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!_isRunning)
                            TriggerFullRebuild();
                    }
                    catch { /* ignore */ }
                    await Task.Delay(delay, stoppingToken);
                }
            }, stoppingToken);

            await foreach (var item in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _isRunning = true;
                    _lastError = null;
                    lock (_statusLock)
                    {
                        _lastStartUtc = DateTimeOffset.UtcNow;
                    }

                    if (item.IsFullRebuild)
                        await HandleFullRebuildAsync(stoppingToken);
                    else
                        await HandleSingleAsync(item.BookId!.Value, stoppingToken);

                    lock (_statusLock)
                    {
                        _lastFinishUtc = DateTimeOffset.UtcNow;
                    }
                }
                catch (OperationCanceledException) { /* graceful */ }
                catch (Exception ex)
                {
                    _log.LogError(ex, "EmbeddingRefresh failed");
                    _lastError = ex.Message;
                    lock (_statusLock)
                    {
                        _lastFinishUtc = DateTimeOffset.UtcNow;
                    }
                }
                finally
                {
                    _phase = "idle";
                    _isRunning = false;
                }
            }
        }

        private async Task HandleFullRebuildAsync(CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emb = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

            _phase = "enumerating-books";
            var ids = await db.Books
                .AsNoTracking()
                .Where(b => b.IsActive)
                .Select(b => b.Id)
                .ToListAsync(ct);

            _total = ids.Count;
            _processed = 0;
            _phase = "refreshing-embeddings";

            foreach (var id in ids)
            {
                ct.ThrowIfCancellationRequested();
                await emb.UpdateBookEmbeddingsAsync(id);
                _processed++;
            }
            _phase = "done";
        }

        private async Task HandleSingleAsync(int bookId, CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var emb = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

            _phase = $"updating-book:{bookId}";
            _total = 1;
            _processed = 0;

            await emb.UpdateBookEmbeddingsAsync(bookId);
            _processed = 1;

            _dedupeBookIds.TryRemove(bookId, out _);
            _phase = "done";
        }

        private record WorkItem(bool IsFullRebuild, int? BookId)
        {
            public static WorkItem FullRebuild() => new(true, null);
            public static WorkItem UpdateBook(int bookId) => new(false, bookId);
        }
    }
}
