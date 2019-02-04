using System;
using Newtonsoft.Json;

namespace Scoreboard {
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

		public string logInfo() {
			string info = "";
			info += JsonConvert.SerializeObject(this);
			return info;
		}
		
    }
}
