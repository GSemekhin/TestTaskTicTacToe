namespace TestTaskTicTacToe.Models
{
    public class Move
    {
        public Guid GameId { get; set; }
        public int Position { get; set; } // Position on the board (0-8)
        public Guid PlayerId { get; set; }
    }
}
