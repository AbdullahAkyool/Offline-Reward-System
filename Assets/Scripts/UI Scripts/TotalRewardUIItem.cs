using UnityEngine;
using TMPro;

public class TotalRewardUIItem : MonoBehaviour
{
    [SerializeField] private RewardType rewardType;
    public RewardType RewardType => rewardType;
    [SerializeField] private TMP_Text totalAmountText;

    private int _totalAmount;
    public int TotalAmount => _totalAmount;

    private void Awake()
    {
        ResetTotal();
    }

    public void AddAmount(int amount)
    {
        _totalAmount += amount;
        UpdateUI();
    }

    public void SetTotal(int amount)
    {
        _totalAmount = amount;
        UpdateUI();
    }

    public void ResetTotal()
    {
        _totalAmount = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (totalAmountText != null)
        {
            totalAmountText.text = _totalAmount.ToString();
        }
    }
}
