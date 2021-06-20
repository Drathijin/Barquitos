using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdText : MonoBehaviour
{
    Text text_;
    void Start()
    {
        text_ = GetComponent<Text>();
        GameManager.Instance().SetIdText(this);
    }

    public void SetText(string id)
    {
        text_.text = "Game id: " + id;
    }
}
