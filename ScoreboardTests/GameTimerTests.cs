using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scoreboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard.Tests {
	[TestClass()]
	public class GameTimerTests {
		GameTimer t;
		GameInfo g;
		HockeyTeam h;
		HockeyTeam a;
		const int interval = 1000;
		[TestInitialize()]
		public void init() {
			t = new GameTimer(interval);
			g = new GameInfo();
			g.setGameTime(new TimeSpan(0, 20, 0));
			h = new HockeyTeam("h");
			a = new HockeyTeam("a");
		}


		[TestMethod()]
		public void GameTimerTest() {
			Assert.AreEqual(0, t.penaltyQueue.Count);
			Assert.AreEqual(0, t.clearQueue.Count);
			Assert.AreEqual(false, t.Running);
		}

		[TestMethod()]
		public void startClockTest() {
			t.setTimerFields(g, h, a);
			t.startClock();
			Assert.AreEqual(true, t.Running);
		}

		[TestMethod()]
		public void stopClockTest() {
			t.setTimerFields(g, h, a);
			t.startClock();
			Assert.AreEqual(true, t.Running);
			t.stopClock();
			Assert.AreEqual(false, t.Running);
		}

		[TestMethod()]
		public void setTimerFieldsTest() {
			t.setTimerFields(g, h, a);
			Assert.AreEqual(g.gameTime, t.gameClock);
		}

		[TestMethod()]
		public void enqueuePenaltyTest() {
			Assert.AreEqual(0, t.penaltyQueue.Count);
			t.enqueuePenalty("HOME", new TimeSpan(0, 2, 0));
			Assert.AreEqual(1, t.penaltyQueue.Count);
		}

		[TestMethod()]
		public void clearPenaltyTest() {
			Assert.AreEqual(0, t.clearQueue.Count);
			t.clearPenalty("HOME1");
			Assert.AreEqual(1, t.clearQueue.Count);
		}
	}
}