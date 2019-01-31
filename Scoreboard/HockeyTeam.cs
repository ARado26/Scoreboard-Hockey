using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Scoreboard{

	public class HockeyTeam {

		public int score { get; set; }
		public string name { get; set; }
		public int activeSkaters { get; set; }
		public ConcurrentQueue<int> penaltyQueue { get; set; }
		public int penalty1 { get; set; }
		public int penalty1Mins { get; set; }
		public int penalty1Sec { get; set; }
		public int penalty2 { get; set; }
		public int penalty2Mins { get; set; }
		public int penalty2Sec { get; set; }
		public Boolean goaliePulled { get; set; }

		public HockeyTeam(String teamName) {
			name = teamName;
			score = 0;
			activeSkaters = 5;
			penaltyQueue = new ConcurrentQueue<int>();
			penalty1 = 0;
			penalty2 = 0;
			goaliePulled = false;
		}

		public void addGoal() {
			++score;
		}

		public void subtractGoal() {
			if (score > 0) {
				--score;
			}
		}

		public void setScore(String scoreString) {
			if (int.TryParse(scoreString, out int newScore)) {
				score = newScore;
			}
		}

		public void calculateAndSetActiveSkaters() {
			int skaters = 5;
			if (goaliePulled) {
				++skaters;
			}
			if (penalty1 != 0) {
				--skaters;
			}
			if (penalty2 != 0) {
				--skaters;
			}
			activeSkaters = skaters;
		}

		public void queuePenalty(string minutesString, string secondsString) {
			int totalSeconds = 0;
			if (int.TryParse(minutesString, out int mins)) {
				if (int.TryParse(secondsString, out int sec)) {
					totalSeconds += sec;
					totalSeconds += mins * 60;
				}
			}
			penaltyQueue.Enqueue(totalSeconds);
			// TEST
			managePenalties();
			// END TEST
		}

		public void clearPen1() {
			penalty1 = 0;
			penalty1Mins = 0;
			penalty1Sec = 0;
			// TEST
			managePenalties();
			// END TEST
		}

		public void clearPen2() {
			penalty2 = 0;
			penalty2Mins = 0;
			penalty2Sec = 0;
			// TEST
			managePenalties();
			// END TEST
		}

		public void setPen1(string minutesString, string secondsString) {
			penalty1 = 0;
			queuePenalty(minutesString, secondsString);
			// TEST
			managePenalties();
			// END TEST
		}

		public void setPen2(string minutesString, string secondsString) {
			penalty2 = 0;
			queuePenalty(minutesString, secondsString);
			// TEST
			managePenalties();
			// END TEST
		}

		public int getAmtOfQueuedPenalties() {
			return penaltyQueue.Count();
		}

		public void toggleGoaliePulled() {
			goaliePulled = !goaliePulled;
			calculateAndSetActiveSkaters();
		}

		public bool hasPenalties() {
			return penalty1 != 0 || penalty2 != 0;
		}

		public void printInfo() {
			Console.WriteLine("Team Name: " + name);
			Console.WriteLine("\tScore: " + score);
			Console.WriteLine("\tSkaters: " + activeSkaters);
			Console.WriteLine("\tGoaliePulled: " + goaliePulled);
			Console.WriteLine("\tPen1: " + penalty1);
			Console.WriteLine("\tPen2: " + penalty2);
			Console.WriteLine("\tPenQSize: " + penaltyQueue.Count);
		}

		// CALL IN TIMER ONLY
		public void managePenalties() {
			while (penaltyQueuedAndPenaltySlotAvailable()) {
				if(penalty1 == 0) {
					if (penaltyQueue.TryDequeue(out int pen)) {
						penalty1 = pen;
						penalty1Mins = penalty1 / 60;
						penalty1Sec = penalty1 % 60;
					}
				}

				if (penaltyQueuedAndPenaltySlotAvailable()) {
					if (penaltyQueue.TryDequeue(out int pen)) {
						penalty2 = pen;
						penalty2Mins = penalty2 / 60;
						penalty2Sec = penalty2 % 60;
					}
				}
			}
		}

		private bool penaltyQueuedAndPenaltySlotAvailable() {
			return penaltyQueue.TryPeek(out int result) &&
				(penalty1 == 0 || penalty2 == 0);
		}
    }
}
