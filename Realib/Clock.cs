using System;
using System.Collections.Generic;
using System.Threading;

namespace Realib
{
	public class Clock
	{
		List<ClockedStructure> csl;
		Thread clockthread;
		bool running = false;

		public Clock ()
		{
			csl = new List<ClockedStructure> ();
		}

		public void RegisterClockedStructure(ClockedStructure cs){
			csl.Add (cs);
		}

		private void clocktask(){
			while (running) {
				foreach (ClockedStructure cs in csl) {
					cs.tick ();
				}
			}
		}

		public void Start(){
			ThreadStart ts = new ThreadStart (clocktask);
			clockthread = new Thread (ts);
			running = true;
			clockthread.Start ();
		}

		public void Stop(){
			running = false;
			clockthread.Join ();
		}
	}
}

