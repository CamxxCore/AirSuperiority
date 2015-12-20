using System;

namespace AirSuperiority.Script.UIManagement
{
    /// <summary>
    /// Event args for UIRankBar rank up events
    /// </summary>
    public class UIRankBarEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize the class
        /// </summary>
        public UIRankBarEventArgs(int previousRank, int newRank)
        {
            PreviousRank = previousRank;
            NewRank = newRank;
        }

        /// <summary>
        /// The previous rank
        /// </summary>
        public int PreviousRank { get; private set; }

        /// <summary>
        /// The rank that was reached
        /// </summary>
        public int NewRank { get; private set; }
    }
}
