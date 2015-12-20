using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Linq;

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

        public static Tuple<Vector3, float> GetClosestVehicleNode(Vector3 position)
        {
            Vector3 result;
            OutputArgument arg, arg1, arg2;
            arg = arg1 = arg2 = new OutputArgument();

            for (int index = 0; index < 28; ++index)
            {

                Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING, position.X, position.Y, position.Z, index, arg, arg1, arg2, 9, 3.0, 2.5);
                result = arg.GetResult<Vector3>();
                var heading = arg1.GetResult<float>();
                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, result.X, result.Y, result.Z, 5f, 5f, 5f, 0))
                {
                    return new Tuple<Vector3, float>(result, heading);
                }
            }

            for (int index = 0; index < 28; ++index)
            {
                Function.Call(Hash.GET_NTH_CLOSEST_VEHICLE_NODE, position.X, position.Y, position.Z, index, arg, 1, 1077936128, 0);
                result = arg.GetResult<Vector3>();
                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, result.X, result.Y, result.Z, 5f, 5f, 5f, 0))
                {
                    return new Tuple<Vector3, float>(result, 0f);
                }
            }
         
            return new Tuple<Vector3, float>(position, 0f);
        }


        /// <summary>
        /// Generate a random spawn position for AI
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetValidSpawnPos(Vector3 position)
        {
            for (int i = 0; i < 20; i++)
            {

                var pos = position.Around(20 * i);

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, pos.X, pos.Y, pos.Z, 5f, 5f, 5f, 0) &&
                    !World.GetAllVehicles().Any(v => v.Position.DistanceTo(pos) < 20f))
                {
                    return pos;
                }
            }

            return position;
        }
    }
}
