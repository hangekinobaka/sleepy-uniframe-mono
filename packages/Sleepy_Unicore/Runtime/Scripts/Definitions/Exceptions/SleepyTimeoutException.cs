namespace Sleepy
{
    /// <summary>
    /// Timeout!
    /// </summary>
    public class SleepyTimeoutException : SleepyException
    {
        public SleepyTimeoutException()
        : base("Timeout!")
        {
        }

        public SleepyTimeoutException(int seconds)
            : base($"Timeout in {seconds} seconds.")
        {
        }

        public SleepyTimeoutException(string message)
            : base(message)
        {
        }

        public SleepyTimeoutException(string message, SleepyException inner)
            : base(message, inner)
        {
        }
    }
}
