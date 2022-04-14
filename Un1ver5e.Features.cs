using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Features
{
    public static class ErrorResponds
    {
        private static TimeSpan storageTime = TimeSpan.FromMinutes(5);
        /// <summary>
        /// Contains latest exceptions of users.
        /// </summary>
        private static readonly Dictionary<ulong, Exception> exceptionsByAuthors = new();

        public static void AddException(ulong userId, Exception ex)
        {
            if (exceptionsByAuthors.ContainsKey(userId))
                exceptionsByAuthors[userId] = ex;
            else exceptionsByAuthors.Add(userId, ex);
        }

        public static Exception GetException(ulong userID)
        {
            return exceptionsByAuthors[userID] ?? throw new KeyNotFoundException();
        }
    }
}
