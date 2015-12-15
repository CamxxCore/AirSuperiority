﻿using GTA;
using GTA.Math;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.IRFlares;
using AirSuperiority.Script.GameManagement;
using AirSuperiority.Script.UIManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class LocalFighter : ActiveFighter
    {
        private const int EngineRepairRecharge = 20000;
        private const int IRFlaresRecharge = 15000;

        private IRFlareManager irFlares;
        private InterpolatingCamera interpCam;
        private Timer boostTimer = new Timer(5000);
        /// <summary>
        /// Setup fighter for the local player.
        /// </summary>
        public void Setup()
        {

            var vehModel = new Model(VehicleHash.Lazer);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            //Create the vehicle
            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, Team.FighterSpawn));
            vehicle.Heading = Team.SpawnHeading;
           // vehicle.Position = vehicle.Position - vehicle.ForwardVector * 150;// + vehicle.UpVector * 50;

            vehicle.Vehicle.EngineRunning = true;
            vehicle.IsInvincible = true;
            vehicle.MaxHealth = 10000;
            vehicle.MaxSpeed = 115;
            ManagedVehicle = vehicle;
            ManagedVehicle.EnterWater += EnterWater;

            irFlares = new IRFlareManager();

            irFlares.SetupWithVehicle(ManagedVehicle.Vehicle);

            //Handle the ped
            var ped = new ManageablePed(Game.Player.Character);
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);
            ped.IsInvincible = true;

            ManagedPed = ped;
            ManagedPed.ExitVehicle += ExitVehicle;

            interpCam = new InterpolatingCamera(vehicle.GetOffsetInWorldCoords(new Vector3(-2f, -2f, 10f)));
            interpCam.MainCamera.PointAt(vehicle);
            interpCam.Start();

            engineFX1 = new LoopedPTFX("core", "ent_sht_extinguisher");
            engineSound = new GameSound("SPRAY", "CARWASH_SOUNDS");

            boostTimer.Start();
        }

        private void ExitVehicle(object sender, EntityChangedEventArgs e)
        {
            Scripts.FadeOutScreen(1500, 2000);
            ManagedVehicle.Delete();
            Setup();
            Scripts.FadeInScreen(500, 1000);
        }

        private void EnterWater(object sender, EntityChangedEventArgs e)
        {
            Scripts.FadeOutScreen(1500, 2000);
            ManagedVehicle.Delete();
            Setup();
            Scripts.FadeInScreen(500, 1000);
        }

        private LoopedPTFX engineFX1;
        private GameSound engineSound;
        private Timer engineFXTimer = new Timer(5000);
        private Timer engineRepairRecharge = new Timer(EngineRepairRecharge);
        private Timer irFlaresRecharge = new Timer(IRFlaresRecharge);

        public void DoFireExtinguisher()
        {
            if (engineRepairRecharge.Enabled || engineFX1.Exists) return;

            PlayEquipSound(0);

            if (!engineFX1.IsLoaded)
                engineFX1.Load();

            engineSound.Play(ManagedVehicle);
            engineFX1.Start(ManagedVehicle.Vehicle, 4f, new Vector3(0f, 1f, 0), new Vector3(89.5f, 0f, 0), (Bone) Function.Call<int>((Hash)0xFB71170B7E76ACBA, ManagedVehicle.Handle, "afterburner")); // ENTITY::GET_ENTITY_BONE_INDEX_BY_NAME           
            Function.Call((Hash)0xDCB194B85EF7B541, engineFX1.Handle, 3000.0f);
            engineFXTimer.Start();
            UIManager.SetHUDStatus(0, false);
        }

        public void DoIRFlares()
        {
            if (irFlaresRecharge.Enabled) return;

            PlayEquipSound(2);

            irFlares.Start();
            irFlaresRecharge.Start();
            UIManager.SetHUDStatus(1, false);
        }

        private void PlayEquipSound(int id)
        {
            switch (id)
            {
                case 0:
                    SoundManager.PlayExternalSound(Properties.Resources.defense_equip);
                    break;

                case 1:
                    SoundManager.PlayExternalSound(Properties.Resources.flares_equip);
                    break;

                case 2:
                    SoundManager.PlayExternalSound(Properties.Resources.flares_equip1);
                    break;

                case 3:
                    SoundManager.PlayExternalSound(Properties.Resources.flares_inactive);
                    break;
            }
        }

        bool tooFar = false;

        public override void Update()
        {
            irFlares.Update();

            if (irFlaresRecharge.Enabled)
            {
                if (Game.GameTime > irFlaresRecharge.Waiter - 10000)
                {
                    UI.Notify("Flares Recharging!");
                }

                if (Game.GameTime > irFlaresRecharge.Waiter)
                {
                    UI.Notify("Flares Available.");
                    irFlaresRecharge.Enabled = false;
                    UIManager.SetHUDStatus(1, true);
                    PlayEquipSound(1);
                }
            }

            if (engineRepairRecharge.Enabled)
            {
                if (Game.GameTime > engineRepairRecharge.Waiter)
                {
                    UI.Notify("Fire Extinguisher Available.");
                    engineRepairRecharge.Enabled = false;
                    UIManager.SetHUDStatus(0, true);
                }
            }

            else if (engineFXTimer.Enabled)
            {
                ManagedVehicle.Vehicle.Repair();

                if (Game.GameTime > engineFXTimer.Waiter)
                {
                    engineSound.Stop();
                    engineFX1.Remove();
                    engineFXTimer.Enabled = false;
                    engineRepairRecharge.Start();
                    UI.Notify("Fire Extinguisher Recharging!");
                }
            }

            if (ManagedPed.IsDead)
            {
                GTA.Script.Wait(7000);
                Setup();
            }
            
            else if (Game.IsControlJustPressed(0, Control.ScriptRB))
            {
                DoIRFlares();
            }        

            else if (Game.IsControlJustPressed(0, Control.ScriptLB))
            {
                DoFireExtinguisher();
            }

            else if (!ManagedVehicle.Vehicle.IsDriveable)
            {
                Scripts.FadeOutScreen(1500, 2000);
                ManagedVehicle.Delete();
                Setup();
                Scripts.FadeInScreen(500, 1000);
            }

            else
            {
                if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 2000f && !tooFar)
                {
                    UIManager.Append("~r~Leaving The Combat Area!");
                    tooFar = true;
                }

                else if (tooFar && ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) < 2000f)
                {
                    UIManager.Clear();
                }

                else if (tooFar && ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 2500f)
                {
                    Scripts.FadeOutScreen(1500, 2000);
                    UIManager.Clear();
                    Setup();
                    Scripts.FadeInScreen(500, 1000);
                    tooFar = false;
                }

                else
                {
                    if (interpCam != null && !Function.Call<bool>(Hash.IS_CAM_RENDERING, interpCam.MainCamera.Handle))
                    {
                        LandingGearState = LandingGearState.Closing;
                        interpCam.Destroy();
                        interpCam = null;
                    }

                    if (boostTimer.Enabled)
                    {
                        if (Game.GameTime > boostTimer.Waiter)
                        {
                            //kill the default flying music, make it more suspenseful.
                            SoundManager.Step();
                            ManagedVehicle.IsInvincible = false;
                            boostTimer.Enabled = false;
                            LandingGearState = LandingGearState.Closing;
                        }
      
                        else
                            ManagedVehicle.ApplyForce(ManagedVehicle.ForwardVector * 0.65f);
                    }
                }
            }

            base.Update();
        }

        /// <summary>
        /// Unload player related things.
        /// </summary>
        public void Unload()
        {
            ManagedPed.IsInvincible = false;
            //ManagedVehicle?.Delete();
        }
    }
}
