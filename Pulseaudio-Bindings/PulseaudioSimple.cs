using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PulseaudioBindings
{
	class PulseaudioSimple
	{
		public enum pa_sample_format{
			PA_SAMPLE_U8, 	//Unsigned 8 Bit PCM.
			PA_SAMPLE_ALAW, //8 Bit a-Law
			PA_SAMPLE_ULAW, //8 Bit mu-Law
			PA_SAMPLE_S16LE, //Signed 16 Bit PCM, little endian (PC)
			PA_SAMPLE_S16BE, //Signed 16 Bit PCM, big endian.
			PA_SAMPLE_FLOAT32LE, //32 Bit IEEE floating point, little endian (PC), range -1.0 to 1.0
			PA_SAMPLE_FLOAT32BE, //32 Bit IEEE floating point, big endian, range -1.0 to 1.0
			PA_SAMPLE_S32LE, //Signed 32 Bit PCM, little endian (PC)
			PA_SAMPLE_S32BE, //Signed 32 Bit PCM, big endian.
			PA_SAMPLE_S24LE, //Signed 24 Bit PCM packed, little endian (PC).
			PA_SAMPLE_S24BE, //Signed 24 Bit PCM packed, big endian.
			PA_SAMPLE_S24_32LE, //Signed 24 Bit PCM in LSB of 32 Bit words, little endian (PC).
			PA_SAMPLE_S24_32BE, //Signed 24 Bit PCM in LSB of 32 Bit words, big endian.
			PA_SAMPLE_MAX, //Upper limit of valid sample types.
			PA_SAMPLE_INVALID 
		};

		///		The direction of a pa_stream object.
		public enum pa_stream_direction{
			PA_STREAM_NODIRECTION, //Invalid direction.
			PA_STREAM_PLAYBACK, //Playback stream.
			PA_STREAM_RECORD, //Record stream.
			PA_STREAM_UPLOAD //Sample upload stream. 
		}

		public struct pa_sample_spec{
			public pa_sample_format format;
			public uint rate;
			public byte channels;
		}

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Creates a new pulseaudio_simple context
		/// </summary>
		/// <returns>A pointer to the newly created pulseaudio_simple context</returns>
		/// <param name="server">Server.</param>
		/// <param name="name">Name.</param>
		/// <param name="dir">Dir.</param>
		/// <param name="dev">Dev.</param>
		/// <param name="stream_name">Stream name.</param>
		/// <param name="ss">Sample Spec.</param>
		/// <param name="map">Map.</param>
		/// <param name="attr">Attr.</param>
		/// <param name="error">Error.</param>
		public static extern unsafe IntPtr pa_simple_new (string server, string name, pa_stream_direction dir, string dev, string stream_name, pa_sample_spec *ss, void* map, void* attr, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Reads bytes bytes from the server. Blocks until it has actually read that many bytes
		/// </summary>
		/// <returns>A negative number on failure</returns>
		/// <param name="s">The pulseaudio_simple context</param>
		/// <param name="data">A byte array in which to write the data</param>
		/// <param name="bytes">The number of bytes to read</param>
		/// <param name="error">A pointer to an int to which an error number may be written</param>
		public static extern unsafe IntPtr pa_simple_read (IntPtr s,[Out] float[] data, uint bytes, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Writes bytes bytes to the server.
		/// </summary>
		/// <returns>A negative number on failure</returns>
		/// <param name="s">The pulseaudio_simple context</param>
		/// <param name="data">A byte array of data to write</param>
		/// <param name="bytes">The number of bytes to read</param>
		/// <param name="error">A pointer to an int to which an error number may be written</param>
		public static extern unsafe IntPtr pa_simple_write (IntPtr s, byte[] data, uint bytes, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Wait until all data already written is played by the daemon.
		/// </summary>
		/// <returns>-1 in case of error</returns>
		/// <param name="s">The pulseaudio_simple context to drain.</param>
		/// <param name="error">A pointer to an int to which an error number may be written</param>
		public static extern unsafe int pa_simple_drain (IntPtr s, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Clears the input or output buffer of the context
		/// </summary>
		/// <returns>-1 in case of error</returns>
		/// <param name="s">The pulseaudio_simple context to clear.</param>
		/// <param name="error">A pointer to an int to which an error number may be written</param>
		public static extern unsafe int pa_simple_flush (IntPtr s, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Queries the latency of the context
		/// </summary>
		/// <returns>The number of microseconds of latency on the context</returns>
		/// <param name="s">The pulseaudio_simple context.</param>
		/// <param name="error">A pointer to an int to which an error number may be written</param>
		public static extern unsafe long pa_simple_get_latency(IntPtr s, int* error);

		[DllImport ("libpulse-simple.so")]
		/// <summary>
		/// Frees all data associated with the pulseaudio_simple context
		/// </summary>
		/// <param name="s">The pulseaudio_simple context to free.</param>
		public static extern unsafe void pa_simple_free (IntPtr s);



		[DllImport ("libpulse.so")]
		private static extern unsafe IntPtr pa_strerror(int error);

		/// <summary>
		/// Gets the error string for an error number
		/// </summary>
		/// <returns>The error string</returns>
		/// <param name="error">The error number</param>
		public static string getErrorString(int error){
			return Marshal.PtrToStringAuto (pa_strerror (error));
		}



	}
}

