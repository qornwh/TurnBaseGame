using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideBarUi : MonoBehaviour
{
    private int maxVal;
    private int curVal;
    
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    public void Init(int maxVal)
    {
        this.maxVal = maxVal;
        curVal = maxVal;
    }

    public void UpdatePersent(int curVal)
    {
        this.curVal = curVal;
        text.text = curVal.ToString() + '/' + maxVal.ToString();    
        slider.value = (float)this.curVal / maxVal;
    }
}
