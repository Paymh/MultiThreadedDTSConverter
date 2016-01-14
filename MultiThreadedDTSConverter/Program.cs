using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MultiThreadedDTSConverter
{
    class Program
    {
        static string currentFile = "";
        static List<string> convertFiles = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your file path to search.");
            Console.Write("> ");
            string path = Console.ReadLine();
            string[] filenames = Directory.GetFiles(path, "*.mkv", SearchOption.AllDirectories);

            foreach (string s in filenames)
            {
                if (File.Exists(s))
                {
                    currentFile = s;
                    Process p = new Process();
                    p.StartInfo.Arguments = "-i " + "\"" + s + "\"";
                    p.StartInfo.FileName = "ffmpeg.exe";
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.ErrorDataReceived += P_ErrorDataReceived;
                    p.OutputDataReceived += P_ErrorDataReceived;
                    p.WaitForExit();
                    if (p.HasExited)
                    {
                        p.CancelErrorRead();
                        p.CancelOutputRead();
                        p.Close();
                    }
                }
            }

            convertFiles = convertFiles.Distinct().ToList();

            Console.WriteLine("File to convert: " + convertFiles.Count);
            runThread1();
            runThread2();
            runThread3();
            runThread4();

            Console.Write("Press any key to exit.");
            Console.ReadKey();

        }

        private static void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.IndexOf("Audio: dts", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                convertFiles.Add(currentFile);
                Console.WriteLine(currentFile);
            }
        }

        static void convertMovie(string path, int thread)
        {
            Process p = new Process();
            p.StartInfo.Arguments = "-i " + "\"" + path + "\"" + " -c:v copy -c:a aac -b:a 320k -strict experimental -y " + Path.GetFileNameWithoutExtension(path) + ".mp4";
            p.StartInfo.FileName = "ffmpeg.exe";
            p.Start();
            p.WaitForExit();

            Console.WriteLine("Convert File: " + path + " on thread " + thread);
            if (convertFiles.Count > 0)
            {
                if (thread == 1)
                    runThread1();

                if (thread == 2)
                    runThread2();

                if (thread == 3)
                    runThread3();

                if (thread == 4)
                    runThread4();
            }
        }

        static void runThread1()
        {
            if (convertFiles.Count > 0)
            {
                var workingFile = convertFiles[0];
                convertFiles.RemoveAt(0);
                Thread t = new Thread(() => convertMovie(workingFile, 1));
                Console.WriteLine("Converting file: " + workingFile);
                t.Start();
            }
        }



        static void runThread2()
        {
            if (convertFiles.Count > 0)
            {
                var workingFile = convertFiles[0];
                convertFiles.RemoveAt(0);
                Thread t = new Thread(() => convertMovie(workingFile, 2));
                Console.WriteLine("Converting file: " + workingFile);
                t.Start();
            }
        }

        static void runThread3()
        {
            if (convertFiles.Count > 0)
            {
                var workingFile = convertFiles[0];
                convertFiles.RemoveAt(0);
                Thread t = new Thread(() => convertMovie(workingFile, 3));
                Console.WriteLine("Converting file: " + workingFile);
                t.Start();
            }
        }

        static void runThread4()
        {
            if (convertFiles.Count > 0)
            {
                var workingFile = convertFiles[0];
                convertFiles.RemoveAt(0);
                Thread t = new Thread(() => convertMovie(workingFile, 4));
                Console.WriteLine("Converting file: " + workingFile);
                t.Start();
            }
        }
    }
}
