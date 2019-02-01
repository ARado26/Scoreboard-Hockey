﻿using System;
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

		public void setGameTime(String minutesString, String secondsString) {
			int totalSeconds = 0;
			if (int.TryParse(minutesString, out int mins)) {
				if (int.TryParse(secondsString, out int sec)) {
					totalSeconds += sec;
					totalSeconds += mins * 60;
				}
			}
			int minutes = totalSeconds / 60;
			int seconds = totalSeconds % 60;
			gameTime = new TimeSpan(0, minutes, seconds);
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
