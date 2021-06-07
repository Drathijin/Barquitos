using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonData : MonoBehaviour
{
    [SerializeField]
    private int x_ = -1, y_ = -1;

    [SerializeField]
    Sprite hit_;

    Image img_;
    Button btn_;

    private void Start()
    {
        img_ = GetComponent<Image>();
        btn_ = GetComponent<Button>();
    }

    public void SetButton(int x, int y)
    {
        x_ = x;
        y_ = y;
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
