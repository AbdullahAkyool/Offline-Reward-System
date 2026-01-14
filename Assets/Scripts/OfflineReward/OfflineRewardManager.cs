using System;
using UnityEngine;


public class OfflineRewardManager : MonoBehaviour
{
    public static OfflineRewardManager Instance; //service locator tercih ederim normalde

    [SerializeField] private OfflineRewardConfig offlineRewardConfig;
    public OfflineRewardConfig OfflineRewardConfig => offlineRewardConfig;

    [SerializeField] private OfflieRewardsPanelController rewardPanel;

    private TimeSpan _currentOfflineDuration;
    public TimeSpan CurrentOfflineDuration => _currentOfflineDuration;
    private TimeSpan _rewardDuration;
    public TimeSpan RewardDuration => _rewardDuration;
    private TimeSpan _accumulatedDuration;
    private TimeSpan _startupOfflineDuration;
    private DateTime _sessionStartUtc;
    private bool _isInitialized;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePanel();
    }

    private void OnApplicationQuit() //cikis zamanini kaydet
    {
        OfflineRewardData.SaveExitTime(GetCurrentAccumulatedDuration());
    }

    private void OnApplicationPause(bool pauseStatus) //cikis zamanini kaydet
    {
        if (pauseStatus)
        {
            OfflineRewardData.SaveExitTime(GetCurrentAccumulatedDuration());
        }
    }

    private void InitializePanel() //offline paneli offline sureye gore initialize et
    {
        EnsureInitialized();

        _currentOfflineDuration = GetCurrentDisplayDuration();
        _rewardDuration = GetCurrentAccumulatedDuration() + GetCappedOfflineDuration();

        rewardPanel.Initialize(_currentOfflineDuration, _rewardDuration, offlineRewardConfig);
    }

    public void RefreshPanel()
    {
        InitializePanel();
    }

    public void ResetDurationTracking()
    {
        EnsureInitialized();
        _accumulatedDuration = TimeSpan.Zero;
        _startupOfflineDuration = TimeSpan.Zero;
        _sessionStartUtc = DateTime.UtcNow;
        OfflineRewardData.SaveExitTime(TimeSpan.Zero);
    }

    private void EnsureInitialized()
    {
        if (_isInitialized) return;

        _sessionStartUtc = DateTime.UtcNow;
        _accumulatedDuration = OfflineRewardData.LoadAccumulatedDuration();
        _startupOfflineDuration = OfflineRewardData.GetOfflineDuration() ?? TimeSpan.Zero;
        _isInitialized = true;
    }

    private TimeSpan GetCurrentAccumulatedDuration()
    {
        EnsureInitialized();
        TimeSpan sessionElapsed = DateTime.UtcNow - _sessionStartUtc;
        if (sessionElapsed < TimeSpan.Zero)
        {
            sessionElapsed = TimeSpan.Zero;
        }

        return _accumulatedDuration + sessionElapsed;
    }

    private TimeSpan GetCurrentDisplayDuration()
    {
        return GetCurrentAccumulatedDuration() + _startupOfflineDuration;
    }

    private TimeSpan GetCappedOfflineDuration()
    {
        if (_startupOfflineDuration > offlineRewardConfig.MaxOfflineDuration)
        {
            return offlineRewardConfig.MaxOfflineDuration;
        }

        return _startupOfflineDuration;
    }

    public static string FormatDuration(TimeSpan duration) // time span formatlama helper
    {
        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }
        else if (duration.TotalMinutes >= 1)
        {
            return $"{(int)duration.TotalMinutes}m {duration.Seconds}s";
        }
        else
        {
            return $"{(int)duration.TotalSeconds}s";
        }
    }
}
