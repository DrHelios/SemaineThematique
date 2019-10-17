using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSelection : MonoBehaviour
{
    public int player1Agent = 0;
    public int player2Agent = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer1Agent(int agentIndex)
    {
        player1Agent = agentIndex;
        Debug.Log("Agent 1 : " + player1Agent);
    }

    public void SetPlayer2Agent(int agentIndex)
    {
        player2Agent = agentIndex;
        Debug.Log("Agent 2 : " + player2Agent);
    }
}
