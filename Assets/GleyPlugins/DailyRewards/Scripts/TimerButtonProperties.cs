namespace GleyDailyRewards
{
    /// <summary>
    /// Properties of the timer button
    /// </summary>
    [System.Serializable]
    public class TimerButtonProperties
    {
        public string buttonID = "Reward Button";
        public int hours = 24;
        public int minutes = 0;
        public int seconds = 0;
        public bool interactable = false;
        public bool availableAtStart = true;
        public string completeText = "Open";
    }
}