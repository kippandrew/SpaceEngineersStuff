using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.ModAPI;
using VRage.Utils;
using SpaceEngineers.Game;
using SpaceEngineers.Game.ModAPI;
using VRage.Game.ModAPI;
using Sandbox.Game.Weapons;

namespace StaticGridWeaponRange
{
    public class Logger
    {
        public static void Error(string message)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange]: ERROR: {message}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[StaticGridWeaponRange] [ERROR: {message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Error(object caller, Exception e)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] ERROR {caller.GetType().FullName}: {e.ToString()}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[StaticGridWeaponRange] [ERROR: {caller.GetType().FullName}: {e.Message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Error(object caller, string message)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] ERROR {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[StaticGridWeaponRange] [ERROR: {caller.GetType().FullName}: {message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Info(string message)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] INFO: {message}");
            MyLog.Default.Flush();
        }

        public static void Info(object caller, string message)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] INFO: {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();
        }

        public static void Notice(string message, bool notify = false, int notifyTime = 5000)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] INFO: {message}");
            MyLog.Default.Flush();

            if (notify)
                MyAPIGateway.Utilities?.ShowNotification($"[StaticGridWeaponRange] [{message}]", notifyTime, MyFontEnum.Green);
        }

        public static void Notice(object caller, string message, bool notify = false, int notifyTime = 5000)
        {
            MyLog.Default.WriteLineAndConsole($"[StaticGridWeaponRange] INFO: {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();

            if (notify)
                MyAPIGateway.Utilities?.ShowNotification($"[StaticGridWeaponRange] [{caller.GetType().FullName}: {message}]", notifyTime, MyFontEnum.Green);
        }
    }

    public abstract class StaticGridTurret : MyGameLogicComponent
    {

        private IMyTerminalBlock mTerminal;
        private IMyCubeBlock mBlock;

        public override void Init(MyObjectBuilder_EntityBase ob)
        {
            try
            {
                mBlock = Entity as IMyCubeBlock;
                mTerminal = Entity as IMyTerminalBlock;
                NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
                base.Init(ob);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing StaticGridTurret", ex);
            }
        }

        public override void UpdateBeforeSimulation100()
        {
            try
            {
                if (mBlock.CubeGrid.Physics != null)
                {
                    if (!mBlock.CubeGrid.IsStatic)
                    {
                        // Set to dynamic targeting range
                        mTerminal.SetValueFloat("Range", GetDynamicTargetingRange());
                        UpdateControls();
                    }
                    else
                    {
                        // Set to static targeting range 
                        mTerminal.SetValueFloat("Range", GetStaticTargetingRange());
                        UpdateControls();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in StaticGridWeaponRangeGameLogicComponent", ex);
            }
        }

        abstract protected void UpdateControls();

        abstract protected float GetDynamicTargetingRange();

        abstract protected float GetStaticTargetingRange();

        protected float GetWeaponRange(IMyEntity e)
        {
            var range = 0.0f;
            var weapon = (IMyGunObject<MyGunBase>)e;
            if (weapon != null)
            {
                range = weapon.GunBase.CurrentAmmoDefinition.MaxTrajectory;
            }
            return range;
        }
    }

    // Large Grid Missile Turret
    // Large Grid Artillery Turret
    // Large Grid Assault Turret
    // Small Grid Missile Turret
    // Small Grid Assault Turret
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeMissileTurret), false, new string[] {})]
    public class StaticGridLargeMissileTurret : StaticGridTurret
    {
        private const float minimumRange = 800f;

        protected override float GetDynamicTargetingRange()
        {
            return minimumRange;
        }

        protected override float GetStaticTargetingRange()
        {
            return Math.Max(GetWeaponRange(Entity), minimumRange);
        }
        
        protected override void UpdateControls()
        {
            var tempList = new List<IMyTerminalControl>();
            MyAPIGateway.TerminalControls.GetControls<IMyLargeTurretBase>(out tempList);
            foreach (var control in tempList)
            {   
                if (control.Id == "Range")
                {
                    control.Enabled = mTurret => false;
                    control.RedrawControl();
                }
            }
        }

    }

    // Large Grid Turret Control Block
    // Small Grid Turret Control Block
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_TurretControlBlock), false, new string[] {})]
    public class StaticGridTurretControlBlock : StaticGridTurret
    {
        private const float minimumRange = 800f;

        private IMyTurretControlBlock mController;

        public override void Init(MyObjectBuilder_EntityBase ob)
        {
            try
            {
                mController = Entity as IMyTurretControlBlock;
                base.Init(ob);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing StaticGridTurretController", ex);
            }
        }
        
        protected override void UpdateControls()
        {
            var tempList = new List<IMyTerminalControl>();
            MyAPIGateway.TerminalControls.GetControls<IMyTurretControlBlock>(out tempList);
            foreach (var control in tempList)
            {   
                Logger.Info($"{control.Id}");
                if (control.Id == "Range")
                {
                    control.Enabled = mTurret => false;
                    control.RedrawControl();
                }
            }
        }

        protected override float GetDynamicTargetingRange()
        {
            return minimumRange;
        }

        protected override float GetStaticTargetingRange()
        {
            float maxRange = 0;

            var tools = new List<Sandbox.ModAPI.Ingame.IMyFunctionalBlock>();
            mController.GetTools(tools);

            foreach (var tool in tools)
            {
                var gunRange = GetWeaponRange((IMyEntity)tool);
                if (gunRange > maxRange)
                {
                    maxRange = gunRange;
                }
            }

            return Math.Max(maxRange, minimumRange);
        }
    }

}