using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

// Made by Daniel Cumbor in 2024

// todo: Possibly redundant, only really used to determine type of fade, could be achieved automatically?
public enum DCAnimatorComponentType
{
    Sprite, UI, Text
}

public enum DCAnimationType
{
    Fade, Scale, Move
}

public enum DCAnimationMode
{
    OnEntry, OnExit, Manual
}

public enum DCDelayType
{
    None, Start, End
}

public class DCAnimator : MonoBehaviour
{
    [ListDrawerSettings(Draggable = true,
        HideAddButton = false,
        HideRemoveButton = false,
        AlwaysExpanded = true)]
    public List<DCAnimation> animations;

    #region PrivateVariables

    private bool _isUserInterface;
    private bool _hasReferences;
    private List<DCAnimation> _activeAnimations;

    #endregion

    #region PrivateComponentReferences

    private RectTransform _rectTransform;
    private Image _uiImage;
    private TMP_Text _textLabel;
    private Sprite _sprite;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        _activeAnimations = new List<DCAnimation>();
        FindAndSetReferences();

        foreach (var dcAnimation in animations.Where(dcAnimation => dcAnimation.applyStartValuesImmediately))
        {
            dcAnimation.SetStartValue();
        }
    }

    private void FindAndSetReferences()
    {
        foreach (var dcAnimation in animations)
        {
            switch (dcAnimation.type)
            {
                case DCAnimationType.Fade:
                    var text = GetComponent<TMP_Text>();
                    var sprite = GetComponent<SpriteRenderer>();
                    var image = GetComponent<Image>();
                    if (text) dcAnimation.SetReference(text);
                    if (sprite) dcAnimation.SetReference(sprite);
                    if (image) dcAnimation.SetReference(image);
                    break;
                case DCAnimationType.Scale: dcAnimation.SetReference(transform); break;
                case DCAnimationType.Move:
                    var transformNeeded = dcAnimation.componentType == DCAnimatorComponentType.UI
                        ? GetComponent<RectTransform>()
                        : GetComponent<Transform>();
                    dcAnimation.SetReference(transformNeeded);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"The reference for {dcAnimation.animationName} could not be found!");
            }
        }
    }

    private void Update()
    {
        if (_activeAnimations.Count <= 0) return;
            
        foreach (var dcAnimation in _activeAnimations)
        {
            dcAnimation.Update();
        }
    }

    #endregion

    #region PublicMethods

    public void StartAnimationsByMode(DCAnimationMode mode)
    {
        _activeAnimations.Clear();
        
        if (mode == DCAnimationMode.Manual)
        {
            Debug.LogWarning("UI Animator Warning: You cannot start manual animations by mode." +
                             "They must be called directly by name. Use UIAnimator::StartAnimationByName() instead");

            return;
        }
        
        var animationsByMode = 
            animations.Where(animationData => animationData.mode == mode).ToList();

        _activeAnimations = animationsByMode;

        foreach (var animationData in _activeAnimations)
        {
            animationData.StartAnimation();
        }
    }

    public void StartAnimationByName(string animationName, Action onAnimationComplete = null)
    {
        _activeAnimations.Clear();
        
        foreach (var animationData in animations.Where(animationData => animationData.animationName == animationName))
        {
            _activeAnimations.Add(animationData);
            animationData.StartAnimation();

            return;
        }
        Debug.LogWarning($"UI Animator Warning:" +
                         $"The requested animation to play on {gameObject.name} does not exist.");
    }

    #endregion

    #region PrivateMethods
    

    #endregion
}
