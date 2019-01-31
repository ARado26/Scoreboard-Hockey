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
		public ConcurrentQueue<TimeSpan> penaltyQueue { get; set; }
		public TimeSpan penalty1 { get; set; }
		public TimeSpan penalty2 { get; set; }
		public Boolean goaliePulled { get; set; }

		public HockeyTeam(String teamName) {
			name = teamName;
			score = 0;
			activeSkaters = 5;
			penaltyQueue = new ConcurrentQueue<TimeSpan>();
			penalty1 = new TimeSpan();
			penalty2 = new TimeSpan();
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
			if (penalty1.TotalMilliseconds != 0) {
				--skaters;
			}
			if (penalty2.TotalMilliseconds != 0) {
				--skaters;
			}
			activeSkaters = skaters;
		}

		public void queuePenalty(string minutesString, string secondsString) {
			Console.WriteLine("Queueing Penalty " + System.DateTime.Now.ToLongTimeString());
			TimeSpan penaltyTime = new TimeSpan();
			if (int.TryParse(minutesString, out int mins)) {
				if (int.TryParse(secondsString, out int sec)) {
					penaltyTime = new TimeSpan(0, mins, sec);
					penaltyQueue.Enqueue(penaltyTime);
				}
			}			
			// TEST
			managePenalties();
			// END TEST
		}

		public void clearPen1() {
			penalty1 = new TimeSpan();
			// TEST
			managePenalties();
			// END TEST
		}

		public void clearPen2() {
			penalty2 = new TimeSpan();
			// TEST
			managePenalties();
			// END TEST
		}

		public void setPen1(string minutesString, string secondsString) {
			penalty1 = new TimeSpan();
			queuePenalty(minutesString, secondsString);
			// TEST
			managePenalties();
			// END TEST
		}

		public void setPen2(string minutesString, string secondsString) {
			penalty2 = new TimeSpan();
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
			return penalty1.TotalMilliseconds != 0 || penalty2.TotalMilliseconds != 0;
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
				if(penalty1.TotalMilliseconds == 0) {
					if (penaltyQueue.TryDequeue(out TimeSpan pen)) {
						penalty1 = pen;
					}
				}

				if (penaltyQueuedAndPenaltySlotAvailable()) {
					if (penaltyQueue.TryDequeue(out TimeSpan pen)) {
						penalty2 = pen;
					}
				}
			}
			calculateAndSetActiveSkaters();
		}

		private bool penaltyQueuedAndPenaltySlotAvailable() {
			return penaltyQueue.TryPeek(out TimeSpan result) &&
				(penalty1.TotalMilliseconds == 0 || penalty2.TotalMilliseconds == 0);
		}
    }
}
