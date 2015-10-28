using GTA;
using GTA.Native;
using GTA.Math;

namespace AirSuperiority
{
    public static class Scripts
    {
        /// <summary>
        /// Fade out screen
        /// </summary>
        /// <param name="wait">The time to sleep while fading.</param>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeOutScreen(int wait, int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, duration);
            GTA.Script.Wait(wait);
        }

        /// <summary>
        /// Fade in screen
        /// </summary>
        /// <param name="wait">The time to sleep while fading.</param>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeInScreen(int wait, int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_IN, duration);
            GTA.Script.Wait(wait);
        }

        public static void DisableHospitalRestart(bool toggle)
        {
            for (int i = 0; i <= 4; i++)
            {
                Function.Call(Hash.DISABLE_HOSPITAL_RESTART, i, toggle);
            }
        }


        /// <summary>
        /// Get random position near an entity.
        /// </summary>
        /// <param name="entity">Target entity.</param>
        /// <param name="multiplier">Distance multiplier.</param>
        /// <returns></returns>
        public static Vector3 GetRandomPositionNearEntity(Entity entity, float multiplier)
        {
            float randX, randY;
            randX = randY = 0.0f;

            var rand = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 3999) / 1000;

            if (rand == 0)
            {
                randX = -Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else if (rand == 1)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 50.0f) * multiplier;
            }
            else if (rand == 2)
            {
                randX = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }
            else
            {
                randX = -Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 50.0f, 200.0f) * multiplier;
                randY = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -50.0f, 50.0f) * multiplier;
            }

            return entity.GetOffsetInWorldCoords(new Vector3(randX, randY, 0.0f));
        }
    }
}
