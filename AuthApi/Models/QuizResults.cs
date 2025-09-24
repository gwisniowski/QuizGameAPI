namespace AuthApi.Models
{
    public class QuizResults
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }

        public DateTime ResultDate { get; set; }
    }
}
