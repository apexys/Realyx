using System;

namespace Realib
{
	/// <summary>
	/// Expands one of the two channels of stereo input into both channels
	/// </summary>
	public class MonoToStereoEffect
	{
		public enum Channel{
			Left,
			Right
		}

		public bool leftChannelSelected = true;
		public MonoToStereoEffect (Channel channel)
		{
			if(channel == Channel.Left){
				leftChannelSelected = true;
			}else{
				leftChannelSelected = false;
			}
		}

		public void process(ref Realib.StereoAudioFrame saf){
			if (leftChannelSelected) {
				Array.Copy (saf.left_data, saf.right_data, Realib.StereoAudioFrame.MAXLEN);
			} else {
				Array.Copy (saf.right_data, saf.left_data, Realib.StereoAudioFrame.MAXLEN);
			}
		}
	}
}

