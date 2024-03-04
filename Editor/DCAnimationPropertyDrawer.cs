// using System;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace Editor
// {
//     [CustomPropertyDrawer(typeof(DCAnimation))]
//     public class DCAnimationPropertyDrawer : PropertyDrawer
//     {
//         private VisualElement _inspector;
//         private VisualElement _fadeSection;
//         private VisualElement _scaleSection;
//         private VisualElement _moveSection;
//
//         private EnumField _animationTypeField;
//         private SerializedProperty _animationType;
//         
//         public override VisualElement CreatePropertyGUI(SerializedProperty property)
//         {
//             _inspector = new VisualElement();
//             
//             // Get the inspector layout from the asset and close it to our new inspector.
//             var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
//                 "Packages/com.danielcumbor.uianimator/Editor/dcanimationlayout.uxml");
//             tree.CloneTree(_inspector);
//
//             // Get the animation type and listen for changes to this enum field.
//             _animationTypeField = _inspector.Q<EnumField>("AnimationType");
//             _animationTypeField.RegisterValueChangedCallback(OnAnimationTypeChanged);
//             
//             // Get the 3 main sections of the inspector
//             _fadeSection = _inspector.Q<VisualElement>("FadeSettingsSection");
//             _scaleSection = _inspector.Q<VisualElement>("ScaleSettingsSection");
//             _moveSection = _inspector.Q<VisualElement>("MoveSettingsSection");
//             
//             // Get the animation type property so we can check it's current value
//             _animationType = property.FindPropertyRelative("type");
//             
//             // Set the active layout based upon the animation type
//             // Note: Only works on first view of inspector.
//             SetActiveSettingsLayout(_animationType.enumNames[_animationType.enumValueIndex]);
//             
//             return _inspector;
//         }
//
//         private void SetActiveSettingsLayout(string enumIndex)
//         {
//             switch (enumIndex)
//             {
//                 case "Fade" : EnableFadeSection();  break;
//                 case "Scale": EnableScaleSection(); break;
//                 case "Move" : EnableMoveSection();  break;
//             }
//         }
//
//         private void EnableFadeSection()
//         {
//             _fadeSection.style.display = DisplayStyle.Flex;
//             _scaleSection.style.display = DisplayStyle.None;
//             _moveSection.style.display = DisplayStyle.None;
//         }
//
//         private void EnableScaleSection()
//         {
//             _fadeSection.style.display = DisplayStyle.None;
//             _scaleSection.style.display = DisplayStyle.Flex;
//             _moveSection.style.display = DisplayStyle.None;
//         }
//
//         private void EnableMoveSection()
//         {
//             _fadeSection.style.display = DisplayStyle.None;
//             _scaleSection.style.display = DisplayStyle.None;
//             _moveSection.style.display = DisplayStyle.Flex;
//         }
//
//         private void OnAnimationTypeChanged(ChangeEvent<Enum> evt)
//         {
//             //SetActiveSettingsLayout(evt.newValue.ToString());
//         }
//     }
// }