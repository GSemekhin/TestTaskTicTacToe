using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TestTaskTicTacToe.Models;

namespace TestTaskTicTacToe.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        {
            // Позволяет проверить, что БД существует и создать при первом запросе
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
