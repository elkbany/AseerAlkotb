using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.BackgroundJobs
{
    public interface IEmbeddingRefreshJob
    {
        /// <summary>شغّل تحديث لكل الكتب في الخلفية (لو مش شغال بالفعل).</summary>
        bool TriggerFullRebuild();

        /// <summary>شغّل تحديث لكتاب واحد في الخلفية.</summary>
        bool TriggerBookUpdate(int bookId);

        /// <summary>حالة الشغل الحالي/الأخير.</summary>
        EmbeddingRefreshStatus GetStatus();
    }

    public record EmbeddingRefreshStatus(
        bool IsRunning,
        string? CurrentPhase,
        int Processed,
        int Total,
        DateTimeOffset? LastRunStartedUtc,
        DateTimeOffset? LastRunFinishedUtc,
        string? LastError
    );
}
