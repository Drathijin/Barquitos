using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovButtons : MonoBehaviour
{
    [SerializeField]
    bool next;

    void Start()
    {
        Button b = GetComponent<Button>();
        b.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (next)
            GameManager.Instance().NextFleet();
        else
            GameManager.Instance().PreviousFleet();
    }
}
