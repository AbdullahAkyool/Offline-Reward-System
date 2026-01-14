using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIItem : MonoBehaviour, IUpdateable
{
    [SerializeField] private RewardType rewardType;
    public RewardType RewardType => rewardType;

    [SerializeField] private Image progressBar;
    [SerializeField] private ParticleSystem collectParticle;

    private int _amountPerCycle;
    private float _cycleDuration;
    private float _currentProgress;
    private bool _isRunning;

    public event Action<RewardType, int> OnCycleComplete;

    private void Awake()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
        }
    }

    public void Initialize(int amountPerCycle, float cycleDuration, float startProgress)
    {
        _amountPerCycle = amountPerCycle;
        _cycleDuration = cycleDuration;
        _currentProgress = Mathf.Clamp01(startProgress);

        if (progressBar != null)
        {
            progressBar.fillAmount = _currentProgress;
        }

        StartCycle();
    }

    private void StartCycle()
    {
        if (_isRunning) return;
        
        _isRunning = true;
        UpdateManager.Instance.Register(this);
    }

    public void OnUpdate(float deltaTime)
    {
        if (!_isRunning || _cycleDuration <= 0) return;

        _currentProgress += deltaTime / _cycleDuration;

        if (progressBar != null)
        {
            progressBar.fillAmount = Mathf.Clamp01(_currentProgress);
        }

        if (_currentProgress >= 1f)
        {
            CompleteCycle();
        }
    }

    private void CompleteCycle()
    {
        _currentProgress = 0f;
        
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
        }

        OnCycleComplete?.Invoke(rewardType, _amountPerCycle);
        PlayCollectEffect();
    }

    public void PlayCollectEffect()
    {
        if (collectParticle != null)
        {
            collectParticle.Play();
        }
    }

    public void StopCycle()
    {
        if (!_isRunning) return;
        
        _isRunning = false;
        UpdateManager.Instance.Unregister(this);
    }

    public void ResetItem()
    {
        StopCycle();
        _currentProgress = 0f;
        _amountPerCycle = 0;

        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
        }
    }

    private void OnDisable()
    {
        StopCycle();
    }
}
