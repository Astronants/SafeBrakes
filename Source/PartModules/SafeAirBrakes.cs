using SafeBrakes.UI;
using System.Linq;

namespace SafeBrakes
{
    public class SafeAirBrakes : PartModule
    {
        private bool SABenabled, SABbrakes, SABstart;
        private ModuleAeroSurface module;

        public override void OnAwake()
        {
            base.OnAwake();
            SABstart = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vessel == null || App.Instance.presets.Selected == null) return;

            SABenabled = App.Instance.presets.Selected.allow_sab;

            if (SABenabled && SABbrakes == vessel.ActionGroups[KSPActionGroup.Brakes])
            {
                SABenabled = false;
            }

            module = part.Modules.GetModules<ModuleAeroSurface>().First();
            float temperature = (float)part.skinTemperature / module.uncasedTemp * 100f;

            if (SABenabled)
            {
                if (temperature >= App.Instance.presets.Selected.sab_highT && SABbrakes != vessel.ActionGroups[KSPActionGroup.Brakes])
                {
                    SABstart = true;
                }
                else if (temperature < App.Instance.presets.Selected.sab_lowT)
                {
                    SABstart = false;
                }
            }

            SAB();
        }

        private void SAB()
        {
            if (SABenabled)
            {
                if (SABstart)
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    SABbrakes = true;
                    UI.App.Instance.SAB_active(true);
                }
                else
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    UI.App.Instance.SAB_active(false);
                    SABbrakes = false;
                }
            }
            else
            {
                UI.App.Instance.SAB_active(false);
            }
        }
    }
}
