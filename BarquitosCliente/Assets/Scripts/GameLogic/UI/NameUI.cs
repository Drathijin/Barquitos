using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUI : MonoBehaviour
{
    Text text_;
    [SerializeField]
    bool enemy;

    void Start()
    {
        text_ = GetComponent<Text>();
        if (enemy)
            GameManager.Instance().SetEnemyNameUI(this);
        else
            GameManager.Instance().SetPlayerNameUI(this);
    }

    public void SetName(string name)
    {
        text_.text = name;
    }
}
