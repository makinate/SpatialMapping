using UnityEngine;
using System.Collections;

public class InputField : MonoBehaviour
{

    TouchScreenKeyboard keyboard;
    public static string keyboardText = "";

    void Start()
    {
        keyboard = new TouchScreenKeyboard("Sample text that goes into the textbox", TouchScreenKeyboardType.Default, false, false, false, false, "sample prompting text that goes above the textbox");
    }

    void Update()
    {

        if (keyboard != null && keyboard.active == false)
        {
            if (keyboard.done == true)
            {
                keyboardText = keyboard.text;
                keyboard = null;
            }
        }
    }
}