using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GamesRules",menuName ="ScriptableObjects/AXD_GameRules",order =1)]
public class AXD_GameRules : ScriptableObject
{
    public float timeBetweenElements;
    public float timeToLoseIfButtonIsReleased;
    public float timeToLoseIfButtonIsNotPressed;
    public float blinkingFrequency;
    public int[] ledPins;
    public int[] buttonPins;
}
