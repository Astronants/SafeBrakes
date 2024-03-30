using SafeBrakes.UI;
using UnityEngine;

namespace SafeBrakes
{
    public class SafeBrakes : PartModule
    {
        private bool lastActionBrakes, toggleBrakes;
        private float brakeTime = 0;
        private bool ABSenabled, ABSstart, ABSbrakes;
        private float ABStime = 0;

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
            if (vessel == null || FlightGlobals.ActiveVessel == null) return;

            if (ABSenabled == false)
            {
                Events["Event_ToggleABS"].guiName = "Turn on ABS";
            }
            else
            {
                Events["Event_ToggleABS"].guiName = "Turn off ABS";
            }

            #region toggling brakes
            if (toggleBrakes)
            {
                brakeTime += Time.deltaTime;
            }

            if ((GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
               (GameSettings.BRAKES.GetKey() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
               (GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKey()))
            {
                toggleBrakes = !lastActionBrakes;
                brakeTime = 0;
            }

            if (toggleBrakes && !vessel.ActionGroups[KSPActionGroup.Brakes] && brakeTime > 0.3)
            {
                toggleBrakes = false;
            }
            #endregion

            #region antilock system
            if (App.Instance.presets.Selected != null)
            {
                if (ABSenabled)
                {
                    if (!ABSstart && vessel.horizontalSrfSpeed >= App.Instance.presets.Selected.abs_minSpd && vessel.ActionGroups[KSPActionGroup.Brakes] != ABSbrakes)
                    {
                        ABSstart = true;
                    }
                    else if (vessel.horizontalSrfSpeed < App.Instance.presets.Selected.abs_minSpd || vessel.ActionGroups[KSPActionGroup.Brakes] == ABSbrakes)
                    {
                        ABSstart = false;
                    }
                }
            }
            #endregion

            ABS();
            ToggleBrakes();

            lastActionBrakes = vessel.ActionGroups[KSPActionGroup.Brakes];
        }

        private void ABS()
        {
            if (ABSenabled && ABSstart)
            {
                if (vessel.LandedOrSplashed)
                {
                    UI.App.Instance.ABS_active(true);

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
            else
            {
                UI.App.Instance.ABS_active(false);
            }
        }

        private void ToggleBrakes()
        {
            if (toggleBrakes && !ABSstart)
            {
                vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, toggleBrakes);
            }
        }
    }
}
