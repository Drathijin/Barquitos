using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDataGroup : MonoBehaviour
{
    public AIData data_;

    [SerializeField]
    Text center, closer, horizontal;

    [SerializeField]
    List<Toggle> toggles;

    void Start()
    {
        data_ = new AIData();
    }

    public void OnCenterPSlider(float slider)
    {
        data_.centerPriority = slider;
        center.text = ((int)(slider * 100)).ToString() + "%";
    }

    public void OnClosePSlider(float slider)
    {
        data_.closerPriority = slider;
        closer.text = ((int)(slider * 100)).ToString() + "%";
    }

    public void OnHorizontalPSlider(float slider)
    {
        data_.horizontalPriority = slider;
        horizontal.text = ((int)(slider * 100)).ToString() + "%";
    }

    public void OnEasy(bool set)
    {
        if (!set)
            return;
        toggles[1].isOn = false;
        toggles[2].isOn = false;
        data_.diff = Difficulty.EASY;
    }

    public void OnMedium(bool set)
    {
        if (!set)
            return;
        toggles[0].isOn = false;
        toggles[2].isOn = false;
        data_.diff = Difficulty.MEDIUM;
    }

    public void OnHard(bool set)
    {
        if (!set)
            return;
        toggles[0].isOn = false;
        toggles[1].isOn = false;
        data_.diff = Difficulty.HARD;
    }
}