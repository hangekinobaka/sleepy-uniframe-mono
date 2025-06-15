namespace Sleepy
{
    public static class Sleepy
    {
        public const string PREPEND = "<color=#72D248>Sleepy: </color>";

        public static void Log(object message)
        {
            Dev.Log(PREPEND + message);
        }
        public static void Error(object message)
        {
            Dev.LogError(PREPEND + message);
        }
        public static void Warning(object message)
        {
            Dev.LogWarning(PREPEND + message);
        }
    }
}
