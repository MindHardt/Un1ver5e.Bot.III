namespace Un1ver5e.Bot
{
    public static class SplashReader
    {
        private static readonly string path = Statics.DataFolderPath + "/Splashes.txt";
        public static string GetSplash() => File.ReadAllLines(path).GetRandomElement();
    }
}
