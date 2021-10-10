using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

[CreateAssetMenu(fileName ="GameElement",menuName ="ScriptableObjects/AXD_GameElement",order =2)]

public class AXD_GameElement : ScriptableObject
{
    public enum Type
    {
        Pause,
        ButtonInstruction
    }
    public enum State
    {
        On,
        Off
    }

    [Header("General")]
    public float elementTime = 5f;
    public Type type;
    public State state = State.Off;

    [Header("Audio")]
    public AudioSource audio;
    public float timeToPlayAudio;

    [Header("Arduino")]
    public int buttonPin;
    public int ledPin;

    public void SetOn()
    {
        if (state != State.On)
        {
            state = State.On;
        }
    }

    public void SetOff()
    {
        if (state != State.Off)
        {
            UduinoManager.Instance.digitalWrite(ledPin, Uduino.State.LOW);
            state = State.Off;
        }
    }
}
