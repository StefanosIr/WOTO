using System.Collections.Generic;
using UnityEditor;

namespace UnityStandardAssets.CrossPlatformInput.Inspector
{
    [InitializeOnLoad]
    public static class CrossPlatformInitialize
    {
        private static readonly BuildTargetGroup[] BuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.WSA
        };

        private static readonly BuildTargetGroup[] MobileBuildTargetGroups =
        {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS,
            BuildTargetGroup.WSA
        };

        static CrossPlatformInitialize()
        {
            List<string> defines = GetDefinesList(BuildTargetGroups[0]);
            if (!defines.Contains("CROSS_PLATFORM_INPUT"))
            {
                SetEnabled("CROSS_PLATFORM_INPUT", true, false);
                SetEnabled("MOBILE_INPUT", true, true);
            }
        }

        [MenuItem("Mobile Input/Enable")]
        private static void Enable()
        {
            SetEnabled("MOBILE_INPUT", true, true);
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.WSAPlayer:
                    EditorUtility.DisplayDialog(
                        "Mobile Input",
                        "You have enabled Mobile Input. Use a compatible mobile target to test the on-screen controls.",
                        "OK");
                    break;
                default:
                    EditorUtility.DisplayDialog(
                        "Mobile Input",
                        "You have enabled Mobile Input, but the active build target is not mobile. The mobile rigs will not be active until you switch targets.",
                        "OK");
                    break;
            }
        }

        [MenuItem("Mobile Input/Enable", true)]
        private static bool EnableValidate()
        {
            List<string> defines = GetDefinesList(MobileBuildTargetGroups[0]);
            return !defines.Contains("MOBILE_INPUT");
        }

        [MenuItem("Mobile Input/Disable")]
        private static void Disable()
        {
            SetEnabled("MOBILE_INPUT", false, true);
            EditorUtility.DisplayDialog(
                "Mobile Input",
                "You have disabled Mobile Input. Cross Platform Input will use standalone controls.",
                "OK");
        }

        [MenuItem("Mobile Input/Disable", true)]
        private static bool DisableValidate()
        {
            List<string> defines = GetDefinesList(MobileBuildTargetGroups[0]);
            return defines.Contains("MOBILE_INPUT");
        }

        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            foreach (BuildTargetGroup group in mobile ? MobileBuildTargetGroups : BuildTargetGroups)
            {
                List<string> defines = GetDefinesList(group);
                if (enable)
                {
                    if (defines.Contains(defineName))
                    {
                        continue;
                    }

                    defines.Add(defineName);
                }
                else
                {
                    while (defines.Contains(defineName))
                    {
                        defines.Remove(defineName);
                    }
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
            }
        }

        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            string raw = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new List<string>();
            }

            return new List<string>(raw.Split(';'));
        }
    }
}
