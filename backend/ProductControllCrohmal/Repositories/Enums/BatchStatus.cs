namespace Repositories.Enums
{
    public enum BatchStatus
    {
        Registered = 1,      // Зареєстрована
        InAnalysis = 2,      // На аналізі
        UnderReview = 3,     // На оцінці якості
        Approved = 4,        // Придатна
        Rejected = 5         // Брак
    }
}
