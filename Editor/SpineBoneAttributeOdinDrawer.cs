
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Spine.Unity.Editor;
using UnityEngine;
using UnityEditor;
using Spine.Unity;
using Spine;
using System;
using System.Linq;

namespace Loorix.SpineOdinExtensions
{
    [CustomPropertyDrawer(typeof(SpineBone), true)]
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class SpineBoneAttributeOdinDrawer : SpineBoneDrawer
    {
        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            Utils.SetGUIDrawerForType(typeof(SpineBone), typeof(SpineBoneAttributeOdinDrawer));
        }
        public class SpineBoneAttributeOdinSelector : OdinSelector<string>
        {
            const string NoneStringConstant = "<None>";
            string _value;
            SkeletonDataAsset _asset;
            SpineBone _attribute;
            public SpineBoneAttributeOdinSelector(string value, SkeletonDataAsset asset, SpineBone attribute)
            {
                _value = value;
                _asset = asset;
                _attribute = attribute;
            }

            public override string Title { get { return _asset.name; } }

            protected override void BuildSelectionTree(OdinMenuTree tree)
            {
                tree.Config.DrawSearchToolbar = true;
                tree.Config.UseCachedExpandedStates = false;
                tree.Selection.SupportsMultiSelect = false;

                if (_attribute.includeNone) {
                    tree.Add(NoneStringConstant, "");
                }

                SkeletonData data = _asset.GetSkeletonData(true);
                if (data == null) return;

                for (int i = 0; i < data.Bones.Count; i++) {
                    var bone = data.Bones.Items[i];
                    string name = bone.Name;
                    if (name.StartsWith(_attribute.startsWith, StringComparison.Ordinal)) {
                        // jointName = "root/hip/bone" to show a hierarchial tree.
                        string jointName = name;
                        var iterator = bone;
                        while ((iterator = iterator.Parent) != null)
                            jointName = string.Format("{0}/{1}", iterator.Name, jointName);
                        tree.Add(jointName, name, SpineEditorUtilities.Icons.bone);
                    }
                }
            }
        }

        Rect _position;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _position = position;
            base.OnGUI(position, property, label);
        }

        protected override void Selector(SerializedProperty property)
        {
            var value = property.stringValue;
            var selector = new SpineBoneAttributeOdinSelector(value, skeletonDataAsset, TargetAttribute);
            selector.SetSelection(value);
            selector.ShowInPopup(_position);

            selector.SelectionConfirmed += x => {
                property.stringValue = x.FirstOrDefault();
                property.serializedObject.ApplyModifiedProperties();
            };
        }
    }
}
