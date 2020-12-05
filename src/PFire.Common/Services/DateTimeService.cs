using System;

namespace PFire.Common.Services
{
    public interface IDateTimeService
    {
        DateTimeOffset Now { get; }
    }

    internal class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
