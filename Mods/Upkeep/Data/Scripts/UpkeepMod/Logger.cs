using System;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Utils;

namespace UpkeepMod {

    public static class Logger 
    {
        public static void Error(string message)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod]: ERROR: {message}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[UpkeepMod] [ERROR: {message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Error(object caller, Exception e)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] ERROR {caller.GetType().FullName}: {e.ToString()}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[UpkeepMod] [ERROR: {caller.GetType().FullName}: {e.Message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Error(object caller, string message)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] ERROR {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();

            if (MyAPIGateway.Session?.Player != null)
                MyAPIGateway.Utilities.ShowNotification($"[UpkeepMod] [ERROR: {caller.GetType().FullName}: {message}] | Send SpaceEngineers.Log to mod author", 10000, MyFontEnum.Red);
        }

        public static void Info(string message)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] INFO: {message}");
            MyLog.Default.Flush();
        }

        public static void Info(object caller, string message)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] INFO: {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();
        }

        public static void Notice(string message, bool notify = false, int notifyTime = 5000)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] INFO: {message}");
            MyLog.Default.Flush();

            if (notify)
                MyAPIGateway.Utilities?.ShowNotification($"[UpkeepMod] [{message}]", notifyTime, MyFontEnum.Green);
        }

        public static void Notice(object caller, string message, bool notify = false, int notifyTime = 5000)
        {
            MyLog.Default.WriteLineAndConsole($"[UpkeepMod] INFO: {caller.GetType().FullName}: {message}");
            MyLog.Default.Flush();

            if (notify)
                MyAPIGateway.Utilities?.ShowNotification($"[UpkeepMod] [{caller.GetType().FullName}: {message}]", notifyTime, MyFontEnum.Green);
        }
    }

}