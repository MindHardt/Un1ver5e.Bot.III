using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    public static class TokenReader
    {
        /// <summary>
        /// Reads token accordig to a preprocessor configuration.
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
#if DEBUG
            string path = "./.token_debug.txt";
#else
            string path = "./.token_release.txt";
#endif
            return File.ReadAllText(path);
        }
        
    }
}
