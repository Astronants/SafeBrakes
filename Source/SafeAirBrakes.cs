namespace SafeBrakes
{
    public class SafeAirBrakes : PartModule
    {
        bool antiHeatBrake = false;
        Configs settings = new Configs();

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (antiHeatBrake != vessel.ActionGroups[KSPActionGroup.Brakes])
            {
                if (part.skinTemperature / (part.maxTemp * 0.5) * 100 >= settings.Fetch<int>("SAB_HighTrigger", 80) && (vessel.ActionGroups[KSPActionGroup.Brakes] == true))
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    antiHeatBrake = true;
                }
                else if (part.skinTemperature / (part.maxTemp * 0.5) * 100 <= settings.Fetch<int>("SAB_LowTrigger", 50) && (vessel.ActionGroups[KSPActionGroup.Brakes] == false))
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    antiHeatBrake = false;
                }
            }
        }
    }
}
