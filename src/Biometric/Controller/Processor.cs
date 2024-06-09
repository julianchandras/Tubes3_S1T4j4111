using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometric.Controller
{
    class Processor
    {
        private List<string> inputPatterns;
        private string inputText;
        private int textLength;

        public void setAttributes(string inputFilePath)
        {
            inputPatterns = FingerprintReader.imgToPattern(inputFilePath);
            inputText = FingerprintReader.imgToText(inputFilePath);
            textLength = inputText.Length;
            Console.WriteLine("Text Length: " + textLength);
            Console.WriteLine("Input Patterns: " + inputPatterns.Count());
        }

        public bool bmComparator(string compImg)
        {
            string text = FingerprintReader.imgToText(compImg);
            foreach (string pattern in inputPatterns)
            {
                if (Algorithms.BM.bmMatch(text, pattern) == -1)
                {
                    return false;
                }
            }

            return true;
        }

        public bool kmpComparator(string compImg)
        {
            string text = FingerprintReader.imgToText(compImg);
            foreach (string pattern in inputPatterns)
            {
                if (Algorithms.KMP.kmpMatch(text, pattern) == -1)
                {
                    return false;
                }
            }

            return true;
        }

        public double levComparator(string compImg) 
        {
            string compText = FingerprintReader.imgToText(compImg);
            int levDistance = Algorithms.Levenshtein.levenshteinDistance(inputText, compText);
            return 1 - (double)levDistance / Math.Max(textLength, compText.Length);
        }
    }
}
