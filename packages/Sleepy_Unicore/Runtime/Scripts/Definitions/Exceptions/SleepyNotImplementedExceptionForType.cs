namespace Sleepy
{
    /// <summary>
    /// Type is not defined
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SleepyNotImplementedExceptionForType<T> : SleepyException
    {
        public SleepyNotImplementedExceptionForType()
            : base($"No implementation for {typeof(T)}")
        {
        }
    }

}
