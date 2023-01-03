using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Utilities
{
    public static class StringUtils
    {
        public static bool TryIncreaseLastNumber(string inputString, out string number)
        {
            var reverseNumberStr = "";
            var digitIndex = 0;
            for(var i = inputString.Length - 1; i >= 0 ; i--)
            {
                if (char.IsDigit(inputString[i]))
                {
                    reverseNumberStr += inputString[i];
                    continue;
                }
                if (string.IsNullOrEmpty(reverseNumberStr))
                {
                    number = inputString;
                    return false; 
                }
                digitIndex = i + 1;
                break;
            }

            var numberString = string.Concat(reverseNumberStr.Reverse());


            if(int.TryParse(numberString, out var intNumber))
            {
                number= inputString.Substring(0,digitIndex) + (++intNumber).ToString();
                return true;
            }
            number = inputString;
            return false;
        }
    }
}
