using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Common
{
    public static class StringNormalizer
    {
        private static readonly Regex MultiSpace = new("\\s+", RegexOptions.Compiled);

        public static string? Normalize(string? input)
        {
            if (input is null) return null;
            var trimmed = input.Trim();
            if (trimmed.Length == 0) return string.Empty;

            var collapsed = MultiSpace.Replace(trimmed, " ");
            return collapsed.Normalize(NormalizationForm.FormC);
        }
    }
}
