using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


// This class is a very hacky way to get Unity to play nice with Dictonaries in the inspector
[InitializeOnLoad]
[CreateAssetMenu(fileName = "UIBarks", menuName = "ScriptableObjects/UIBarks", order = 1)]
public class UIBarks : ScriptableObject
{
    [Serializable]
    public struct BarkInfo
    {
        public string BarkName;
        public int EmojiNumber;
    }

    public List<BarkInfo> BarkList;
    public Dictionary<string, int> BarkDict = new Dictionary<string, int>();

    private void OnEnable()
    {
        foreach (var bark in BarkList)
        {
            BarkDict[bark.BarkName] = bark.EmojiNumber;
            Debug.Log(BarkDict[bark.BarkName]);
        }
    }
}
