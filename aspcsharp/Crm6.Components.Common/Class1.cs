using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm6.Components.Common
{
    /// <summary>
    /// Useful for testing, when you need to mock the DateTime value type.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        long UnixTimeStamp { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public long UnixTimeStamp => Convert.ToInt64((UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
    }
}
