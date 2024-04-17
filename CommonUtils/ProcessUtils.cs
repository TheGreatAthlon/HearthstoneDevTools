using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonUtils
{
	public class ProcessUtils
	{
		public static int ExecuteProcessWithReturnCode(string filename, string args, string workDir)
		{
			List<string> stdout = new List<string>();

			return ExecuteProcessWithReturnCode(filename, args, workDir, stdout);
		}

		public static int ExecuteProcessWithReturnCode(string filename, string args, string workDir, List<string> stdout)
		{
			var workDirBeforeExec = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = workDir;

				var proc = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = filename,
						Arguments = args,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					}
				};
				proc.OutputDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrWhiteSpace(e.Data))
					{
						stdout.Add(e.Data);
						Console.WriteLine($"[STDOUT]: {e.Data}");
					}
				};
				proc.ErrorDataReceived += (sender, e) =>
				{
					if (!string.IsNullOrWhiteSpace(e.Data))
					{
						Console.WriteLine($"[STDERR]: {e.Data}");
					}
				};

				Console.WriteLine($"WorkDir: {workDir}");
				Console.WriteLine($"> {filename} {args}");

				proc.Start();
				proc.BeginOutputReadLine();
				proc.BeginErrorReadLine();
				proc.WaitForExit();

				return proc.ExitCode;
			}
			finally
			{
				Environment.CurrentDirectory = workDirBeforeExec;
			}
		}

		public static void ExecuteProcess(string filename, string args, string workDir)
		{
			int exitCode = ExecuteProcessWithReturnCode(filename, args, workDir);
			if (exitCode != 0)
			{
				throw new SystemException($"Process ended with exit code {exitCode}");
			}
		}

		public static void MkLinkDir(string link, string targetDir)
		{
			ExecuteProcess("cmd.exe", $"/c mklink /D {link} \"{targetDir}\"", Environment.CurrentDirectory);
		}
	}
}
