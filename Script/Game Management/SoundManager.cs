using System;
using GTA;
using GTA.Native;
using System.IO;
using System.Media;

namespace AirSuperiority.Script.GameManagement
{
    public class SoundManager : GTA.Script
    {
        private static string[] soundAssets = new string[] {
            "SM_W1_START_ALL",
            "SM_W2_START",
            "SM_W3_START",
            "SM_W5_START",
            "SM_W6_START",
            "SM_W7_START",
            "SM_W8_START",
            "SM_W9_START",
            "SM_W10_START"
        };     
          
        public SoundManager()
        {
            Tick += OnTick; 
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (stepTimer.Enabled && Game.GameTime > stepTimer.Waiter)
            {
                Step();
                stepTimer.Reset();
            }
        }

        private static int currentIndex;
        private static Timer stepTimer = new Timer(60000);

        public static void Step(int soundIndex = 0)
        {
            if (soundIndex > soundAssets.Length) return;
            if (soundIndex > 0)
            {
                TriggerMusicEvent(soundAssets[soundIndex]);
                currentIndex = soundIndex;
            }
            else
            {
                TriggerMusicEvent(soundAssets[currentIndex]);
                currentIndex++;
                currentIndex %= soundAssets.Length;
            }

            stepTimer.Reset();
        }

        public static void PlayExternalSound(Stream soundStream)
        {
            var extSound = new SoundPlayer(soundStream);
            extSound.Play();
        }

        private static bool TriggerMusicEvent(string soundAsset)
        {
            Function.Call(Hash.PREPARE_MUSIC_EVENT, soundAsset);
            return Function.Call<bool>(Hash.TRIGGER_MUSIC_EVENT, soundAsset);
        }
    }
}
