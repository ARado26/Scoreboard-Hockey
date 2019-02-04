using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scoreboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard.Tests {
	[TestClass()]
	public class HockeyTeamTests {
		public HockeyTeam team;
		[TestInitialize()]
		public void Initialize() {
			team = new HockeyTeam("TEST");
		}
		[TestMethod()]
		public void HockeyTeamTest() {
			Assert.AreEqual("TEST", team.name);
			Assert.AreEqual(false, team.goaliePulled);
		}

		[TestMethod()]
		public void addGoalTest() {
			team.addGoal();
			Assert.AreEqual(1, team.score);
		}

		[TestMethod()]
		public void subtractGoalTest() {
			team.setScore(5.ToString());
			team.subtractGoal();
			Assert.AreEqual(4, team.score);
		}

		[TestMethod()]
		public void setScoreTest() {
			team.setScore(5.ToString());
			Assert.AreEqual(5, team.score);
		}

		[TestMethod()]
		public void calculateAndSetActiveSkatersTest() {
			team.queuePenalty(new TimeSpan(0, 2, 0));
			team.managePenalties();
			team.calculateAndSetActiveSkaters();
			Assert.AreEqual(4, team.activeSkaters);
		}

		[TestMethod()]
		public void queuePenaltyTest() {
			team.queuePenalty(new TimeSpan(0, 2, 0));
			team.managePenalties();
			Assert.AreEqual(new TimeSpan(0, 2, 0), team.penalty1);
		}

		[TestMethod()]
		public void clearPen1Test() {
			team.queuePenalty(new TimeSpan(0, 2, 0));
			team.managePenalties();
			team.clearPen1();
			Assert.AreNotEqual(new TimeSpan(0, 2, 0), team.penalty1);
		}

		[TestMethod()]
		public void clearPen2Test() {
			team.queuePenalty(new TimeSpan(0, 2, 0));
			team.queuePenalty(new TimeSpan(0, 2, 0));
			team.managePenalties();
			team.clearPen2();
			Assert.AreNotEqual(new TimeSpan(0, 2, 0), team.penalty2);
		}

		[TestMethod()]
		public void setPen1Test() {
			team.setPen1(new TimeSpan(0, 2, 0));
			team.managePenalties();
			Assert.AreEqual(new TimeSpan(0, 2, 0), team.penalty1);
		}

		[TestMethod()]
		public void setPen2Test() {
			team.setPen1(new TimeSpan(0, 1, 30));
			team.setPen2(new TimeSpan(0, 2, 0));
			team.managePenalties();
			Assert.AreEqual(new TimeSpan(0, 2, 0), team.penalty2);
		}

		[TestMethod()]
		public void getAmtOfQueuedPenaltiesTest() {
			team.queuePenalty(new TimeSpan(0, 2, 0));
			Assert.AreEqual(1, team.getAmtOfQueuedPenalties());
		}

		[TestMethod()]
		public void toggleGoaliePulledTest() {
			team.toggleGoaliePulled();
			Assert.AreEqual(true, team.goaliePulled);
		}

		[TestMethod()]
		public void logInfoTest() {
			string s = team.logInfo();
			Assert.AreNotEqual("", s);
		}

		[TestMethod()]
		public void managePenaltiesTest() {
			team.setPen1(new TimeSpan(0, 2, 0));
			Assert.AreNotEqual(new TimeSpan(0, 2, 0), team.penalty1);
			team.managePenalties();
			Assert.AreEqual(new TimeSpan(0, 2, 0), team.penalty1);
		}
	}
}