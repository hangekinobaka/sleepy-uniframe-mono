namespace Sleepy
{
    public static class ObjectExt
    {
        /// <summary>
        /// 判断指定对象是否已被 Unity 标记为 null 或实际上就是 null。<br/>
        /// Determines whether the specified object has been marked as null by Unity or is actually null.
        /// </summary>
        /// <param name="obj">待判断的对象。<br/>
        /// The object to be evaluated.</param>
        /// <returns>如果对象为 null 或被标记为 null，则返回 true，否则返回 false。<br/>
        /// Returns true if the object is null or marked as null, otherwise false.</returns>
        public static bool IsNullObject(this object obj)
        {
            return obj == null || obj.ToString() == "null";
        }
    }
}