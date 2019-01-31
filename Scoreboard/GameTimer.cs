using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Scoreboard {
	public class GameTimer {
		private Timer timer;
		private int interval;

		private DateTime refreshStart;
		private DateTime timerStarted;
		private DateTime now;
		private TimeSpan elapsedTime;

		public TimeSpan gameClock { get; private set; }
		public TimeSpan homePen1 { get; private set; }
		public TimeSpan homePen2 { get; private set; }
		public TimeSpan awayPen1 { get; private set; }
		public TimeSpan awayPen2 { get; private set; }

		public event EventHandler TimeExpired;

		public GameTimer(int millisecondsInterval) {
			interval = millisecondsInterval;
			timer = new Timer(interval);
			timer.Elapsed += adjustTimersHandler;
		}
		
		public void startClock() {
			Console.WriteLine("Starting Clock: " + gameClock);
			now = System.DateTime.Now;
			timerStarted = now;
			refreshStart = now;
			timer.Start();
		}

		public void stopClock() {
			timer.Stop();
			adjustTimers();
			Console.WriteLine("Stopping Clock: " + gameClock);
			Console.WriteLine("Elapsed time: " + (System.DateTime.Now - timerStarted));
		}

		public void setTimerFields(GameInfo game, HockeyTeam home, HockeyTeam away) {
			gameClock = game.gameTime;
			Console.WriteLine("GameTime:" + gameClock);
			homePen1 = home.penalty1;
			homePen2 = home.penalty2;
			awayPen1 = away.penalty1;
			awayPen2 = away.penalty2;
		}


		private void adjustTimers() {
			now = System.DateTime.Now;
			elapsedTime = (now - refreshStart);
			refreshStart = now;
			gameClock -= elapsedTime;
			homePen1 -= elapsedTime;
			homePen2 -= elapsedTime;
			awayPen1 -= elapsedTime;
			awayPen2 -= elapsedTime;
			handleNegativeTimers();
			//Console.WriteLine("Elapsed:" + elapsedTime);
			//Console.WriteLine("GameTime:" + gameClock);
			//Console.WriteLine("HomePen1:" + homePen1);
			//Console.WriteLine("HomePen2:" + homePen2);
			//Console.WriteLine("AwayPen1:" + awayPen1);
			//Console.WriteLine("AwayPen2:" + awayPen2);
		}

		private void handleNegativeTimers() {
			if (gameClock.TotalMilliseconds < 0) {
				gameClock = new TimeSpan();
			}
			if (homePen1.TotalMilliseconds < 0) {
				homePen1 = new TimeSpan();
			}
			if (homePen2.TotalMilliseconds < 0) {
				homePen2 = new TimeSpan();
			}
			if (awayPen1.TotalMilliseconds < 0) {
				awayPen1 = new TimeSpan();
			}
			if (awayPen2.TotalMilliseconds < 0) {
				awayPen2 = new TimeSpan();
			}
		}

		private void adjustTimersHandler(Object source, ElapsedEventArgs e) {
			if (gameClock.TotalMilliseconds <= 0) {
				stopClock();
				OnTimeExpire(EventArgs.Empty);
			}else {
				adjustTimers();
			}
		}
		
		protected virtual void OnTimeExpire(EventArgs e) {
			TimeExpired?.Invoke(this, e);
		}

		//private void addAdjustTimerHandlerValue(double time, string name) {
		//	timer.Elapsed += (sender, e) => subtractElapsedFromTime(time, name);
		//}

		//private void subtractElapsedFromTime(double time, string name) {
		//	elapsedTime = (System.DateTime.Now - startTime).TotalMilliseconds;
		//	time -= elapsedTime;
		//	Console.WriteLine(name + ':' + time);
		//}
	}
}
