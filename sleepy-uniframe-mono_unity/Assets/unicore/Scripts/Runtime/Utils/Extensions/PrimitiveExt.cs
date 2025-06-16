using System;

namespace Sleepy
{
    public static class PrimitiveExt
    {
        #region String

        /// <summary>
        /// 将浮点数格式化为百分比字符串。<br/>
        /// Formats a float to a percentage string.
        /// </summary>
        /// <param name="value">要格式化的浮点数，应在0到1之间。/ The float value to be formatted, should be between 0 and 1.</param>
        /// <param name="decimalPlaces">结果中小数的位数，默认为2。/ The number of decimal places in the result, default is 2.</param>
        /// <returns>格式化后的百分比字符串。/ The formatted percentage string.</returns>
        public static string ToPercentage(this float value, int decimalPlaces = 2)
        {
            // 如果数值不在0到1之间，返回错误提示
            // If the value is not between 0 and 1, return an error message
            if (value < 0 || value > 1)
            {
                // 给用户一个警告 / Provide a warning to the user
                Dev.Warning($"{value} is not a valid percentage float.");
            }

            // 确保值在0到1之间 / Ensure the value is clamped between 0 and 1
            float clampedValue = Math.Clamp(value, 0f, 1f);

            // 格式化字符串，动态设置保留的小数位数 / Format the string, dynamically setting the number of decimal places
            string format = $"0.{new string('0', decimalPlaces)}";

            // 转换成百分数并保留指定位数的小数 / Convert to a percentage and keep the specified number of decimal places
            string percentage = (clampedValue * 100).ToString(format) + "%";

            return percentage;
        }

        #endregion

    }
}
