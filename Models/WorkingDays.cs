using System;
using System.Collections;

namespace PhilipsSignageDisplaySicp.Models
{
    [Flags]
    public enum WorkingDays : byte
    {
        None = 0,
        RepeatEveryWeek = 1,
        
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Sunday = 128,

        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekends = Saturday | Sunday,
        EveryDay = Weekdays | Weekends,
    }
}