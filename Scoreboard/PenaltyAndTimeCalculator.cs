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

		public static string calculatePlayerAdvantage(HockeyTeam home, HockeyTeam away) {
			string advantage = "";
			List<int> playerCounts = new List<int> {
				home.activeSkaters,
				away.activeSkaters
			};
			int max = playerCounts.Max();
			int min = playerCounts.Min();
			if (max == 6 && min == 5) {
				advantage = "Empty Net";
			}
			else {
				advantage += max.ToString();
				advantage += " On ";
				advantage += min.ToString();
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
				displayTime = timeSpanToPenaltyString(minimum);
			}
			else {
				displayTime = "";
			}
			
			return displayTime;
		}

		public static string timeSpanToPenaltyString(TimeSpan penalty) {
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
