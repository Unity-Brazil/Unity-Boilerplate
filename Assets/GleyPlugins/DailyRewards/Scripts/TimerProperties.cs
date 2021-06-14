namespace GleyDailyRewards
{
    using System;

    /// <summary>
    /// Helping class used to deal with multiple buttons in the same time
    /// </summary>
    public class TimerProperties
    {
        public TimerButtonIDs buttonID;
        public DateTime savedTime;
        public TimeSpan timeToPass;

        public TimerProperties(TimerButtonIDs buttonID)
        {
            this.buttonID = buttonID;
        }
    }
}
