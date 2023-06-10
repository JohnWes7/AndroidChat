using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private InputField userinput;
    [SerializeField] private Text inputText;

    public void InputValueChange()
    {
        inputText.text = userinput.text;
    }
}
