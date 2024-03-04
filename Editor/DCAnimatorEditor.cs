// using System.Collections.Generic;
// using Unity.Plastic.Newtonsoft.Json.Serialization;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace Editor
// {
//     [CustomEditor(typeof(DCAnimator)), CanEditMultipleObjects]
//     public class DCAnimatorEditor : UnityEditor.Editor
//     {
//         private VisualElement _inspector;
//         private Button _createNewAnimButton;
//         private ListView _animationsList;
//         
//         public override VisualElement CreateInspectorGUI()
//         {
//             _inspector = new VisualElement();
//
//             var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
//                 "Packages/com.danielcumbor.uianimator/Editor/dcanimatorlayout.uxml");
//             tree.CloneTree(_inspector);
//
//             return _inspector;
//         }
//
//         private void OnCreateButton(MouseUpEvent evt)
//         {
//             Debug.Log("Create new animation object!");
//
//             var animation = new DCAnimation();
//         }
//
//         private Func<VisualElement> _makeItem = () => new Label();
//     }
// }