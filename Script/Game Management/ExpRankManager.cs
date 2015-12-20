using AirSuperiority.Script.UIManagement;

namespace AirSuperiority.Script.GameManagement
{
    public static class ExpRankManager
    {
        public static void AddRankPoints(int value, bool showUI)
        {
            var currentXP = PlayerStats.ReadPlayerStat("exp");
            PlayerStats.WritePlayerStat("exp", currentXP + value);

            if (showUI)
            {
                var rIndex = GetRankIndex(currentXP);
                UIManager.RankBar.ShowRankBar(rIndex, currentXP, value, 116, 3000, 2000);
             }
        }

        public static void RemoveRankPoints(int value)
        {
            var prevRank = PlayerStats.ReadPlayerStat("exp");
            PlayerStats.WritePlayerStat("exp", prevRank - value);
        }

        public static int GetRankIndex(int value)
        {
            for (int i = 0; i < RankTables.RankData.Length; i+=2)
            {
                var rankFloor = RankTables.RankData[i];
                var rankCeil = RankTables.RankData[i + 1];
                if (value >= rankFloor && value < rankCeil)
                    return i;
            }
            return -1;
        }
    }
}
