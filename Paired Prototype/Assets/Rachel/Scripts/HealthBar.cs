using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image barFill; // assign in Inspector

    public void SetHealth(int current, int max)
    {
        if (barFill == null || max <= 0) return;

        float ratio = Mathf.Clamp01((float)current / max);
        barFill.fillAmount = ratio;

        // Update color based on thresholds
        if (ratio > 0.5f)
        {
            barFill.color = Color.green; // > 50%
        }
        else if (ratio > 0.2f)
        {
            barFill.color = Color.yellow; // 20% - 50%
        }
        else
        {
            barFill.color = Color.red; // <= 20%
        }
    }
}