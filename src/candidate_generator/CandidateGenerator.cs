using System;
using System.IO;

namespace CandidateGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "./candidate_data.txt";
            StreamWriter streamWriter = new StreamWriter(path, false);
            int cnt = Convert.ToInt32(args[0]);
            for (int i = 0; i < cnt; i++)
            {
                streamWriter.WriteLine(i);

            }
            streamWriter.Close();
        }
    }
}
