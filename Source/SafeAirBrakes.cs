namespace SafeBrakes
{
    public class SafeAirBrakes : PartModule
    {
        private bool antiHeatBrakes, SAB_on;

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vessel != FlightGlobals.ActiveVessel) { return; }
            if (Configs.current.sab_allow)
            {
                if (antiHeatBrakes != vessel.ActionGroups[KSPActionGroup.Brakes])
                {
                    if (part.skinTemperature / (part.maxTemp * 0.5f) * 100f >= Configs.current.sab_highT && (vessel.ActionGroups[KSPActionGroup.Brakes] == true))
                    {
                        vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                        antiHeatBrakes = true;
                        Configs.SAB_active = true;
                    }
                    else if (part.skinTemperature / (part.maxTemp * 0.5f) * 100f <= Configs.current.sab_lowT && (vessel.ActionGroups[KSPActionGroup.Brakes] == false))
                    {
                        vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                        antiHeatBrakes = false;
                        Configs.SAB_active = false;
                    }
                }
                SAB_on = true;
            }
            else
            {
                if (SAB_on)
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, !antiHeatBrakes);
                }
                Configs.SAB_active = false;
                SAB_on = false;
            }
        }
    }
}
