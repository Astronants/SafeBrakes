using UnityEngine;

namespace SafeBrakes
{
    public class SafeBrakes : PartModule
    {
        private bool LastActionBrakes, handBrake, ABSenabled, ABSstart, ABSbrakes;
        private float ABStime = 0, brakeTime = 0;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "ABS interval", guiFormat = "0.00"),
            UI_FloatRange(minValue = 0.1f, maxValue = 1.0f, stepIncrement = 0.01f)]
        private readonly float ABSrate = 0.5f;

        [KSPEvent(guiName = "Turn on ABS", guiActive = true, isPersistent = true)]
        public void Event_ToggleABS()
        {
            ToggleABS();
        }
        [KSPAction("Toggle ABS")]
        public void Action_ToggleABS(KSPActionParam param)
        {
            ToggleABS();
        }
        private  void ToggleABS()
        {
            if (ABSenabled == false)
            {
                ABSenabled = true;
                ABStime = 0;
            }
            else
            {
                ABSenabled = false;
                ABSstart = false;
                ABSbrakes = false;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vessel != FlightGlobals.ActiveVessel) { return; }
            if (ABSenabled == false)
            {
                Events["Event_ToggleABS"].guiName = "Turn on ABS";
            }
            else
            {
                Events["Event_ToggleABS"].guiName = "Turn off ABS";
            }

            if (handBrake)
            {
                brakeTime += Time.deltaTime;
            }

            if ((GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
               (GameSettings.BRAKES.GetKey() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
               (GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKey()))
            {
                handBrake = !LastActionBrakes;
                brakeTime = 0;
            }

            if (handBrake && !vessel.ActionGroups[KSPActionGroup.Brakes] && brakeTime > 0.3)
            {
                handBrake = false;
            }

            if ((vessel.ActionGroups[KSPActionGroup.Brakes] != ABSbrakes) && ABSenabled && !ABSstart && vessel.horizontalSrfSpeed >= Configs.current.abs_minSpd)
            {
                ABSstart = true;
            }
            else if ((vessel.ActionGroups[KSPActionGroup.Brakes] == ABSbrakes) || vessel.horizontalSrfSpeed < Configs.current.abs_minSpd)
            {
                ABSstart = false;
            }

            LastActionBrakes = vessel.ActionGroups[KSPActionGroup.Brakes];
            ABS();
            ParkBrake();
        }

        private void ABS()
        {
            if (!ABSenabled || !ABSstart)
            {
                Configs.ABS_active = false;
            }
            if (ABSenabled && ABSstart && (vessel.checkLanded() || vessel.checkSplashed()))
            {
                Configs.ABS_active = true;
                ABStime += Time.deltaTime;
                if (ABStime >= ABSrate)
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    ABStime = 0;
                    ABSbrakes = true;
                }
                else
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    ABSbrakes = false;
                }
            }
        }

        private void ParkBrake()
        {
            if (handBrake && !ABSstart)
            {
                vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, handBrake);
            }
        }

    }
}
