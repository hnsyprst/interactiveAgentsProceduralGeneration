using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AgentCommunicate : MonoBehaviour
{
    public TMPro.TextMeshProUGUI BarkTextMesh;

    public UIBarks Barks;

    // Start is called before the first frame update
    void Start()
    {
        BarkTextMesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void UIBark(string BarkName)
    {
        int Emoji = Barks.BarkDict[BarkName];
        BarkTextMesh.text = "<sprite=" + Emoji + ">";
    }*/

    public void UIBark(params string[] BarkName)
    {
        string OutputText = "";
        foreach (string bark in BarkName)
        {
            int Emoji = Barks.BarkDict[bark];
            OutputText += "<sprite=" + Emoji + ">";
        }
        BarkTextMesh.text = OutputText;
    }

    public void UIBarkStop()
    {
        BarkTextMesh.text = "";
    }
}
