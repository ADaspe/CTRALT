using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class AXD_GameManager : MonoBehaviour
{
    public AXD_GameRules rules;
    public Queue<AXD_GameElement> sequence;
    private Queue<AXD_GameElement> sequenceCopy;
    private AXD_GameElement currentElement;
    [SerializeField] private bool hasPressedButton;
    [SerializeField] private float timeBeforeNextIntruction = 0;
    [SerializeField] private float currentElementStartingTime = 0;
    [SerializeField] private float timeSinceButtunReleased = 0;
    [SerializeField] private float timeSinceButtonBlink = 0;


    private void Start()
    {
        foreach (int pin in rules.ledPins)
        {
            UduinoManager.Instance.pinMode(pin, PinMode.Output);
        }
        foreach (int pin in rules.buttonPins)
        {
            UduinoManager.Instance.pinMode(pin, PinMode.Input_pullup);
        }
        UduinoManager.Instance.pinMode(8,PinMode.Output);
        sequenceCopy = new Queue<AXD_GameElement>(sequence);
    }

    private void Update()
    {
        //Si aucun élément n'est activé, on prend le prochain
        if(currentElement == null)
        {
            currentElement = sequence.Dequeue();
            currentElement.SetOn();
            currentElementStartingTime = Time.time;
        }

        //On active l'élément (Si c'est une pause, on attend, si c'est un bouton, on active le blink, jusqu'à la pression du joueur, puis on active le continu)
        if (currentElement.type == AXD_GameElement.Type.ButtonInstruction)
        {
            if (UduinoManager.Instance.digitalRead(currentElement.buttonPin) == 0)
            {
                currentElement.BlinkingLight();
                if (hasPressedButton)
                {
                    timeSinceButtunReleased += Time.deltaTime;
                }
                else
                {
                    timeSinceButtonBlink += Time.deltaTime;
                }
            }
            else if(UduinoManager.Instance.digitalRead(currentElement.buttonPin) == 1)
            {
                currentElement.ContinousLigth();
                if (!hasPressedButton)
                {
                    hasPressedButton = true;
                }
            }
        }
        else if (currentElement.type == AXD_GameElement.Type.Pause && timeBeforeNextIntruction < Time.time)
        {
            timeBeforeNextIntruction = Time.time + currentElement.elementTime;
        }


        //Si le joueur relâche trop longtemps ou n'appuie pas sur le bouton, il perd la partie

    }


}
