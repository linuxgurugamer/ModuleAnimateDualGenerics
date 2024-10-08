﻿using UnityEngine;
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
        public string activateAnim_1_Name = "";
        [KSPField]
        public string activateAnim_2_Name = "";

        [KSPField]
        public string stopActionName = "Turn Off <<1>>";
        [KSPField]
        public string stopAnim_1_Name = "";
        [KSPField]
        public string stopAnim_2_Name = "";

        [KSPField]
        public string toggleActionName = "Toggle <<1>>";

        [KSPField]
        public string moduleType = "Beacon";

        [KSPField]
        public float animSpeed = 1f;

        [KSPField]
        public bool repeating = false;
        [KSPField]
        public bool anim1Repeating = false;
        [KSPField]
        public bool anim2Repeating = false;

        [KSPField]
        public KSPActionGroup defaultActionGroup;
        [KSPField(isPersistant = true)]
        public bool beaconOn = false;

        [KSPField(isPersistant = true)]
        public bool anim_1_On = false;

        [KSPField(isPersistant = true)]
        public bool anim_2_On = false;

        [KSPField(isPersistant = true)]
        public float anim1Time;

        [KSPField(isPersistant = true)]
        public float anim2Time;

        #endregion

        Animation DeployAnimation, ActiveAnimation;

        void StartAnim_1(bool dual = false)
        {
            anim_1_On = true;
            Events["ActivateAnim_1"].active = false;
            if (stopAnim_1_Name != "")
                Events["StopAnim_1"].active = true;

            if (DeployAnimation != null)
            {
                part.Effect(anim_1_name, 1f);
                DeployAnimation[anim_1_name].speed = 1f;
                DeployAnimation[anim_1_name].weight = 1f;
                DeployAnimation[anim_1_name].time = anim1Time;
                DeployAnimation.Play(anim_1_name);
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void ActivateAnim_1()
        {
            StartAnim_1();
        }


        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        void StopAnim_1()
        {
            anim_1_On = false;

            Events["StopAnim_1"].active = false;
            if (activateAnim_1_Name != string.Empty)
                Events["ActivateAnim_1"].active = true;

            if (DeployAnimation != null)
            {
             anim1Time = DeployAnimation[anim_1_name].time;
               if (repeating || anim1Repeating)
                {
                    DeployAnimation[anim_1_name].speed = 0f;
                    DeployAnimation.Stop(anim_1_name);
                }
                else
                {

                    DeployAnimation[anim_1_name].speed = -animSpeed;
                    DeployAnimation[anim_1_name].normalizedTime = 1f;
                    DeployAnimation[anim_1_name].time = DeployAnimation[anim_1_name].length;

                    DeployAnimation.Play(anim_1_name);
                }
            }

        }


        void StartAnim_2(bool dual = false)
        {
            anim_2_On = false;

            Events["ActivateAnim_2"].active = false;
            if (stopAnim_2_Name != "")
                Events["StopAnim_2"].active = true;


            if (ActiveAnimation != null)
            {
                part.Effect(anim_2_name, 1f);
                ActiveAnimation[anim_2_name].time = anim2Time;
                ActiveAnimation[anim_2_name].speed = animSpeed;
                if (repeating || anim2Repeating)
                {
                    ActiveAnimation[anim_2_name].wrapMode = WrapMode.Loop;
                }
                else
                {
                    ActiveAnimation[anim_2_name].wrapMode = WrapMode.Once;
                    StartCoroutine("SlowUpdate");
                }

                ActiveAnimation.Play(anim_2_name);
            }

        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        void StopAnim_2()
        {

            Events["StopAnim_2"].active = false;
            if (activateAnim_2_Name != string.Empty)
                Events["ActivateAnim_2"].active = true;

            if (ActiveAnimation != null)
            {
                anim2Time = ActiveAnimation[anim_2_name].time;

                if (!repeating && !anim2Repeating)
                {
                    ActiveAnimation[anim_2_name].speed = -animSpeed;
                    ActiveAnimation.Play(anim_2_name);

                }
                else
                    ActiveAnimation.Stop(anim_2_name);
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void ActivateAnim_2()
        {
            StartAnim_2();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void Deploy()
        {
            beaconOn = true;
            Events["Deploy"].active = false;
            Events["Stop"].active = true;
            StartAnim_1(true);

            StartAnim_2(true);

        }

        [KSPEvent(guiActive = true, guiActiveEditor = true)]
        public void Stop()
        {
            beaconOn = false;

            Events["Deploy"].active = true;
            Events["Stop"].active = false;

            StopAnim_1();
            StopAnim_2();

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
            if (beaconOn || anim_1_On || anim_2_On)
                Stop();
            else
                Deploy();
        }

        [KSPAction("Toggle-Action", KSPActionGroup.REPLACEWITHDEFAULT)]
        public void ToggleAction(KSPActionParam param)
        {
            Toggle();
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
                Actions["ToggleAction"].guiName = Localizer.Format("actionGUIName", moduleType);

                Events["Deploy"].guiName = Localizer.Format(activateActionName, moduleType);
                Events["Stop"].guiName = Localizer.Format(stopActionName, moduleType);

                if (activateAnim_1_Name != "")
                    Events["ActivateAnim_1"].guiName = Localizer.Format(activateAnim_1_Name, moduleType);
                else
                    Events["ActivateAnim_1"].active = false;

                if (activateAnim_2_Name != "")
                    Events["ActivateAnim_2"].guiName = Localizer.Format(activateAnim_2_Name, moduleType);
                else
                    Events["ActivateAnim_2"].active = false;

                if (stopAnim_1_Name != "")
                    Events["StopAnim_1"].guiName = Localizer.Format(stopAnim_1_Name, moduleType);
                else
                    Events["StopAnim_1"].active = false;

                if (stopAnim_2_Name != "")
                    Events["StopAnim_2"].guiName = Localizer.Format(stopAnim_2_Name, moduleType);
                else
                    Events["StopAnim_2"].active = false;



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
                if ((beaconOn || anim_1_On || anim_2_On) && HighLogic.LoadedSceneIsFlight)
                {
                    if (anim_1_On && !anim_2_On)
                        StartAnim_1();
                    if (!anim_1_On && anim_2_On)
                        StartAnim_2();
                    if ((!anim_1_On && !anim_2_On) || (anim_1_On && anim_2_On))
                        Deploy();
                }
                // else
                {
                    Events["Deploy"].active = true;
                    Events["Stop"].active = false;

                    if (activateAnim_1_Name != string.Empty)
                        Events["ActivateAnim_1"].active = true;
                    if (activateAnim_2_Name != string.Empty)
                        Events["ActivateAnim_2"].active = true;

                    Events["StopAnim_1"].active = false;
                    Events["StopAnim_2"].active = false;
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