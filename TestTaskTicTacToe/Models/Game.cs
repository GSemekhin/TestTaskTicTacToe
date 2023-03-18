namespace TestTaskTicTacToe.Models
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Player1Nickname { get; set; }
        public string? Player2Nickname { get; set; }
        public Guid Player1Id { get; set; }
        public Guid? Player2Id { get; set; }
        public string BoardState { get; set; }
        public string? Winner { get; set; }
        public string GameStatus { get; set; }

    }
}
