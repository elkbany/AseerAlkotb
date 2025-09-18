using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface ITranslationService
    {
        Task<string> TranslateToArabicAsync(string englishText);
        Task<string> TranslateToEnglishAsync(string arabicText);
        Task<bool> IsEnglishTextAsync(string text);
    }
}