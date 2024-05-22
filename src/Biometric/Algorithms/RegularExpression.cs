using Biometric.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometric.Algorithms
{
    class RegularExpression
    {
        private static string[] word = new string[26]
        {
            "(aA4)?",
            "[bB8]",
            "[cC]",
            "[dD]",
            "([eE3])?",
            "[fF]",
            "[gG6]",
            "[hH]",
            "([iI1])?",
            "[jJ]",
            "[kK]",
            "[lL]",
            "[mM(111)]",
            "[nN(11)]",
            "([oO0])?",
            "[pP]",
            "[qQ]",
            "[rR(12)]",
            "[sS5]",
            "[tT]",
            "([uU])?",
            "[vV]",
            "[wW]",
            "[xX]",
            "[yY]",
            "[zZ2]"
        };

        private string regexText;

        private string findRegex(char c)
        {
            int idx = ((int)c) % 65;
            return RegularExpression.word[idx];
        }

        public RegularExpression(string text)
        {
            text = text.ToUpper();
            regexText = @"";
            foreach (char character in text)
            {
                if (character != ' ')
                {
                    regexText += findRegex(character);
                }
                else
                {
                    regexText += "\\s";
                }
            }
        }

        public string GetRegexText()
        {
            return regexText;
        }
    }

//    class Program
//    {
//        public static void Main(string[] args)
//        {
//            // Instantiate RegularExpression and print the result
//            RegularExpression re = new("ayam");
//            Console.WriteLine(re.GetRegexText());
//        }
//    }
}