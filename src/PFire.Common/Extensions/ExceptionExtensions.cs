using System;
using System.Text;

namespace PFire.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
