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
            string path = $"{Statics.DataFolderPath}/.token_debug.txt";
#else
            string path = $"{Statics.DataFolderPath}/.token_release.txt";
#endif
            return File.ReadAllText(path);
        }

    }
}
