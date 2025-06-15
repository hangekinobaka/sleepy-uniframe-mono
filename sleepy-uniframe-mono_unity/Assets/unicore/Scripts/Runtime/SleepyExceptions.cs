namespace Sleepy
{
    /// <summary>
    /// Our custome exception BASE
    /// </summary>
    public class SleepyException : System.Exception
    {
        // 无参数的默认构造函数 / Default constructor without parameters
        public SleepyException()
            : base(Sleepy.PREPEND + "An exception has occurred.")
        {
        }

        // 带消息的构造函数 / Constructor with message
        public SleepyException(string message)
            : base(Sleepy.PREPEND + message) // Pass the message with prefix
        {
        }

        // 带消息和内部异常的构造函数 / Constructor with message and inner exception
        public SleepyException(string message, System.Exception innerException)
            : base(Sleepy.PREPEND + message, innerException) // Inner exception for exception chaining
        {
        }

        // 用于序列化的构造函数 / Constructor for serialization
        protected SleepyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) // Used for exception serialization (e.g., during remoting)
        {
        }

        // 其他自定义的属性和方法
        // Additional custom properties and methods 
    }

}