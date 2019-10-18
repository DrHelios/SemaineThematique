using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using Rules = SpaceInvadersGameState;


public class ButtonScriptCopy : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;

    public static int action;

    // Start is called before the first frame update
    void Start()
    {

        Button button = button1.GetComponent<Button>();
        button.onClick.AddListener(Up);

       // Button button = but.GetComponent<Button>();
       // button.onClick.AddListener(Down);

       // Button button = but.GetComponent<Button>();
       // button.onClick.AddListener(Fire);
    }

    // Update is called once per frame
    void Update()
    {
        if (!button1.isActiveAndEnabled || !button2.isActiveAndEnabled || !button3.isActiveAndEnabled || !button4.isActiveAndEnabled || !button5.isActiveAndEnabled || !button6.isActiveAndEnabled)
        {
            action = 0;
            Debug.Log("Action 0");
        }
    }

    public void Up()
    {
        action = 1;
        Debug.Log("Action 1");
    }

    public void Down()
    {
        action = 2;
        Debug.Log("Action 2");
    }

    public void Fire()
    {
        action = 3;
        Debug.Log("Action 3");
    }

    public void Up2()
    {
        action = 5;
        Debug.Log("Action 5");
    }

    public void Down2()
    {
        action = 6;
        Debug.Log("Action 6");
    }

    public void Fire2()
    {
        action = 7;
        Debug.Log("Action 7");
    }
}
