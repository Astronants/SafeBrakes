using UnityEngine;

namespace SafeBrakes
{
    public class SafeBrakes : PartModule
    {
        bool LastActionbrakes;
        bool handBrake = false;
        bool ABSenabled = false;
        float ABStime = 0;
        float brakeTime = 0;
        bool ABSstart = false;
        bool ABSstate = false;

        [KSPField(isPersistant = true,guiActiveEditor = true, guiActive = true, guiName = "ABS rate", guiFormat = "0.00"),
            UI_FloatEdit(minValue = 0.1f, maxValue =  1, incrementSlide = 0.01f, incrementSmall = 0.05f, incrementLarge = 0.1f, unit = "s", scene = UI_Scene.All, sigFigs = 2)]
        private float ABSrate = 0.5f;

        [KSPEvent(guiName = "Turn on ABS", guiActive = true)]
        private void ToggleABS()
        {
            if (ABSenabled == false)
            {
                ABSenabled = true;
                Events["ToggleABS"].guiName = "Turn off ABS";
                ABStime = 0;
            }
            else
            {
                ABSenabled = false;
                Events["ToggleABS"].guiName = "Turn on ABS";
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vessel == FlightGlobals.ActiveVessel)
            {
                if (handBrake)
                {
                    brakeTime += Time.deltaTime;
                }

                if ((GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
                   (GameSettings.BRAKES.GetKey() && GameSettings.MODIFIER_KEY.GetKeyDown()) ||
                   (GameSettings.BRAKES.GetKeyDown() && GameSettings.MODIFIER_KEY.GetKey()))
                {
                    handBrake = !LastActionbrakes;
                    brakeTime = 0;
                }

                if (handBrake && !vessel.ActionGroups[KSPActionGroup.Brakes] && brakeTime > 0.3)
                {
                    handBrake = false;
                }

                if ((vessel.ActionGroups[KSPActionGroup.Brakes] != ABSstate) && ABSenabled && !ABSstart && vessel.horizontalSrfSpeed >= 2)
                {
                    ABSstart = true;
                }
                else if ((vessel.ActionGroups[KSPActionGroup.Brakes] == ABSstate) || vessel.horizontalSrfSpeed < 2)
                {
                    ABSstart = false;
                }

                ABS();
                ParkBrake();
            }
            LastActionbrakes = vessel.ActionGroups[KSPActionGroup.Brakes];
        }

        private void ABS()
        {
            if (ABSenabled && ABSstart)
            {
                ABStime += Time.deltaTime;
                if (ABStime >= ABSrate)
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, false);
                    ABStime = 0;
                    ABSstate = true;
                }
                else
                {
                    vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                    ABSstate = false;
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
