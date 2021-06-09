using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnemyField : GridObject
{
    [SerializeField]
    Sprite hit_;

    Image img_;
    Button btn_;

    private void Start()
    {
        img_ = GetComponent<Image>();
        btn_ = GetComponent<Button>();
    }

    public void OnClick()
    {
        Debug.Log("X: " + (x_ + 1) + " Y: " + (y_ + 1));

        if (Random.Range(0,100f) >= 83)   // Mandar ataque
        {
            img_.sprite = hit_;
            img_.color = Color.white;
            img_.type = Image.Type.Simple;
        }
        else
            img_.enabled = false;
        btn_.enabled = false;
    }

}
