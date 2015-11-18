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

        }
    }
}
