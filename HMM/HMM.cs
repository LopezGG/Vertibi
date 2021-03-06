﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMM
{
    class HMM
    {
        public Dictionary<String, double> initBlock;
        public Dictionary<String, Dictionary<String, double>> TransitionBlock;
        public Dictionary<String, Dictionary<String, double>> LogTransitionBlock;
        public Dictionary<String, Dictionary<String, double>> EmissionBlock;
        public Dictionary<String, Dictionary<String, double>> logEmissionBlock;
        public Dictionary<String, int> symbolList ;
        public Dictionary<String, int> statesList ;
        public List<String> revStatesList;
        public int symbolCount;
        public int stateCount;

        public HMM()
        {
            initBlock = new Dictionary<String, double>();
            TransitionBlock = new Dictionary<string, Dictionary<string, double>>();
            LogTransitionBlock = new Dictionary<string, Dictionary<string, double>>(); 
            EmissionBlock = new Dictionary<string, Dictionary<string, double>>();
            logEmissionBlock = new Dictionary<string, Dictionary<string, double>>();
            symbolList = new Dictionary<string, int>();
            statesList = new Dictionary<string, int>();
            revStatesList = new List<string>();
            symbolCount = 0;
            stateCount = 0;
        }
        public void validateFillHmm(string inputpath)
        {
            string line;
            List<String> AllInputString = new List<string>();

            //read the file and store contents
            using (StreamReader SR = new StreamReader(inputpath))
            {
                while ((line = SR.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    AllInputString.Add(line);

                }
            }

            int linecount = 0;
            int state_num = 0, sym_num = 0, init_line_num = 0, trans_line_num = 0, emiss_line_num = 0;
            int initBLockCount = 0, TransmissionBlockCount = 0, EmmissionBlockCount = 0;
            string t1, t2, temp;

            double prob, logprob ;
            line = AllInputString[linecount++];
            if (line.Contains("state_num"))
            {
                temp = line.Substring(line.IndexOf("=") + 1);
                state_num = Convert.ToInt32(temp);
            }
            line = AllInputString[linecount++];

            if (line.Contains("sym_num"))
            {
                temp = line.Substring(line.IndexOf("=") + 1);
                sym_num = Convert.ToInt32(temp);
            }

            line = AllInputString[linecount++];
            if (line.Contains("init_line_num"))
            {
                temp = line.Substring(line.IndexOf("=") + 1);
                init_line_num = Convert.ToInt32(temp);
            }

            line = AllInputString[linecount++];
            if (line.Contains("trans_line_num"))
            {
                temp = line.Substring(line.IndexOf("=") + 1);
                trans_line_num = Convert.ToInt32(temp);
            }
            line = AllInputString[linecount++];
            if (line.Contains("emiss_line_num"))
            {
                temp = line.Substring(line.IndexOf("=") + 1);
                emiss_line_num = Convert.ToInt32(temp);
            }

            line = AllInputString[linecount++];
            if (!line.Contains("init"))
                throw new Exception("warning: init block missing");
            while (true)
            {
                if (linecount >= AllInputString.Count)
                    break;
                line = AllInputString[linecount++];
                if (line.Contains(@"\transition"))
                    break;
                string[] tempwords = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);

                t1 = tempwords[0];
                prob = Convert.ToDouble(tempwords[1]);
                initBLockCount++;
                if (initBlock.ContainsKey(t1))
                    Console.WriteLine("warning: init block has duplicate entries");
                else
                    initBlock.Add(t1, prob);

            }

            while (!line.Contains(@"\transition"))
            {
                if (linecount >= AllInputString.Count)
                    break;
                line = AllInputString[linecount++];
            }

            while (true)
            {
                if (linecount >= AllInputString.Count)
                    break;
                line = AllInputString[linecount++];
                if (line.Contains(@"\emission"))
                    break;
                string[] tempwords = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (tempwords.Length < 4)
                {
                    throw new Exception("warning: TransitionBlock not properly formed with 4 columns and hence reading this block is aborted");
                }
                t1 = tempwords[0];
                t2 = tempwords[1];
                prob = Convert.ToDouble(tempwords[2]);
                logprob = (Convert.ToDouble(tempwords[2]) == 1) ? Math.Log10(0.99999) : Math.Log10(Convert.ToDouble(tempwords[2])); 
                TransmissionBlockCount++;
                if (TransitionBlock.ContainsKey(t1) && TransitionBlock[t1].ContainsKey(t2))
                    Console.WriteLine("warning: TransitionBlock  block has duplicate entries");
                else if (TransitionBlock.ContainsKey(t1))
                {
                    TransitionBlock[t1].Add(t2, prob);
                    LogTransitionBlock[t1].Add(t2, logprob);
                }
                    
                else
                {
                    TransitionBlock.Add(t1, new Dictionary<string, double> { { t2, prob } });
                    LogTransitionBlock.Add(t1, new Dictionary<string, double> { { t2, logprob } });
                }
                    
                if (!statesList.ContainsKey(t1))
                {
                    revStatesList.Add(t1);
                    statesList.Add(t1, stateCount++);
                }
                    
                if (!statesList.ContainsKey(t2))
                {
                    revStatesList.Add(t2);
                    statesList.Add(t2, stateCount++);
                }
                    

            }

            while (!line.Contains(@"\emission"))
            {
                if (linecount >= AllInputString.Count)
                    break;
                line = AllInputString[linecount++];
            }

            while (true)
            {
                if (linecount >= AllInputString.Count)
                    break;
                line = AllInputString[linecount++];
                string[] tempwords = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (tempwords.Length < 3)
                {
                    throw new Exception("warning: EmissionBlock not properly formed with 4 columns and hence reading this block is aborted");
                    //break;
                }
                t1 = tempwords[0];
                string observation = tempwords[1];
                prob = Convert.ToDouble(tempwords[2]);
                logprob = (Convert.ToDouble(tempwords[2]) == 1) ? Math.Log10(0.99999) : Math.Log10(Convert.ToDouble(tempwords[2])); 
                EmmissionBlockCount++;
                if (EmissionBlock.ContainsKey(t1) && EmissionBlock[t1].ContainsKey(observation))
                    Console.WriteLine("warning: EmissionBlock  block has duplicate entries");
                else if (EmissionBlock.ContainsKey(t1))
                {
                    EmissionBlock[t1].Add(observation, prob);
                    logEmissionBlock[t1].Add(observation, logprob);
                }
                    
                else
                {
                    EmissionBlock.Add(t1, new Dictionary<string, double> { { observation, prob } });
                    logEmissionBlock.Add(t1, new Dictionary<string, double> { { observation, logprob } });
                }
                    
                //here t2 is hte observation
                if (!symbolList.ContainsKey(observation))
                {
                    symbolList.Add(observation, symbolCount++);
                }
                    

            }

            double totalProb = 0;
            foreach (var items in initBlock)
            {
                totalProb += items.Value;

            }
            if (totalProb != 1)
            {
                throw new Exception("Warning: Total Prob of init out of state  is not equal to one");
            }
            foreach (var tagset in TransitionBlock)
            {
                totalProb = 0;
                foreach (var items in tagset.Value)
                {
                    totalProb += items.Value;
                }
                if (Math.Round(totalProb, 2) != 1.00)
                {
                    throw new Exception("warning: the trans_prob_sum for state " + tagset.Key + " is " + totalProb);
                }
            }

            foreach (var tagset in EmissionBlock)
            {
                totalProb = 0;
                foreach (var items in tagset.Value)
                {
                    totalProb += items.Value;
                }
                if (Math.Round(totalProb, 2) != 1.00)
                {
                    throw new Exception("warning: the emiss_prob_sum for state " + tagset.Key + " is " + totalProb);
                }
            }
            if (state_num != statesList.Count)
                Console.WriteLine ("Actual number of states is " + statesList.Count + " but the number of states declared is " + state_num);

            if (sym_num != symbolList.Count)
                Console.WriteLine("Actual number of symbols is " + symbolList.Count + " but the number of states declared is " + sym_num);

            if (init_line_num != initBLockCount)
                Console.WriteLine("warning: different numbers of init_line_num: claimed=" + init_line_num + ", real=" + initBLockCount);

            if (trans_line_num != TransmissionBlockCount)
                Console.WriteLine("warning: different numbers of trans_line_num: claimed=" + trans_line_num + ", real=" + TransmissionBlockCount);

            if (emiss_line_num != EmmissionBlockCount)
                Console.WriteLine("warning: different numbers of trans_line_num: claimed=" + emiss_line_num + ", real=" + EmmissionBlockCount);
        }
    }
}
