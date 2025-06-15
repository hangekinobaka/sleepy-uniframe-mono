using System.Diagnostics;

namespace Sleepy
{
    public static class Dev
    {

        #region Debug 

        /// <summary>
        /// 开发环境下输出Log。<br/>
        /// Logs a message in the development environment.
        /// </summary>
        /// <param name="message">要打印的消息。/ The message to log.</param>
        [Conditional(SleepyConsts.DEV_ENV)]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(SleepyConsts.LOG_PREPEND + message);
        }

        /// <summary>
        /// 开发环境下输出Error。<br/>
        /// Logs an error message in the development environment.
        /// </summary>
        /// <param name="message">要打印的错误消息。/ The error message to log.</param>
        [Conditional(SleepyConsts.DEV_ENV)]
        public static void Error(object message)
        {
            UnityEngine.Debug.LogError(SleepyConsts.LOG_PREPEND + message);
        }

        /// <summary>
        /// 开发环境下输出 Warning。<br/>
        /// Logs a warning message in the development environment.
        /// </summary>
        /// <param name="message">要打印的警告消息。/ The warning message to log.</param>
        [Conditional(SleepyConsts.DEV_ENV)]
        public static void Warning(object message)
        {
            UnityEngine.Debug.LogWarning(SleepyConsts.LOG_PREPEND + message);
        }

        #endregion
    }
}
