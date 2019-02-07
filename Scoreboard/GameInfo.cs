using System;
using Newtonsoft.Json;

namespace Scoreboard {
	public class GameInfo
    {
		public TimeSpan gameTime { get; set; }
		public string period { get; set; }
		public bool reversedBanner { get; set; }
		

		public GameInfo() {
			gameTime = new TimeSpan();
			period = "PG";
			reversedBanner = false;
		}

		public void fromJson(string json) {
			GameInfo saved = JsonConvert.DeserializeObject<GameInfo>(json);
			gameTime = saved.gameTime;
			period = saved.period;
			reversedBanner = saved.reversedBanner;
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

		public string getFormattedGameTime() {
			string formattedTime;
			if (gameTime.Minutes > 0) {
				formattedTime = PenaltyAndTimeCalculator.timeSpanToTimeString(gameTime);
			} else {
				double seconds = gameTime.TotalSeconds;
				formattedTime = string.Format("{0:N2}", seconds);
			}
			return formattedTime;
		}
		
    }
}
