using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard {
	public static class PenaltyAndTimeCalculator {

		public static string calculateTeamWithAdvantage(HockeyTeam home, HockeyTeam away) {
			string teamWithAdvantage;
			if (home.activeSkaters > away.activeSkaters) {
				teamWithAdvantage = "HOME";
			}
			else if (home.activeSkaters < away.activeSkaters) {
				teamWithAdvantage = "AWAY";
			}
			else {
				teamWithAdvantage = "NONE";
			}
			return teamWithAdvantage;
		}

		public static string calculatePlayerAdvantage(HockeyTeam team1, HockeyTeam team2) {
			string advantage = "";
			if (team1.activeSkaters == 5 && team2.activeSkaters == 4) {
				advantage = "PP";
			}
			else if(team1.activeSkaters == 5 && team2.activeSkaters == 3) {
				advantage = "2PP";
			}
			else if (team1.activeSkaters == 4 && team2.activeSkaters == 3) {
				advantage = "4 on 3";
			}
			else {
				advantage = "";
			}
			if (team1.goaliePulled) {
				if (advantage == "") {
					advantage = "Empty Net";
				}
				else {
					advantage = "EN " + advantage;
				}
			}
			return advantage;
		}

		public static string calculateEvenStrength(HockeyTeam team1, HockeyTeam team2) {
			string advantage = "";
			if (team1.activeSkaters == 5 && team2.activeSkaters == 5) {
				advantage = "";
			}
			else if (team1.activeSkaters == 4 && team2.activeSkaters == 4) {
				advantage = "4 on 4";
			}
			else if (team1.activeSkaters == 3 && team2.activeSkaters == 3) {
				advantage = "3 on 3";
			}
			return advantage;
		}

		public static string calculateTimeToDisplay(HockeyTeam home, HockeyTeam away) {
			List<TimeSpan> penalties = new List<TimeSpan> {
				home.penalty1,
				home.penalty2,
				away.penalty1,
				away.penalty2
			};
			penalties.RemoveAll(isZeroTimeSpan);
			string displayTime;
			if (penalties.Count > 0) {
				TimeSpan minimum = penalties.Min();
				displayTime = timeSpanToTimeString(minimum);
			}
			else {
				displayTime = "";
			}
			
			return displayTime;
		}

		public static string timeSpanToTimeString(TimeSpan penalty) {
			string formattedString = "";
			if (penalty.TotalMilliseconds != 0) {
				formattedString += penalty.Minutes.ToString();
				formattedString += ':';
				string s = penalty.Seconds.ToString();
				s = (s.Length > 1 ? s : "0" + s);
				formattedString += s;
			}
			return formattedString;
		}

		private static bool isZeroTimeSpan(TimeSpan t) {
			return t.TotalMilliseconds == 0;
		}
	}
}
