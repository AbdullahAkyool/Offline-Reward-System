using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class OfflieRewardsPanelController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Coroutine _durationCoroutine;
    private TimeSpan _currentDuration;

    [Header("--- TOP CONTAINER ---")]
    [SerializeField] private ButtonController closeButton;

    [Header("--- CENTER CONTAINER ---")]
    [SerializeField] private List<RewardUIItem> rewardUIItems;
    [SerializeField] private List<TotalRewardUIItem> totalRewardUIItems;
    [SerializeField] private TMP_Text durationText;

    [Header("--- BOTTOM CONTAINER ---")]
    [SerializeField] private ButtonController collectButton;
    [SerializeField] private ButtonController doubleCollectButton;

    private Dictionary<RewardType, TotalRewardUIItem> _totalItemsByType; //total itemlara hizli erisim icin

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _totalItemsByType = new Dictionary<RewardType, TotalRewardUIItem>();

        foreach (var item in totalRewardUIItems) //total itemlari dictionaryye ekle
        {
            if (item != null && !_totalItemsByType.ContainsKey(item.RewardType))
            {
                _totalItemsByType[item.RewardType] = item;
            }
        }

        foreach (var item in rewardUIItems) //reward item eventlerine abone ol
        {
            if (item != null)
            {
                item.OnCycleComplete += OnRewardCycleComplete;
            }
        }
    }

    private void Start()
    {
        closeButton.AddListener(HidePanel);
        collectButton.AddListener(OnCollectClicked);
        doubleCollectButton.AddListener(OnDoubleCollectClicked);
    }

    public void ShowPanel()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        if (OfflineRewardManager.Instance != null)
        {
            OfflineRewardManager.Instance.RefreshPanel();
        }
    }

    public void HidePanel()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        foreach (var item in rewardUIItems)
        {
            if (item != null) item.StopCycle();
        }

        StopDurationTimer();
    }

    public void Initialize(TimeSpan displayDuration, TimeSpan rewardDuration, OfflineRewardConfig config) //offline sureye gore paneli initialize et
    {
        _currentDuration = displayDuration;
        UpdateDurationText();
        StartDurationTimer();

        float cycleDuration = config.CycleDurationSeconds;

        // tamamlanan dongu sayisi ve suanki dongu ilerlemesi
        int completedCycles = OfflineRewardData.GetCompletedCycles(rewardDuration, cycleDuration);
        float cycleProgress = OfflineRewardData.GetCurrentCycleProgress(rewardDuration, cycleDuration);

        foreach (var item in rewardUIItems) //gecen sureye gore her bir reward itemi initialize et
        {
            if (item == null) continue;

            int amountPerCycle = config.GetAmountPerMinute(item.RewardType);

            if (_totalItemsByType.TryGetValue(item.RewardType, out var totalItem))
            {
                int initialTotal = completedCycles * amountPerCycle;
                totalItem.SetTotal(initialTotal);
            }

            item.Initialize(amountPerCycle, cycleDuration, cycleProgress); //progress bari suanki ilerleme ile baslat
        }

        Debug.Log($"Initialized with {completedCycles} completed cycles, cycle progress: {cycleProgress:P0}");
        UpdateCollectButtonsState();
    }

    private void OnRewardCycleComplete(RewardType type, int amount) //bir tam dongu tamamlandiginda totale ekle
    {
        if (_totalItemsByType.TryGetValue(type, out var totalItem))
        {
            totalItem.AddAmount(amount);
        }

        UpdateCollectButtonsState();
    }

    private void OnCollectClicked()
    {
        CollectRewards(false);
    }

    private void OnDoubleCollectClicked()
    {
        Debug.Log("Watching ad for double rewards...");
        CollectRewards(true);
    }

    private void CollectRewards(bool isDouble)
    {
        int multiplier = isDouble ? 2 : 1;
        string suffix = isDouble ? " (2x)" : "";

        foreach (var totalItem in totalRewardUIItems)
        {
            if (totalItem == null) continue;

            int amount = totalItem.TotalAmount * multiplier;
            Debug.Log($"Collected {amount} {totalItem.RewardType}{suffix}");

            foreach (var rewardItem in rewardUIItems) //reward itemlar icin collect efekti oynat
            {
                if (rewardItem != null && rewardItem.RewardType == totalItem.RewardType)
                {
                    rewardItem.PlayCollectEffect();
                }
            }

            totalItem.ResetTotal();
        }

        if (OfflineRewardManager.Instance != null)
        {
            OfflineRewardManager.Instance.ResetDurationTracking();
        }

        _currentDuration = TimeSpan.Zero;
        UpdateDurationText();

        UpdateCollectButtonsState();
    }

    private void OnDestroy()
    {
        foreach (var item in rewardUIItems)
        {
            if (item != null)
            {
                item.OnCycleComplete -= OnRewardCycleComplete;
            }
        }

        if (collectButton != null)
            collectButton.RemoveListener(OnCollectClicked);
        if (doubleCollectButton != null)
            doubleCollectButton.RemoveListener(OnDoubleCollectClicked);
    }

    private void StartDurationTimer()
    {
        StopDurationTimer();
        _durationCoroutine = StartCoroutine(DurationTick());
    }

    private void StopDurationTimer()
    {
        if (_durationCoroutine != null)
        {
            StopCoroutine(_durationCoroutine);
            _durationCoroutine = null;
        }
    }

    private IEnumerator DurationTick()
    {
        var wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;
            _currentDuration = _currentDuration.Add(TimeSpan.FromSeconds(1));
            UpdateDurationText();
        }
    }

    private void UpdateDurationText()
    {
        if (durationText != null)
        {
            durationText.text = OfflineRewardManager.FormatDuration(_currentDuration);
        }
    }

    private void UpdateCollectButtonsState()
    {
        bool hasRewards = false;
        foreach (var totalItem in totalRewardUIItems)
        {
            if (totalItem != null && totalItem.TotalAmount > 0)
            {
                hasRewards = true;
                break;
            }
        }

        if (collectButton != null)
        {
            collectButton.SetInteractable(hasRewards);
        }

        if (doubleCollectButton != null)
        {
            doubleCollectButton.SetInteractable(hasRewards);
        }
    }
}
