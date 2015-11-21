using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
                throw new Exception("give the name of the input file");
            String InputFile = args[0];
            string line;
            using (StreamReader Sr = new StreamReader(InputFile))
            {
                while((line = Sr.ReadLine())!=null)
                {
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] obsTaglines = line.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                    if(obsTaglines.Length!=2)
                        Console.WriteLine("incorrect split");
                    string[] observWords = obsTaglines[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] tagWords = obsTaglines[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (observWords.Length + 1 != tagWords.Length)
                        throw new Exception("words and tags dont match");
                    string temp;

                    for (int i = 1; i < observWords.Length-1; i++)
                    {
                        temp = tagWords[i].Substring(tagWords[i].LastIndexOf("_")+1);
                        Console.Write(observWords[i]+"/"+temp+" ");
                        
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
