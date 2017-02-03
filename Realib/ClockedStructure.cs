using System;

namespace Realib
{
	public interface ClockedStructure
	{
		/// <summary>
		/// Process the tick event. Block while doing so
		/// </summary>
		void tick();
	}
}

