using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OfflineRewardConfig", menuName = "Offline Reward/Config")]
public class OfflineRewardConfig : ScriptableObject
{
    [Header("--- TIME SETTINGS ---")]
    [SerializeField] private float maxOfflineMinutes = 480f;
    public float MaxOfflineMinutes => maxOfflineMinutes;
    public TimeSpan MaxOfflineDuration => TimeSpan.FromMinutes(maxOfflineMinutes);
    [SerializeField] private float cycleDurationSeconds = 60f;
    public float CycleDurationSeconds => cycleDurationSeconds;
    

    [Header("--- REWARD DATA ---")]
    [SerializeField] private List<RewardData> rewardDatas;
    public List<RewardData> RewardDatas => rewardDatas;


    public RewardData GetRewardRate(RewardType type)
    {
        return rewardDatas.Find(r => r.rewardType == type);
    }

    public int GetAmountPerMinute(RewardType type)
    {
        var rate = GetRewardRate(type);
        return rate != null ? rate.amountPerMinute : 0;
    }
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    public int amountPerMinute = 1;
}
