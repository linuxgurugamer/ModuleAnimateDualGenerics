#if false

using UnityEngine;
using System.Linq;
using KSP.Localization;
using System.Collections;

namespace DualGenerics
{
    public class ModuleAnimateBeacon : PartModule
    {
        #region KSPFields
        [KSPField]
        public string deployEffectName = "";

        [KSPField]
        public string activeAnimationName = "";

        [KSPField]
        public string deployActionName = "Turn On <<1>>";

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
                part.Effect(deployEffectName, 1f);
                DeployAnimation[deployEffectName].speed = 1f;
                DeployAnimation[deployEffectName].weight = 1f;
                DeployAnimation.Play(deployEffectName);
            }

            if (ActiveAnimation != null)
            {
                part.Effect(activeAnimationName, 1f);
                ActiveAnimation[activeAnimationName].speed = animSpeed;
                if (repeating)
                    ActiveAnimation[activeAnimationName].wrapMode = WrapMode.Loop;
                else
                {
                    ActiveAnimation[activeAnimationName].wrapMode = WrapMode.Once;
                    StartCoroutine("SlowUpdate");
                }
                ActiveAnimation.Play(activeAnimationName);
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
                DeployAnimation[deployEffectName].speed = -1000f;
                DeployAnimation.Play(deployEffectName);
            }

            if (ActiveAnimation != null)
            {
                ActiveAnimation.Stop(activeAnimationName);
            }
        }

        public void OnAwake()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
            {
                var toggleAct = Actions["ToggleAction"];
                if (toggleAct.actionGroup == KSPActionGroup.REPLACEWITHDEFAULT)
                    toggleAct.actionGroup = defaultActionGroup;
                if (toggleAct.defaultActionGroup == KSPActionGroup.REPLACEWITHDEFAULT)
                    toggleAct.defaultActionGroup = defaultActionGroup;
            }
        }

        public void Start()
        {
            OnAwake();
            if (HighLogic.LoadedScene >= GameScenes.SPACECENTER)
            {

                Events["Deploy"].guiName = Localizer.Format(deployActionName, moduleType);
                Events["Stop"].guiName = Localizer.Format(stopActionName, moduleType);
                Actions["ToggleAction"].guiName = Localizer.Format("actionGUIName", moduleType);

                DeployAnimation = base.part.FindModelAnimators(deployEffectName).FirstOrDefault();
                ActiveAnimation = base.part.FindModelAnimators(activeAnimationName).FirstOrDefault();

                var m = part.FindModulesImplementing<ModuleAnimateBeacon>();
                int idx = 0;

                for (idx = 0; idx < m.Count; idx++)
                {
                    if (m[idx] == this)
                        break;
                }
                Debug.Log("ModuleAnimateBeacon, idx: " + idx);
                if (deployEffectName != string.Empty)
                {
                    DeployAnimation[deployEffectName].layer = 3 + idx * 2;
                }
                if (activeAnimationName != string.Empty)
                {
                    ActiveAnimation[activeAnimationName].layer = 4 + idx * 2;
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
                ActiveAnimation[activeAnimationName].speed = 0f;
            }
        }

        void OnUnpause()
        {
            if (beaconOn)
            {
                animationStartTime += Time.unscaledTime - pauseStartTime;
                ActiveAnimation[activeAnimationName].speed = animSpeed;
                ActiveAnimation[activeAnimationName].time = animationStartTime;
                ActiveAnimation[activeAnimationName].wrapMode = WrapMode.Loop;
                ActiveAnimation.Play(activeAnimationName);
                pauseStartTime = 0f;
            }
        }
        #endregion
    }
}

#endif