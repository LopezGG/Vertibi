using System;
using System.Collections.Generic;
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
            String Input = "<s> Influential members of the House Ways and Means Committee introduced legislation that would restrict how the new savings-and-loan bailout agency can raise capital , creating another potential obstacle to the government 's sale of sick thrifts ." + @" </s> </s>";
            string[] words = Input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Double[,] Vertibi = new Double[HmmInput.statesList.Count + 2, words.Length + 2];
            int[,] BackTrack = new int[HmmInput.statesList.Count + 2, words.Length + 2];

            for (int i = 0; i < HmmInput.revStatesList.Count; i++)
            {
                double initProb = 0;
                if (HmmInput.initBlock.ContainsKey(HmmInput.revStatesList[i]))
                {
                    initProb = Math.Log10((HmmInput.initBlock[HmmInput.revStatesList[i]]) == 1.0 ? 0.99 : HmmInput.initBlock[HmmInput.revStatesList[i]] );
                }

                else
                    initProb = 0;
                Vertibi[i, 0] = initProb;
                BackTrack[i, 0] = -1;
            }

            double maxProb = int.MinValue, curProb, transProb = 0, emissionProb = 0;
            string curTag,curword,prevTag,unknown = @"<unk>";
            for (int t = 0; t < words.Length; t++)
            {
                curword = words[t];
                for (int j = 0; j < HmmInput.revStatesList.Count; j++)
                {
                    prevTag = HmmInput.revStatesList[j];
                    for (int i = 0; i < HmmInput.revStatesList.Count; i++)
                    {
                        curTag = HmmInput.revStatesList[i];
                        if (HmmInput.TransitionBlock.ContainsKey(prevTag) && HmmInput.TransitionBlock[prevTag].ContainsKey(curTag))
                            transProb = Math.Log10((HmmInput.TransitionBlock[prevTag])[curTag]);
                        else
                            transProb = 0;

                        if (HmmInput.EmissionBlock.ContainsKey(curTag) && HmmInput.EmissionBlock[curTag].ContainsKey(curword))
                            emissionProb = Math.Log10(((HmmInput.EmissionBlock[curTag])[curword]) == 1.0 ? 0.999 : (HmmInput.EmissionBlock[curTag])[curword]);
                        else if (!HmmInput.symbolList.ContainsKey(curword) && HmmInput.EmissionBlock[curTag].ContainsKey(unknown))
                            emissionProb = Math.Log10((HmmInput.EmissionBlock[curTag])[unknown]);
                        else
                            emissionProb = 0;

                        if ( transProb == 0 || emissionProb == 0)
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
            int wordIndex = words.Length-1;
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
            Console.WriteLine(Vertibi[maxStateRowIndex,wordIndex]);
            List<String> sb = new List<String>();
            sb.Add(Convert.ToString(Vertibi[maxStateRowIndex, wordIndex]));
            
            while (wordIndex >= 0 && maxStateRowIndex > 0)
            {
                sb.Add(HmmInput.revStatesList[maxStateRowIndex]);
                maxStateRowIndex = BackTrack[maxStateRowIndex, wordIndex--];
            }
            sb.Add(" => ");
            sb.Add(Input);
            sb.Reverse();
            var result = string.Join(" ", sb);
            Console.WriteLine(result);
            Console.WriteLine("finished building the vertibi table");
            Console.ReadLine();
        }
    }
}
