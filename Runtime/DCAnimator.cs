using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

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
    public event Action EntryAnimationsComplete;
    public event Action ExitAnimationsComplete;
    
    [ListDrawerSettings(Draggable = true,
        HideAddButton = false,
        HideRemoveButton = false,
        AlwaysExpanded = true)]
    public List<DCAnimation> animations;

    #region PrivateVariables

    private bool _isUserInterface;
    private bool _hasReferences;
    private List<DCAnimation> _activeAnimations;
    private int _activeCompletionCount;
    private int _activeAnimationCount;
    private bool _removeFlag;
    private DCAnimationMode _currentMode;

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
                    var rectTransform = GetComponent<RectTransform>();
                    if (!rectTransform)
                    {
                        var transformNorm = GetComponent<Transform>();
                        dcAnimation.SetReference(transformNorm);
                    }
                    else
                    {
                        dcAnimation.SetReference(rectTransform);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"The reference for {dcAnimation.animationName} could not be found!");
            }
        }
    }

    private void Update()
    {
        if (_activeAnimations.Count <= 0 || _removeFlag) return;
            
        foreach (var dcAnimation in _activeAnimations)
        {
            if (dcAnimation.IsFinished)
            {
                _activeAnimations.Remove(dcAnimation);
                return;
            }
            dcAnimation.Update();
        }
    }

    #endregion

    #region PublicMethods

    public void StartAnimationsByMode(DCAnimationMode mode)
    {
        _currentMode = mode;
        
        if (mode == DCAnimationMode.Manual)
        {
            Debug.LogWarning("UI Animator Warning: You cannot start manual animations by mode." +
                             "They must be called directly by name. Use UIAnimator::StartAnimationByName() instead");

            return;
        }
        
        var animationsByMode = 
            animations.Where(animationData => animationData.mode == mode).ToList();

        _activeAnimations = animationsByMode;
        _activeAnimationCount += _activeAnimations.Count;

        foreach (var animationData in _activeAnimations)
        {
            animationData.AnimationComplete += OnActiveAnimationComplete;
            animationData.StartAnimation();
        }
    }

    public void StartAnimationByName(string animationName, Action onAnimationComplete = null)
    {
        foreach (var animationData in animations.Where(animationData => animationData.animationName == animationName))
        {
            animationData.AnimationComplete += OnActiveAnimationComplete;
            _activeAnimations.Add(animationData);
            animationData.StartAnimation();

            return;
        }
        Debug.LogWarning($"UI Animator Warning:" +
                         $"The requested animation to play on {gameObject.name} does not exist.");
    }

    #endregion

    #region PrivateMethods
    
    private void OnActiveAnimationComplete(DCAnimation activeAnimation)
    {
        activeAnimation.AnimationComplete -= OnActiveAnimationComplete;
        _activeCompletionCount++;
 
        if (_activeCompletionCount < _activeAnimationCount) return;

        if (_currentMode == DCAnimationMode.OnEntry)
        {
            EntryAnimationsComplete?.Invoke();
        }
        else if (_currentMode == DCAnimationMode.OnExit)
        {
            ExitAnimationsComplete?.Invoke();
        }
        _activeCompletionCount = 0;
        _activeAnimationCount = 0;
    }

    #endregion
}
