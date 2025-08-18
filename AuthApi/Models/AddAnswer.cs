namespace AuthApi.Models
{
    public class AddAnswer
    {
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
