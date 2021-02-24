using System;

namespace LegacyApp.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTimeNow => DateTime.Now;
    }
}