using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localhost.AI.Core.Helpers
{
    public static class TextHelper
    {
        public static int CountCharInString(string input, char character)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
            return input.Count(c => c == character);
        }
    }
}
