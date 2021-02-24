using System;

namespace LegacyApp.Services
{
    public interface IDateTimeProvider
    {
        public DateTime DateTimeNow { get; }
    }
}