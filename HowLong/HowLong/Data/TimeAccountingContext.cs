using HowLong.DependencyServices;
using HowLong.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace HowLong.Data
{
	public sealed class TimeAccountingContext : DbContext
	{
		private readonly string _databasePath;
		public DbSet<TimeAccount> TimeAccounts { get; set; }
        public DbSet<Break> Breaks { get; set; }
        
        public TimeAccountingContext()
		{
			_databasePath = DependencyService.Get<IGetSqLitePath>().GetDatabasePath("howLong.db");
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Filename={_databasePath}");

	}
}
