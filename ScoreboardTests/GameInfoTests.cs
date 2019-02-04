using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scoreboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard.Tests {
	[TestClass()]
	public class GameInfoTests {
		[TestMethod()]
		public void GameInfoTest() {
			GameInfo g = new GameInfo();
			Assert.AreEqual("PG", g.period);
			Assert.AreEqual(0, g.gameTime.TotalMilliseconds);
		}

		[TestMethod()]
		public void setGameTimeTest() {
			GameInfo g = new GameInfo();
			g.setGameTime(new TimeSpan(0, 10, 0));
			Assert.AreEqual(10, g.gameTime.Minutes);
		}

		[TestMethod()]
		public void setPeriodTest() {
			GameInfo g = new GameInfo();
			g.setPeriod("TEST");
			Assert.AreEqual("TEST", g.period);
		}

		[TestMethod()]
		public void logInfoTest() {
			GameInfo g = new GameInfo();
			string s = g.logInfo();
			Assert.AreNotEqual("", s);
		}
	}
}