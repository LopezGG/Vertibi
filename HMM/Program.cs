using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
                throw new Exception("Incorect number of arguments");
            string inputHmm = args[0];
            string testFile = args[1];
            string outputFile = args[2];
            HMM HmmInput = new HMM();
            HmmInput.validateFillHmm(inputHmm);
            Stopwatch stopwatch = Stopwatch.StartNew();
            String Input;
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
            StreamWriter Sw = new StreamWriter(outputFile,true);
            using (StreamReader Sr = new StreamReader(testFile))
            {
                while((Input=Sr.ReadLine())!=null)
                {
                    Input = "<s> " + Input + " </s>";
                    if (String.IsNullOrWhiteSpace(Input))
                        continue;
                    Viterbi(Input, HmmInput,Sw);
                }
            }
             Console.WriteLine("finished building the vertibi table : ");
             Sw.Close();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //Console.ReadLine();
            
        }
        public static void Viterbi(String Input, HMM HmmInput, StreamWriter Sw)
        {
            string[] words = Input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Double[,] Vertibi = new Double[HmmInput.statesList.Count + 2, words.Length + 2];
            int[,] BackTrack = new int[HmmInput.statesList.Count + 2, words.Length + 2];

            for (int i = 0; i < HmmInput.revStatesList.Count; i++)
            {
                double initProb = 0;
                if (HmmInput.initBlock.ContainsKey(HmmInput.revStatesList[i]))
                {
                    initProb = Math.Log10((HmmInput.initBlock[HmmInput.revStatesList[i]]) == 1.0 ? 0.99 : HmmInput.initBlock[HmmInput.revStatesList[i]]);
                }

                else
                    initProb = 0;
                Vertibi[i, 0] = initProb;
                BackTrack[i, 0] = -1;
            }

            double maxProb = int.MinValue, curProb, transProb = 0, emissionProb = 0;
            string curTag, curword, prevTag, unknown = @"<unk>";
            for (int t = 0; t < words.Length; t++)
            {
                curword = words[t];
                for (int j = 0; j < HmmInput.revStatesList.Count; j++)
                {
                    prevTag = HmmInput.revStatesList[j];
                    if (!HmmInput.TransitionBlock.ContainsKey(prevTag))
                        continue;
                    if (Vertibi[j, t] == 0.0)
                        continue;
                    foreach (var State in HmmInput.TransitionBlock[prevTag])
                    {
                        curTag = State.Key;
                        int i = HmmInput.statesList[curTag];
                        if (HmmInput.LogTransitionBlock.ContainsKey(prevTag) && HmmInput.LogTransitionBlock[prevTag].ContainsKey(curTag))
                            transProb = (HmmInput.LogTransitionBlock[prevTag])[curTag];
                        else
                            transProb = 0;

                        if (HmmInput.logEmissionBlock.ContainsKey(curTag) && HmmInput.logEmissionBlock[curTag].ContainsKey(curword))
                            emissionProb = (HmmInput.logEmissionBlock[curTag])[curword];
                        else if (!HmmInput.symbolList.ContainsKey(curword) && HmmInput.logEmissionBlock[curTag].ContainsKey(unknown))
                            emissionProb = (HmmInput.logEmissionBlock[curTag])[unknown];
                        else
                            emissionProb = 0;

                        if (transProb == 0 || emissionProb == 0)
                            curProb = int.MinValue;
                        else
                            curProb = Vertibi[j, t] + transProb + emissionProb;
                        if (Vertibi[i, t + 1] == 0.0 || curProb > Vertibi[i, t + 1])
                        {
                            Vertibi[i, t + 1] = curProb;
                            BackTrack[i, t + 1] = j;
                        }
                    }
                }

            }
            //this gives the index from which we must start backtracking
            int maxStateRowIndex = 0;
            int wordIndex = words.Length ;
            maxProb = int.MinValue;
            for (int i = 0; i < HmmInput.revStatesList.Count; i++)
            {
                curProb = Vertibi[i, wordIndex];
                if (curProb > maxProb)
                {
                    maxStateRowIndex = i;
                    maxProb = curProb;
                }
            }
            List<String> sb = new List<String>();
            sb.Add(Convert.ToString(Vertibi[maxStateRowIndex, wordIndex]));

            while (wordIndex > 0 && maxStateRowIndex >= 0)
            {
                sb.Add(HmmInput.revStatesList[maxStateRowIndex]);
                maxStateRowIndex = BackTrack[maxStateRowIndex, wordIndex--];
            }
            sb.Add(" => ");
            sb.Add(Input);
            sb.Reverse();
            var result = string.Join(" ", sb);
            Sw.WriteLine(result);
           
        }
    }
}
