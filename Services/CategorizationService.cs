



namespace ExpanseCategorizationAPI.Services
{
    public class CategorizationService
    {
        public static readonly Dictionary<string, string> Rules = new()
        {
            { "train", "Transport" },
            { "groceries", "Food & Drinks" },
            { "netflix", "Entertainment" },
            { "steam", "Gaming" },
        };

        public string CategorizeTransaction(string description)
        {
            string key = description.ToLower();

            return Rules.FirstOrDefault(r => key.Contains(r.Key)).Value ?? "Other";
        }
    }
}