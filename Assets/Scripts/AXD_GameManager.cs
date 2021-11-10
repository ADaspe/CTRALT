using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class AXD_GameManager : MonoBehaviour
{
    [Header("References")]
    public AXD_GameRules rules;
    public List<AXD_GameElement> sequence;

    [Header("GameState")]
    public bool gameOver = false;
    public bool gameStarted = true;

    [SerializeField] private AXD_GameElement currentElement;
    [SerializeField] private int currentElementIndex = 0;

    [SerializeField] private bool hasPressedButton;
    [SerializeField] private float timeBeforeNextIntruction = 0;
    [SerializeField] private float currentElementStartingTime = 0;
    [SerializeField] private float timeSinceButtonReleased = 0;
    [SerializeField] private float timeSinceButtonBlink = 0;


    private void Start()
    {
        foreach (int pin in rules.ledPins)
        {
            UduinoManager.Instance.pinMode(pin, PinMode.Output);
            UduinoManager.Instance.digitalWrite(pin, State.LOW);
        }
        foreach (AXD_GameElement element in sequence){
            element.state = AXD_GameElement.State.Off;
        }
        //StartCoroutine(TestAnalog());
    }

    private void FixedUpdate()
    {
        //TestAnalog();
        if (!gameOver && gameStarted)
        {
            if ((currentElement == null) || 
            currentElementStartingTime + currentElement.elementTime <= Time.time)
            {
                Debug.Log("No Element");
                if(currentElement == null || currentElementIndex+1 == sequence.Count)
                {
                    currentElementIndex = 0;
                }
                else
                {
                    currentElementIndex++;
                }
                currentElement = sequence[currentElementIndex];

                if(currentElement.state == AXD_GameElement.State.On)
                {
                    currentElement.SetOff();
                }
                else
                {
                    currentElement.SetOn();
                    StartCoroutine(currentElement.Verify());
                }

                currentElementStartingTime = Time.time;
                Debug.Log("Current element = " + currentElement.name);

            }

            //On active l'élément (Si c'est une pause, on attend, si c'est un bouton, on active le blink, jusqu'� la pression du joueur, puis on active le continu)
            if (currentElement.type == AXD_GameElement.Type.ButtonInstruction 
            && currentElement.state == AXD_GameElement.State.On)
            {
                Debug.Log("Anykey ?" + Input.anyKey);
                if (!(Input.GetKey(currentElement.input)))
                {
                    Debug.Log("Appuyé ? False");
                    if (hasPressedButton)
                    {
                        timeSinceButtonReleased += Time.deltaTime;
                    }
                    else
                    {
                        timeSinceButtonBlink += Time.deltaTime;
                    }
                }
                else 
                {
                    Debug.Log("Appuyé ? True");
                    //Si c'est appuyé, on met la lumière
                    currentElement.LightOn();
                    if (!hasPressedButton)
                    {
                        hasPressedButton = true;
                    }
                    //Si le joueur appuie, on réinitialise les délais de game over
                    timeSinceButtonReleased = 0;
                    timeSinceButtonBlink = 0;
                }
            }
            else if (currentElement.type == AXD_GameElement.Type.Pause 
            && timeBeforeNextIntruction < Time.time)
            {
                Debug.Log("Pause");
                timeBeforeNextIntruction = Time.time + currentElement.elementTime;
            }
            //Si le joueur rel�che trop longtemps ou n'appuie pas sur le bouton, il perd la partie
            if (timeSinceButtonReleased >= rules.timeToLoseIfButtonIsReleased 
            || timeSinceButtonBlink >= rules.timeToLoseIfButtonIsNotPressed)
            {
                GameOver();
            } 
        }
    }
    
    public void GameOver()
    {
        Debug.Log("GameOver");
        // � d�finir comment on mat�rialise le game over;
        foreach (int pin in rules.ledPins){
            UduinoManager.Instance.digitalWrite(pin, State.LOW);
        }
        gameOver = true;
        
    }

    public IEnumerator TestAnalog(){
        UduinoManager.Instance.pinMode(14, PinMode.Output);
        yield return new WaitForSeconds(0.5f);
        UduinoManager.Instance.digitalWrite(14, State.HIGH);
        UduinoManager.Instance.pinMode(14, PinMode.Output);
        yield return new WaitForSeconds(0.5f);
        UduinoManager.Instance.digitalWrite(14, State.HIGH);
    }

    /*public IEnumerator BlinkLight(AXD_GameElement element)
    {
        while (!(Input.GetKey(currentElement.input)) 
        && element.state == AXD_GameElement.State.On 
        && gameStarted 
        && !gameOver) {
            LightOn(element.ledPin);
            yield return new WaitForSeconds(1/rules.blinkingFrequency);
            LightOff(element.ledPin);
        }
    }*/
}
