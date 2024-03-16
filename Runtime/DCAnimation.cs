using System;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

// Made by Daniel Cumbor in 2024

[DeclareFoldoutGroup("coreBox", Title = "Core Settings")]
[DeclareFoldoutGroup("delayBox", Title = "Delay Settings")]
[DeclareFoldoutGroup("modeBox", Title = "Mode Settings")]

[DeclareHorizontalGroup("values")]
[DeclareHorizontalGroup("delay")]

[Serializable]
public class DCAnimation
{
    [Title("$" + nameof(animationName))]
    
    // Animation Data
    public string animationName;
    [Group("coreBox")] public DCAnimationType type;
    [Group("coreBox")] public DCAnimationMode mode;
    
    [HideIf(nameof(type), DCAnimationType.Fade)]
    [Group("coreBox")] public EasingMethods.Ease easeType;
    
    [ShowIf(nameof(type), DCAnimationType.Fade)]
    [Group("coreBox")] public DCAnimatorComponentType componentType;
    
    [Group("delayBox")] public float startDelay;
    [Group("delayBox")] public float endDelay;
    
    [Group("modeBox")] public bool applyStartValuesImmediately;
    
    [HideIf(nameof(type), DCAnimationType.Move)]
    [Group("modeBox")] public float startValue;

    [HideIf(nameof(type), DCAnimationType.Move)]
    [Group("modeBox")] public float endValue;
    
    [ShowIf(nameof(type), DCAnimationType.Move)]
    [Group("modeBox")] public Vector3 startPosition = Vector3.zero;
    
    [ShowIf(nameof(type), DCAnimationType.Move)]
    [Group("modeBox")] public Vector3 endPosition = Vector3.zero;
    
    [Group("modeBox")] public float speedValue;
    
    // Events
    public event Action<DCAnimation> AnimationStarted;
    public event Action<DCAnimation> AnimationComplete;
    public bool IsFinished { get; set; }

    // Component References
    private Transform _transform;
    private RectTransform _rectTransform;
    private Image _uiImage;
    private TMP_Text _textLabel;
    private SpriteRenderer _sprite;

    // Timing variables
    private bool _canTransition;
    private bool _isUserInterface;
    private float _currentTime;
    private DCDelayType _delayType = DCDelayType.None;
    private Timer _delayTimer = new();

    public void StartAnimation()
    {
        IsFinished = false;
        _currentTime = 0;
        SetStartValue();
        if (startDelay > 0)
        {
            _delayType = DCDelayType.Start;
            _delayTimer.OnTimerComplete += TimerComplete;
            _delayTimer.SetStartValue(startDelay);
            _delayTimer.Start();
        }
        else
        {
            _delayType = DCDelayType.None;
            AnimationStarted?.Invoke(this);
            _canTransition = true;
        }
    }

    public string DynamicTitle() => animationName;

    public void StopAnimation()
    {
        _canTransition = false;
    }
    
    public void SetReference(Component component)
    {
        switch (component)
        {
            case Image image: _uiImage = image; break;
            case TMP_Text text: _textLabel = text; break;
            case SpriteRenderer sr: _sprite = sr; break;
            case RectTransform rt: _rectTransform = rt; _isUserInterface = true; break;
            case Transform t: _transform = t; _isUserInterface = false; break;
            default: Debug.LogError($"DCAnimator: This component type for {animationName} is not supported!"); break;
        }
    }

    private void TimerComplete()
    {
        if (_delayType is DCDelayType.Start)
        {
            AnimationStarted?.Invoke(this);
            _canTransition = true;
        }
        if (_delayType is DCDelayType.End or DCDelayType.None)
        {
            AnimationComplete?.Invoke(this);
        }
        _delayTimer.OnTimerComplete -= TimerComplete;
    }

    public void Update()
    {
        if (_delayTimer.IsRunning())
        {
            _delayTimer.Update();
        }
        
        if (!_canTransition) return;
        
        _currentTime += Time.deltaTime * speedValue;
        ProcessAnimation();

        if (_currentTime >= 1)
        {
            CompleteAnimation();
        }
    }

    public void SetStartValue()
    {
        switch (type)
        {
            case DCAnimationType.Fade:
                ChangeFadeValueByType(startValue);
                break;
            case DCAnimationType.Scale:
                if (_isUserInterface)
                {
                    _rectTransform.localScale = new Vector3(startValue, startValue, startValue);
                }
                else
                {
                    _transform.localScale = new Vector3(startValue, startValue, startValue);   
                }
                break;
            case DCAnimationType.Move:
                if (!_isUserInterface)
                {
                    _transform.position = startPosition;
                }
                else
                {
                    _rectTransform.anchoredPosition = startPosition;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChangeFadeValueByType(float value)
    {
        switch (componentType)
        {
            case DCAnimatorComponentType.Sprite:
                var spriteColor = _sprite.color;
                spriteColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, value);
                _sprite.color = spriteColor;
                break;
            case DCAnimatorComponentType.UI:
                var imageColor = _uiImage.color;
                imageColor = new Color(imageColor.r, imageColor.g, imageColor.b, value);
                _uiImage.color = imageColor;
                break;
            case DCAnimatorComponentType.Text:
                _textLabel.alpha = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessAnimation()
    {
        switch (type)
        {
            case DCAnimationType.Fade:
                ProcessFade();
                break;
            case DCAnimationType.Scale:
                ProcessScale();
                break;
            case DCAnimationType.Move:
                ProcessMove();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessFade()
    {
        var desiredValue = EasingMethods.GetEasingFunction(EasingMethods.Ease.Linear)
            .Invoke(startValue, endValue, _currentTime);
        
        ChangeFadeValueByType(desiredValue);
    }

    private void ProcessScale()
    {
        var desiredValue = EasingMethods.GetEasingFunction(easeType)
            .Invoke(startValue, endValue, _currentTime);

        if (!_isUserInterface)
        {
            _transform.localScale = new Vector3(desiredValue, desiredValue, desiredValue);   
        }
        else
        {
            _rectTransform.localScale = new Vector3(desiredValue, desiredValue, desiredValue);
        }
    }

    private void ProcessMove()
    {
        var x = EasingMethods.GetEasingFunction(easeType)
            .Invoke(startPosition.x, endPosition.x, _currentTime);
        var y = EasingMethods.GetEasingFunction(easeType)
            .Invoke(startPosition.y, endPosition.y, _currentTime);
        var z = EasingMethods.GetEasingFunction(easeType)
            .Invoke(startPosition.z, endPosition.z, _currentTime);

        if (_isUserInterface)
        {
            _rectTransform.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            _transform.localPosition = new Vector3(x, y, z);
        }
    }
    
    private void CompleteAnimation()
    {
        _canTransition = false;
        _currentTime = 0;
        if (endDelay > 0)
        {
            _delayType = DCDelayType.End;
            _delayTimer.OnTimerComplete += TimerComplete;
            _delayTimer.SetStartValue(endDelay);
            _delayTimer.Start();
        }
        else
        {
            IsFinished = true;
            _delayType = DCDelayType.None;
            AnimationComplete?.Invoke(this);
        }
    }
}
