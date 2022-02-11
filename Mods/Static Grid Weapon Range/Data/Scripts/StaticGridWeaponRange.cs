using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

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

    public abstract class StaticGridWeaponRangeGameLogicComponent : MyGameLogicComponent
    {

        protected IMyTerminalBlock mTerminal;
        protected IMyLargeTurretBase mTurret;
        protected bool mStatic = false;

        abstract protected float DynamicTargetingRange {
          get;
        }
        
        abstract protected float StaticTargetingRange {
          get;
        }

        public override void Init(MyObjectBuilder_EntityBase ob)
        {
            try
            {
                mTurret = Entity as IMyLargeTurretBase;
                mTerminal = Entity as IMyTerminalBlock;
                mStatic = mTurret.CubeGrid.IsStatic;
                NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
                base.Init(ob);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing StaticGridWeaponRangeGameLogicComponent", ex);
            }
        }

        public override void UpdateBeforeSimulation10()
        {
            try
            {
                if (mTurret.CubeGrid.Physics != null)
                {
                    if (!mTurret.CubeGrid.IsStatic)
                    {
                        // Set to dynamic targeting range
                        mTerminal.SetValueFloat("Range", DynamicTargetingRange);
                        UpdateControls();
                    }
                    else
                    {
                        // Set to static targeting range 
                        mTerminal.SetValueFloat("Range", StaticTargetingRange);
                        UpdateControls();
                    }

                    mStatic = mTurret.CubeGrid.IsStatic;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in StaticGridWeaponRangeGameLogicComponent", ex);
            }
        }

        protected void UpdateControls()
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

    // Large Grid Assault Turret
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeMissileTurret), false, new string[] { "LargeBlockMediumCalibreTurret" })]
    public class StaticGridMediumCalibreTurret : StaticGridWeaponRangeGameLogicComponent
    {
        protected override float DynamicTargetingRange => 800;
        protected override float StaticTargetingRange => 1400;
    }

    // Large Grid Artillery Turret
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LargeMissileTurret), false, new string[] { "LargeCalibreTurret" })]
    public class StaticGridLargeCalibreTurret : StaticGridWeaponRangeGameLogicComponent
    {
        protected override float DynamicTargetingRange => 800;
        protected override float StaticTargetingRange => 200;
    }

}