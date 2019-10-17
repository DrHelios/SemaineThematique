using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour
{
    public Text changetext;

    private void Start()
    {
        changetext.text = ApplicationData.gameOverText;
    }
}
