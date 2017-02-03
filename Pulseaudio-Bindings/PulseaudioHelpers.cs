using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PulseaudioBindings
{
	public class PulseaudioHelpers
	{
		public static string[] getSourceDeviceNames(){
			List<string> names = new List<string> ();
			var psi = new ProcessStartInfo ("pacmd", "list-sources");
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;
			var p = Process.Start (psi);
			p.WaitForExit ();

			var lines = p.StandardOutput.ReadToEnd ().Split ('\n');

			var r = new Regex (@"name: <(.+)>");
			foreach (var line in lines) {
				Match m = r.Match (line);
				if (m.Success) {
					var d = m.Groups [1].ToString ().Trim ();
					if (d != null && d.Length > 0) {
						names.Add (d);
					}
				}
			}
			return names.ToArray ();
		}

		public static string[] getSinkDeviceNames(){
			List<string> names = new List<string> ();
			var psi = new ProcessStartInfo ("pacmd", "list-sinks");
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;
			var p = Process.Start (psi);
			p.WaitForExit ();

			var lines = p.StandardOutput.ReadToEnd ().Split ('\n');

			var r = new Regex (@"name: <(.+)>");
			foreach (var line in lines) {
				Match m = r.Match (line);
				if (m.Success) {
					var d = m.Groups [1].ToString ().Trim ();
					if (d != null && d.Length > 0) {
						names.Add (d);
					}
				}
			}
			return names.ToArray ();
		}
	}
}

