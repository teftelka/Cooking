using System;
using System.Data;
using TMPro;
using UIScripts;
using UnityEngine;

public class ScoreManagerUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        ScoreManager.Instance.OnExperienceUpdated += OnExperienceUpdated;
        ScoreManager.Instance.OnMoneyUpdated += OnMoneyUpdated;
    }

    private void OnMoneyUpdated(object sender, EventArgs e)
    {
        moneyText.text = ScoreManager.Instance.GetMoneyAmount().ToString();
    }

    private void OnExperienceUpdated(object sender, EventArgs e)
    {
        experienceText.text = ScoreManager.Instance.GetExpAmount().ToString();
    }
}
