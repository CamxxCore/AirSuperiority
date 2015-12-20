using System;
using GTA.Native;
using AirSuperiority.Script.GameManagement;
using GTA;

namespace AirSuperiority.Script.UIManagement
{
    public delegate void UIRankBarRankEvent(UIRankBar sender, UIRankBarEventArgs e);

    public class UIRankBar
    {
        private int duration;
        private int animSpeed;
        private int newXP;
        private int colour;
        private int currentRank;
        private int currentRankXP;
      //  private int nextRank;
        private bool rankOverflow;
        private int rankOverflowTick;

        public event UIRankBarRankEvent RankedUp;

        public UIRankBar()
        { }

        /// <summary>
        /// Loads the rank bar to be used next function call.
        /// </summary>
        protected void LoadRankBar()
        {
            Function.Call((Hash)0x9304881D6F6537EA, 19); //REQUEST_HUD_SCALEFORM
            while (!Function.Call<bool>(Hash._HAS_HUD_SCALEFORM_LOADED, 19))
            {
                GTA.Script.Wait(0);
            }
        }

        /// <summary>
        /// Display the rank bar with the specified arguments.
        /// </summary>
        /// <param name="currentRank"></param>
        /// <param name="currentXP">The target XP for the next rank</param>
        /// <param name="nextRank"></param>
        /// <param name="nextXP"></param>
        /// <param name="colour"></param>
        /// <param name="duration"></param>
        /// <param name="animationSpeed"></param>
        public void ShowRankBar(int currentRank, int currentXP, int newXP, int colour = 116, int duration = 500, int animationSpeed = 1000)
        {
            LoadRankBar();
            ResetBarText();
            SetColour(colour);
            SetDuration(duration);
            SetAnimationSpeed(animationSpeed);

            var rankLimit = RankTables.RankData[currentRank + 2];
            SetRankScores(RankTables.RankData[currentRank], rankLimit, currentXP, currentXP + newXP > rankLimit ? rankLimit - 1 : currentXP + newXP, currentRank, 100, currentRank + 1);
            Show();

            if (currentXP + newXP >= rankLimit)
            {
                this.currentRank = currentRank + 1; //rank to display on the left side
                this.currentRankXP = rankLimit; //xp floor for the next rank
                this.newXP = newXP; //xp floor for the next rank plus new xp to get the remainder
                this.rankOverflowTick = (Game.GameTime + duration) + 1000;
                this.duration = duration;
                this.animSpeed = animationSpeed;
                this.colour = colour;
                this.rankOverflow = true;
            }
        }

        protected virtual void OnRankedUp(UIRankBarEventArgs e)
        {
            RankedUp?.Invoke(this, e);
        }

        /// <summary>
        /// Shows the rank bar.
        /// </summary>
        protected void Show()
        {
            CallFunction("SHOW");
        }

        /// <summary>
        /// Sets the hud colour for the rank bar.
        /// </summary>
        /// <param name="colour"></param>
        protected void SetColour(int colour)
        {
            CallFunction("SET_COLOUR", colour);
        }

        /// <summary>
        /// Sets the duration the rank bar is on screen.
        /// </summary>
        /// <param name="duration"></param>
        protected void SetDuration(int duration)
        {
            CallFunction("OVERRIDE_ONSCREEN_DURATION", duration);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void SetAnimationSpeed(int speed)
        {
            CallFunction("OVERRIDE_ANIMATION_SPEED", speed);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void FadeBarOut(bool remove = false)
        {
            CallFunction("FADE_BAR_OUT", remove);
        }

        /// <summary>
        /// Sets the speed of the rank up animation.
        /// </summary>
        protected void ResetBarText()
        {
            CallFunction("RESET_BAR_TEXT");
        }

        /// <summary>
        /// Set the score values for the rank bar.
        /// </summary>
        /// <param name="currentRankLimit"></param>
        /// <param name="nextRankLimit"></param>
        /// <param name="playerPreviousXP"></param>
        /// <param name="playerCurrentXP"></param>
        /// <param name="rank"></param>
        /// <param name="alpha"></param>
        /// <param name="nextRank"></param>
        protected void SetRankScores(int currentRankLimit, int nextRankLimit, int playerPreviousXP, int playerCurrentXP, int rank, int alpha, int nextRank)
        {
            CallFunction("SET_RANK_SCORES", currentRankLimit, nextRankLimit, playerPreviousXP, playerCurrentXP, rank, alpha, nextRank);
        }

        protected void CallFunction(string functionName, params object[] args)
        {
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_FROM_HUD_COMPONENT, 19, functionName);

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    Type type = arg.GetType();

                    if (type == typeof(bool))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL, (bool)arg);
                    else if (type == typeof(float))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT, (float)arg);
                    else if (type == typeof(int))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)arg);
                    else if (type == typeof(string))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL, (string)arg);
                }
            }

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
        }

        public void Update()
        {
            if (rankOverflow && Game.GameTime > rankOverflowTick)
            {
                rankOverflow = false;

                OnRankedUp(new UIRankBarEventArgs(currentRank, currentRank + 1));
                ResetBarText();
                CallFunction("STAY_ON_SCREEN");
                ShowRankBar(currentRank, currentRankXP, newXP, colour, duration, animSpeed);
                SoundManager.PlayExternalSound(Properties.Resources.rank_achieved);
                SoundManager.Step();
            }
        }
    }
}
