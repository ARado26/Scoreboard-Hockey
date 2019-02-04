using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    public class GameInfo
    {
		public TimeSpan gameTime { get; set; }
		public string period { get; set; }
		

		public GameInfo() {
			gameTime = new TimeSpan();
			period = "PG";
		}

		public void setGameTime(TimeSpan time) {
			gameTime = time;
		}

		public void setPeriod(string prd) {
			period = prd;
		}

		public void printInfo() {
			Console.WriteLine("GameTime: " + gameTime.ToString());
			Console.WriteLine("Period: " + period);
		}
		
    }
}
