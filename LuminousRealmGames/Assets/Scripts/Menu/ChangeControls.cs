using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using MalbersAnimations;
using UnityEngine.InputSystem;

public class ChangeControls : MonoBehaviour
{
    public KeyCode speedUpKey = KeyCode.Alpha1;
    public KeyCode speedDownKey = KeyCode.Alpha2;
    [SerializeField] private List<TextMeshProUGUI> buttons;
    [SerializeField] private MalbersInput malbersInput;
    private void Update()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].text == "Awaiting Input")
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kcode))
                    {
                        buttons[i].text = kcode.ToString();
                        string name = buttons[i].name;
                        //malbersInput.inputs[i].key = kcode;
                        
                    }
                }
            }
        }
    }

    public void ChangeSpeedUpKey()
    {
        buttons[0].text = "Awaiting Input";
        malbersInput.SetMap("Default");
    }
    public void ChangeSpeedDownKey()
    {
        buttons[1].text = "Awaiting Input";
    }
}
