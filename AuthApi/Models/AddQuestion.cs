namespace AuthApi.Models
{
    public class AddQuestion
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<AddAnswer> Answers { get; set; } = new();


    }
}
