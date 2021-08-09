#region Using Namespaces

using System;

#endregion

namespace BrewingCoffee.Helpers
{
    public interface ISysTime
    {
        DateTime GetCurrentTime();
    }

    public class SysTime : ISysTime
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}