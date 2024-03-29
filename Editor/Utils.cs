using System;
using System.Reflection;

namespace Loorix.SpineOdinExtensions
{
    public static class Utils
    {
        public static void SetGUIDrawerForType(Type type, Type drawer)
        {
// Doesn't work in 2023.3+ as the code was removed in unity, but it works without this hack anyway
#if !UNITY_2023_3_OR_NEWER
            var ScriptAttributeUtility = Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor");
            var DrawerKeySet = ScriptAttributeUtility.GetNestedType("DrawerKeySet", BindingFlags.NonPublic);

            var s_DrawerTypeForTypeFieldInfo = ScriptAttributeUtility.GetField("s_DrawerTypeForType", BindingFlags.NonPublic | BindingFlags.Static);


            if (s_DrawerTypeForTypeFieldInfo.GetValue(null) == null) {
                // Delay until drawers are initialized by unity
                UnityEditor.EditorApplication.delayCall += () => {
                    SetGUIDrawerForType(type, drawer);
                };
                return;
            }

            var newDrawer = Activator.CreateInstance(DrawerKeySet);
            DrawerKeySet.GetField("type").SetValue(newDrawer, type);
            DrawerKeySet.GetField("drawer").SetValue(newDrawer, drawer);

            s_DrawerTypeForTypeFieldInfo.FieldType.GetMethod("set_Item").Invoke(s_DrawerTypeForTypeFieldInfo.GetValue(null), new object[] {
                type,
                newDrawer
            });
#endif
        }
    }
}
