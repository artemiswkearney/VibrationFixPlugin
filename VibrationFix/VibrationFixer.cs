using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ReflectionUtilEnhanced;
using UnityEngine.XR;

namespace VibrationFix
{
    public class VibrationFixer : MonoBehaviour
    {
        public ObstacleSaberSparkleEffectManager effectManager;
        public HapticFeedbackController haptics;
        public Saber left;
        public void Update()
        {
            // Look for the effect manager every tick until we find it
            if (effectManager == null)
            {
                effectManager = UnityEngine.Object.FindObjectOfType<ObstacleSaberSparkleEffectManager>();
                if (effectManager != null)
                {
                    foundEffectManager();
                }
            }
        }

        public void foundEffectManager()
        {
            effectManager.sparkleEffectDidStartEvent += onSparkleEffectStart;
            effectManager.sparkleEffectDidEndEvent += onSparkleEffectEnd;
            haptics = effectManager.getPrivateField<HapticFeedbackController>("_hapticFeedbackController");
            left = effectManager.getPrivateField<Saber[]>("_sabers")[0];
        }

        public void onSparkleEffectStart(Saber.SaberType type) { }

        public void onSparkleEffectEnd(Saber.SaberType type)
        {
            if (haptics != null)
            {
                // Robust against left being either A or B. Not robust against left and right being the same type.
                // This is because the effect manager's exposed events only tell us what type of saber stopped vibrating,
                // not whether it was left or right.
                if (type == left.saberType)
                {
                    haptics.UpdateStartCount(XRNode.LeftHand,
                        haptics.getPrivateField<int>("_leftControllerStartRumbleCount") * -1);
                }
                else
                {
                    haptics.UpdateStartCount(XRNode.RightHand,
                        haptics.getPrivateField<int>("_rightControllerStartRumbleCount") * -1);
                }
            }
        }

        public void OnDisable()
        {
            if (effectManager != null)
            {
                effectManager.sparkleEffectDidStartEvent -= onSparkleEffectStart;
                effectManager.sparkleEffectDidEndEvent -= onSparkleEffectEnd;
                effectManager = null;
                haptics = null;
            }
        }
    }
}