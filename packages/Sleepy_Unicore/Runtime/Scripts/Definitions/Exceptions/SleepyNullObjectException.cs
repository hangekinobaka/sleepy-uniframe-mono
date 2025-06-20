namespace Sleepy
{
    public class SleepyNullObjectException : SleepyException
    {
        public SleepyNullObjectException(object obj)
            : base($"The {obj.GetType()} object is destroyed and is an empty \"null\" object now.")
        {
        }
    }
}
