using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BaseLibrary.Process
{
    /// <summary>
    /// Launch process allows to launch any given process by passing command line arguments
    /// and returns appropriate output
    /// </summary>
    public class LaunchAnyProcess
    {
        /// <summary>
        /// Launches the passed process
        /// </summary>
        /// <param name="processName">pass process name</param>
        /// <param name="commandLineArguments">pass command line argument</param>
        /// <returns></returns>
        public static string Launch(string processName, string commandLineArguments, ref int returnCode, bool waitToExit)
        {
            //if (!File.Exists(processName))
            //    throw new Exception(processName + " does not exist");
            try
            {
                returnCode = 0;
                string output = "";
                string errorOutput = "";
                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    System.Diagnostics.ProcessStartInfo sInfo = new System.Diagnostics.ProcessStartInfo();
                    sInfo.FileName = processName;
                    if (commandLineArguments == null)
                        commandLineArguments = "";
                    sInfo.Arguments = commandLineArguments;
                    sInfo.UseShellExecute = false;
                    sInfo.RedirectStandardError = true;
                    sInfo.RedirectStandardOutput = true;
                    sInfo.CreateNoWindow = true;
                    proc.StartInfo = sInfo;

                    output = DateTime.Now + " : Process " + sInfo.FileName + " started with arguments " + sInfo.Arguments + Environment.NewLine;
                    output += " **********Output Log******** " + Environment.NewLine;
                    errorOutput += " **********Error Log******** " + Environment.NewLine;

                    proc.OutputDataReceived += new DataReceivedEventHandler
                    (
                        delegate (object sender, DataReceivedEventArgs e)
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                output += DateTime.Now + " : " + e.Data + Environment.NewLine;
                            }
                        }
                    );

                    proc.ErrorDataReceived += new DataReceivedEventHandler
                   (
                       delegate (object sender, DataReceivedEventArgs e)
                       {
                           if (!string.IsNullOrEmpty(e.Data))
                           {
                               errorOutput += DateTime.Now + " : " + e.Data + Environment.NewLine;
                           }
                       }
                   );

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    if (waitToExit)
                    {
                        proc.WaitForExit();
                        returnCode = proc.ExitCode;
                    }
                    //proc.Close();
                    //proc.Dispose();
                }
                if (returnCode != 0)
                {
                    return output + Environment.NewLine + errorOutput;
                }
                else
                {
                    return output;
                }
            }
            catch// (Exception ex)
            {
                returnCode = 255;
                //return ("Failed to launch process " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Executes process with output and erroroutput
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="commandLineArguments"></param>
        /// <param name="returnCode"></param>
        /// <param name="waitToExit"></param>
        /// <param name="output"></param>
        /// <param name="errorOutput"></param>
        public static void LaunchForErrorandOutputArgs(string processName, string commandLineArguments, ref int returnCode, bool waitToExit,
            ref string output, ref string errorOutput)
        {
            //if (!File.Exists(processName))
            //    throw new Exception(processName + " does not exist");
            string outputVal = "";
            string errorVal = "";
            try
            {
                returnCode = 0;
                output = "";
                errorOutput = "";


                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    System.Diagnostics.ProcessStartInfo sInfo = new System.Diagnostics.ProcessStartInfo();
                    sInfo.FileName = processName;
                    if (commandLineArguments == null)
                        commandLineArguments = "";
                    sInfo.Arguments = commandLineArguments;
                    sInfo.UseShellExecute = false;
                    sInfo.RedirectStandardError = true;
                    sInfo.RedirectStandardOutput = true;
                    sInfo.CreateNoWindow = true;
                    proc.StartInfo = sInfo;

                    //output = DateTime.Now + " : Process " + sInfo.FileName + " started with arguments " + sInfo.Arguments + Environment.NewLine;

                    proc.OutputDataReceived += new DataReceivedEventHandler
                    (
                        delegate (object sender, DataReceivedEventArgs e)
                        {
                            outputVal += e.Data;
                        }
                    );



                    proc.ErrorDataReceived += new DataReceivedEventHandler
                   (
                       delegate (object sender, DataReceivedEventArgs e)
                       {
                           errorVal += e.Data;
                       }
                   );

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    if (waitToExit)
                    {
                        proc.WaitForExit();
                        returnCode = proc.ExitCode;
                    }
                    //proc.Close();
                    //proc.Dispose();
                }
            }
            catch
            {
                returnCode = 256;
                throw;
            }
            finally
            {
                output = outputVal;
                errorOutput = errorVal;
            }
        }

        /// <summary>
        /// Returns out in synchronized mode
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="commandLineArguments"></param>
        /// <param name="returnCode"></param>
        /// <param name="waitToExit"></param>
        /// <param name="output"></param>
        /// <param name="errorOutput"></param>
        public static void LaunchForErrorandOutputArgsWithSyncOutput(string processName, string commandLineArguments, ref int returnCode, bool waitToExit,
    ref string output, ref string errorOutput)
        {
            //if (!File.Exists(processName))
            //    throw new Exception(processName + " does not exist");
            string outputVal = "";
            string errorVal = "";
            try
            {
                returnCode = 0;
                output = "";
                errorOutput = "";


                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    System.Diagnostics.ProcessStartInfo sInfo = new System.Diagnostics.ProcessStartInfo();
                    sInfo.FileName = processName;
                    if (commandLineArguments == null)
                        commandLineArguments = "";
                    sInfo.Arguments = commandLineArguments;
                    sInfo.UseShellExecute = false;
                    sInfo.RedirectStandardError = true;
                    sInfo.RedirectStandardOutput = true;
                    sInfo.CreateNoWindow = true;
                    proc.StartInfo = sInfo;

                    proc.Start();
                    if (waitToExit)
                    {
                        outputVal = proc.StandardOutput.ReadToEnd();
                        errorVal = proc.StandardError.ReadToEnd();
                        proc.WaitForExit();
                        returnCode = proc.ExitCode;
                    }

                }
            }
            catch
            {
                returnCode = 257;
                throw;
            }
            finally
            {
                output = outputVal;
                errorOutput = errorVal;
            }
        }


    }
}
