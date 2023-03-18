namespace TestTaskTicTacToe.Models
{
    public class GameStateResponse
    {
        public Guid GameId { get; set; }
        public string BoardState { get; set; }
        public string GameStatus { get; set; }
        public string Player1Nickname { get; set; }
        public string? Player2Nickname { get; set; }
        public string? Winner { get; set; }
    }
}
