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
    public AXD_GameRules rules;

    [Header("Audio")]
    public AXD_ClipInfos audioBeginningOn;
    public AXD_ClipInfos audioBeginningOff;
    public float audioBeginningTimeOn;
    public float audioBeginningTimeOff;
    public float timeToPlayAudio;

    [Header("Arduino")]
    public char buttonChar;
    public KeyCode input;
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
    public void LightOn()
    {
        UduinoManager.Instance.digitalWrite(ledPin, Uduino.State.HIGH);
    }

    public void LightOff()
    {
        UduinoManager.Instance.digitalWrite(ledPin, Uduino.State.LOW);
    }
    
    public IEnumerator Verify(){
        while(state == State.On){

            if(Input.GetKey(input)){
                UduinoManager.Instance.digitalWrite(ledPin, Uduino.State.HIGH);
                yield return new WaitForSeconds(0.1f);
            }else{
                LightOn();
                yield return new WaitForSeconds(1/rules.blinkingFrequency);
                LightOff();
            }
        }
    }
}
