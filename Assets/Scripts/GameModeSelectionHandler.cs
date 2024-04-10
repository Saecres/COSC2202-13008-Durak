using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSelectionHandler : MonoBehaviour
{
    public void OnAIBtnPressed()
    {
        GameSettings.IsPlayingAgainstAI = true;
        Debug.Log("AI Mode Selected");
    }

    public void OnPVPBtnPressed()
    {
        GameSettings.IsPlayingAgainstAI = false;
        Debug.Log("PVP Mode Selected");
    }
}


