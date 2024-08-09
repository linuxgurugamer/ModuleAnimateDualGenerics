using UnityEngine;
using System.Linq;
using KSP.Localization;
using System.Collections;

namespace DualGenerics
{
    public class ModuleAnimateDualGenerics : PartModule
    {
        #region KSPFields
        [KSPField]
        public string anim_1_name = "";

        [KSPField]
        public string anim_2_name = "";

        [KSPField]
        public string activateActionName = "Turn On <<1>>";

        [KSPField]
        public string stopActionName = "Turn Off <<1>>";

        [KSPField]
        public string toggleActionName = "Toggle";

        [KSPField]
        public string moduleType = "Beacon";

        [KSPField]
        public float animSpeed = 1f;

        [KSPField]
        public bool repeating = true;

        [KSPField]
        public KSPActionGroup defaultActionGroup;

        [KSPField(isPersistant = true)]
        public bool beaconOn = false;


        #endregion

        Animation DeployAnimation, ActiveAnimation;


        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void Deploy()
        {
            beaconOn = true;
            Events["Deploy"].active = false;
            Events["Stop"].active = true;
            if (DeployAnimation != null)
            {
                part.Effect(anim_1_name, 1f);
                DeployAnimation[anim_1_name].speed = 1f;
                DeployAnimation[anim_1_name].weight = 1f;
                DeployAnimation.Play(anim_1_name);
            }

            if (ActiveAnimation != null)
            {
                part.Effect(anim_2_name, 1f);
                ActiveAnimation[anim_2_name].speed = animSpeed;
                if (repeating)
                    ActiveAnimation[anim_2_name].wrapMode = WrapMode.Loop;
                else
                {
                    ActiveAnimation[anim_2_name].wrapMode = WrapMode.Once;
                    StartCoroutine("SlowUpdate");
                }
                ActiveAnimation.Play(anim_2_name);
            }
        }

        IEnumerator SlowUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (!ActiveAnimation.isPlaying && pauseStartTime == 0f)
                {
                    Stop();
                    break;
                }
            }
            yield return null;
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void Toggle()
        {
            if (beaconOn)
            {
                Stop();
            }
            else
            {
                Deploy();
            }
        }

        [KSPAction("Toggle-Action", KSPActionGroup.REPLACEWITHDEFAULT)]
        public void ToggleAction(KSPActionParam param)
        {
            Toggle();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void Stop()
        {
            beaconOn = false;
            Events["Deploy"].active = true;
            Events["Stop"].active = false;

            if (DeployAnimation != null)
            {
                DeployAnimation[anim_1_name].speed = -1000f;
                DeployAnimation.Play(anim_1_name);
            }

            if (ActiveAnimation != null)
            {
                ActiveAnimation.Stop(anim_2_name);
            }
        }

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                var toggleAct = Actions["ToggleAction"];
                if (toggleAct.actionGroup == KSPActionGroup.REPLACEWITHDEFAULT)
                    toggleAct.actionGroup = defaultActionGroup;
                if (toggleAct.defaultActionGroup == KSPActionGroup.REPLACEWITHDEFAULT)
                    toggleAct.defaultActionGroup = defaultActionGroup;

                Events["Deploy"].guiName = Localizer.Format(activateActionName, moduleType);
                Events["Stop"].guiName = Localizer.Format(stopActionName, moduleType);
                Actions["ToggleAction"].guiName = Localizer.Format("actionGUIName", moduleType);

                DeployAnimation = base.part.FindModelAnimators(anim_1_name).FirstOrDefault();
                ActiveAnimation = base.part.FindModelAnimators(anim_2_name).FirstOrDefault();

                var m = part.FindModulesImplementing<ModuleAnimateDualGenerics>();
                int idx = 0;

                for (idx = 0; idx < m.Count; idx++)
                {
                    if (m[idx] == this)
                        break;
                }

                if (anim_1_name != string.Empty)
                {
                    DeployAnimation[anim_1_name].layer = 3 + idx * 2;
                }
                if (anim_2_name != string.Empty)
                {
                    ActiveAnimation[anim_2_name].layer = 4 + idx * 2;
                }

                GameEvents.onGamePause.Add(OnPause);
                GameEvents.onGameUnpause.Add(OnUnpause);
                if (beaconOn && HighLogic.LoadedSceneIsFlight)
                {
                    Deploy();
                }
                else
                {
                    Events["Deploy"].active = true;
                    Events["Stop"].active = false;
                }
            }
        }

        public void OnDestroy()
        {
            GameEvents.onGamePause.Remove(OnPause);
            GameEvents.onGameUnpause.Remove(OnUnpause);
        }

        #region Pause
        protected float animationStartTime;
        protected float pauseStartTime = 0f;

        void OnPause()
        {
            if (beaconOn)
            {
                pauseStartTime = Time.unscaledTime;
                ActiveAnimation[anim_2_name].speed = 0f;
            }
        }

        void OnUnpause()
        {
            if (beaconOn)
            {
                animationStartTime += Time.unscaledTime - pauseStartTime;
                ActiveAnimation[anim_2_name].speed = animSpeed;
                ActiveAnimation[anim_2_name].time = animationStartTime;
                ActiveAnimation[anim_2_name].wrapMode = WrapMode.Loop;
                ActiveAnimation.Play(anim_2_name);
                pauseStartTime = 0f;
            }
        }
        #endregion
    }
}