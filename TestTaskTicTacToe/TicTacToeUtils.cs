using TestTaskTicTacToe.Models;

namespace TestTaskTicTacToe
{
    public static class TicTacToeUtils
    {
        public static bool IsValidMove(Move move, Game game)
        {
            if (move.Position < 0 || move.Position >= game.BoardState.Length)
            {
                return false;
            }

            if (game.BoardState[move.Position] != '-')
            {
                return false;
            }

            return true;
        }

        public static bool IsWin(string boardState, char player)
        {
            // Все возможные выигрышные комбинации
            int[][] lines = new int[][]
                            {
                            new int[] { 0, 1, 2 },
                            new int[] { 3, 4, 5 },
                            new int[] { 6, 7, 8 },
                            new int[] { 0, 3, 6 },
                            new int[] { 1, 4, 7 },
                            new int[] { 2, 5, 8 },
                            new int[] { 0, 4, 8 },
                            new int[] { 2, 4, 6 },
                            };

            return lines.Any(line => line.All(i => boardState[i] == player));
        }

        public static bool IsDraw(string boardState)
        {
            return !boardState.Contains('-');
        }
        public static char GetCurrentPlayerSymbol(string boardState)
        {
            int emptyCells = boardState.Count(c => c == '-');
            return emptyCells % 2 == 0 ? 'O' : 'X';
        }
        public static GameStateResponse GameToGameStateResponse(Game game)
        {
            string winner = null;
            if (game.GameStatus == "Player1Wins")
            {
                winner = game.Player1Nickname;
            }
            else if (game.GameStatus == "Player2Wins")
            {
                winner = game.Player2Nickname;
            }

            return new GameStateResponse
            {
                GameId = game.GameId,
                BoardState = game.BoardState,
                GameStatus = game.GameStatus,
                Player1Nickname = game.Player1Nickname,
                Player2Nickname = game.Player2Nickname,
                Winner = winner
            };
        }

    }

}
