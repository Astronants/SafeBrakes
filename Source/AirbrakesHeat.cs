using UnityEngine;

namespace SafeBrakes
{
    public class AirbrakesHeat : PartModule
    {
        bool antiHeatBrake = false;

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (antiHeatBrake != vessel.ActionGroups[KSPActionGroup.Brakes])
            {
                if (part.skinTemperature / (part.maxTemp * 0.5) * 100 >= 80 && (vessel.ActionGroups[KSPActionGroup.Brakes] == true))
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    antiHeatBrake = true;
                }
                else if (part.skinTemperature / (part.maxTemp * 0.5) * 100 <= 50 && (vessel.ActionGroups[KSPActionGroup.Brakes] == false))
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    antiHeatBrake = false;
                }
            }
        }
    }
}
