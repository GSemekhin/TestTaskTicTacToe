using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTaskTicTacToe.Data;
using TestTaskTicTacToe.Models;

namespace TestTaskTicTacToe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameContext _context;

        public GameController(GameContext context)
        {
            _context = context;
        }

        // POST: api/game/makemove
        [HttpPost("makemove")]
        public async Task<IActionResult> MakeMove([FromBody] Move move)
        {
            // Проверить, что запрашиваемая игра существует
            var game = await _context.Games.FindAsync(move.GameId);
            if (game == null)
            {
                return NotFound();
            }

            if (game.GameStatus != "InProgress")
            {
                return BadRequest("The game is not in progress.");
            }

            // Определенеи текущего символа и игрока
            char currentPlayerSymbol = TicTacToeUtils.GetCurrentPlayerSymbol(game.BoardState);
            Guid? expectedPlayerId = currentPlayerSymbol == 'X' ? game.Player1Id : game.Player2Id;
            if (move.PlayerId != expectedPlayerId)
            {
                return BadRequest("It's not your turn.");
            }

            if (!TicTacToeUtils.IsValidMove(move, game))
            {
                return BadRequest("Invalid move.");
            }

            // Сделать ход
            var boardArray = game.BoardState.ToCharArray();
            boardArray[move.Position] = currentPlayerSymbol;
            game.BoardState = new string(boardArray);

            if (TicTacToeUtils.IsWin(game.BoardState, currentPlayerSymbol))
            {
                game.GameStatus = "Ended";
                if (currentPlayerSymbol == 'X')
                {
                    game.Winner = game.Player1Nickname;
                }
                else
                {
                    game.Winner = game.Player2Nickname;
                }
            }
            else if (TicTacToeUtils.IsDraw(game.BoardState))
            {
                game.GameStatus = "Ended";
                game.Winner = "Draw";
            }

            // Сохранить в БД
            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{gameId}/state")]
        public async Task<ActionResult<GameStateResponse>> GetGameState(Guid gameId)
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null)
            {
                return NotFound("Game not found.");
            }

            // Создадим ответ, который не включает в себя Id игрока
            var response = new GameStateResponse
            {
                GameId = game.GameId,
                BoardState = game.BoardState,
                GameStatus = game.GameStatus,
                Player1Nickname = game.Player1Nickname,
                Player2Nickname = game.Player2Nickname,
                Winner = game.Winner
            };

            return Ok(response);
        }

        [HttpPost("joingame")]
        public async Task<ActionResult<GameStateResponse>> JoinGame([FromBody] Guid playerId)
        {
            var waitingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameStatus == "Waiting");

            // Проверить, что игрок зарегистрирован
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return NotFound("Player not found. Please Register first.");
            }

            // Проверить, что игрок уже не находится в очереди
            var existingWaitingGame = await _context.Games.FirstOrDefaultAsync(g => g.GameStatus == "Waiting" && g.Player1Id == playerId);
            if (existingWaitingGame != null)
            {
                return BadRequest("Player already has a waiting game. Please wait for second player.");
            }

            // Если игры нет, то создать игру. Если игра уже есть, то сменить добавить игрока и сменить статус игры
            if (waitingGame == null)
            {
                Game newGame = new Game
                {
                    GameId = Guid.NewGuid(),
                    Player1Id = playerId,
                    Player1Nickname = player.Nickname,
                    GameStatus = "Waiting",
                    BoardState = "---------"
                };

                _context.Games.Add(newGame);
                await _context.SaveChangesAsync();

                return Ok(TicTacToeUtils.GameToGameStateResponse(newGame));
            }
            else
            {
                waitingGame.Player2Id = playerId;
                waitingGame.Player2Nickname = player.Nickname;
                waitingGame.GameStatus = "InProgress";

                _context.Entry(waitingGame).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(TicTacToeUtils.GameToGameStateResponse(waitingGame));
            }
        }

        // POST: api/game/registerplayer
        [HttpPost("registerplayer")]
        public async Task<ActionResult<Player>> RegisterPlayer([FromBody] string nickname)
        {
            Player newPlayer = new Player
            {
                PlayerId = Guid.NewGuid(),
                Nickname = nickname
            };

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            // Отправим ответ, что игрок успешно зарегистрирован и вернем его id
            return CreatedAtAction("RegisterPlayer", new { id = newPlayer.PlayerId }, newPlayer);
        }
    }
}
