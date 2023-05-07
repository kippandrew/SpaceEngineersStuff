using System;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace UpkeepMod
{

    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class UpkeepSessionComponent : MySessionComponentBase
    {

        private bool m_isInitialized = false;

        public override void UpdateBeforeSimulation()
        {
            if (!m_isInitialized)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            try
            {
                m_isInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing UpkeepSessionComponent", ex);
            }

            Logger.Info("Initialized UpkeepSessionComponent");

        }

    }

}