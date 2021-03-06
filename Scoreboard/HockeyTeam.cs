﻿using System;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Scoreboard {

	public class HockeyTeam {

		public int score { get; set; }
		public string name { get; set; }
		public int activeSkaters { get; set; }
		public ConcurrentQueue<TimeSpan> penaltyQueue { get; set; }
		public TimeSpan penalty1 { get; set; }
		public TimeSpan penalty2 { get; set; }
		public Boolean goaliePulled { get; set; }
		public string imagePath { get; set; }

		public HockeyTeam(String teamName) {
			name = teamName;
			score = 0;
			activeSkaters = 5;
			penaltyQueue = new ConcurrentQueue<TimeSpan>();
			penalty1 = new TimeSpan();
			penalty2 = new TimeSpan();
			goaliePulled = false;
			imagePath = @".\Images\Home_Cell_Specular.png";
		}

		public void fromJson(string json) {
			HockeyTeam saved = JsonConvert.DeserializeObject<HockeyTeam>(json);
			score = saved.score;
			name = saved.name;
			activeSkaters = saved.activeSkaters;
			penaltyQueue = saved.penaltyQueue;
			penalty1 = saved.penalty1;
			penalty2 = saved.penalty2;
			goaliePulled = saved.goaliePulled;
			imagePath = saved.imagePath;
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
			if (penalty1.TotalMilliseconds != 0) {
				--skaters;
			}
			if (penalty2.TotalMilliseconds != 0) {
				--skaters;
			}
			activeSkaters = skaters;
		}

		public void queuePenalty(TimeSpan penaltyTime) {
			penaltyQueue.Enqueue(penaltyTime);
		}

		public void clearPen1() {
			penalty1 = new TimeSpan();
		}

		public void clearPen2() {
			penalty2 = new TimeSpan();
		}

		public void setPen1(TimeSpan penaltyTime) {
			penalty1 = new TimeSpan();
			queuePenalty(penaltyTime);
		}

		public void setPen2(TimeSpan penaltyTime) {
			penalty2 = new TimeSpan();
			queuePenalty(penaltyTime);
		}

		public int getAmtOfQueuedPenalties() {
			return penaltyQueue.Count();
		}

		public void toggleGoaliePulled() {
			goaliePulled = !goaliePulled;
			calculateAndSetActiveSkaters();
		}

		public string logInfo() {
			string info = "";
			info += JsonConvert.SerializeObject(this);
			return info;
		}
		
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
