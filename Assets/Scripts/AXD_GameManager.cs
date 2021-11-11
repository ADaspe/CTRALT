using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class AXD_GameManager : MonoBehaviour
{
    [Header("References")]
    public AXD_GameRules rules;
    public List<AXD_GameElement> sequence;

    public AXD_AudioManager audioManager;

    public AudioSource musicIntro;
    public AudioSource musicGame;

    [Header("GameState")]
    public int defeats;
    public bool gameStarted = true;

    [SerializeField] private AXD_GameElement currentElement;
    [SerializeField] private int currentElementIndex = 0;

    [SerializeField] private bool hasPressedButton;
    [SerializeField] private float timeBeforeNextIntruction = 0;
    [SerializeField] private float currentElementStartingTime = 0;
    [SerializeField] private float timeSinceButtonReleased = 0;
    [SerializeField] private float timeSinceButtonBlink = 0;

    [Header ("Button Smash Variables")]
    public KeyCode KeySmashButton;
    public int gaugeCurrentScore;
    public int scorePerSmash;
    public int scoreReduction;
    public float scoreReductionFrequency;
    private bool coroutineStarted;
    public int[] gaugeStep;
    public int[] gaugeLedPins;
    public bool isSmashButtonDone = false;
  


    private void Start()
    {
        defeats = 0;
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

    private void Update()
    {
        //TestAnalog();
        /*-- DONE -- game over n'arrête pas le jeu, compter le nombre de défaites 
        -- DONE (à tester pour être sûr) -- Lancer les audio des elements au début de l'élément
        -- A TESTER -- Lancer les audio de pause
        -- A TESTER --Faire l'UI (1 bouton toggle Start/Stop, 1 bouton "penses à continuer le rituel",
         demander le reste des boutons)
        -- A FAIRE --Pouvoir lancer la musique / changer de musique quand on a fini l'intro
        -- A FAIRE -- bouton smash*/
        
        if (gameStarted)
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

                if(currentElement.state == AXD_GameElement.State.On
                && currentElement.type == AXD_GameElement.Type.ButtonInstruction)
                {
                    //Joue le son du bouton qui s'éteint
                    StartCoroutine(PlaySource(currentElement.audioBeginningOff.linkedAudioSourceComponent));
                    Debug.Log("Son début extinction"+currentElement.name);
                    currentElement.SetOff();
                }
                else if(currentElement.state == AXD_GameElement.State.Off
                && currentElement.type == AXD_GameElement.Type.ButtonInstruction )
                {
                    //Joue le son du bouton qui s'allume
                    StartCoroutine(PlaySource(currentElement.audioBeginningOn.linkedAudioSourceComponent));
                    Debug.Log("Son début allumage"+currentElement.name);
                    currentElement.SetOn();
                    StartCoroutine(currentElement.Verify());

                }else if (currentElement.type == AXD_GameElement.Type.Pause){
                    //Si c'est la pause, jouer le texte de la pause
                    //currentElement.audioBeginningOn.Play();
                    Debug.Log("Son début allumage"+currentElement.name);
                }

                currentElementStartingTime = Time.time;
                //Debug.Log("Current element = " + currentElement.name);

            }

            //On active l'élément (Si c'est une pause, on attend, si c'est un bouton, on active le blink, jusqu'� la pression du joueur, puis on active le continu)
            if (currentElement.type == AXD_GameElement.Type.ButtonInstruction 
            && currentElement.state == AXD_GameElement.State.On)
            {
                //Debug.Log("Anykey ? " + Input.anyKey);
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
            
            //Si j'appuie, j'augmente le score --DONE--
            //Si le score dépasse un seuil, j'allume une nouvelle led -- DONE --
            //Si le dernier seuil est atteint j'arrête la coroutine et j'allume la dernière led -- DONE --
            //Si, en baissant, le score descend d'un seuil, éteindre la dernière led allumée.



            if(Input.GetKey(KeySmashButton) && !isSmashButtonDone){
                
                gaugeCurrentScore += scorePerSmash;
                if(!coroutineStarted){
                    StartCoroutine(SmashCoroutine());
                }
                for(int i = 0 ; i<gaugeStep.Length ; i++){
                    if(gaugeCurrentScore >= gaugeStep[i]){
                        UduinoManager.Instance.digitalWrite(gaugeLedPins[i], State.HIGH);
                    }
                }
                if(gaugeCurrentScore>=gaugeStep[gaugeStep.Length-1]){
                    StopCoroutine(SmashCoroutine());
                    UduinoManager.Instance.digitalWrite(gaugeLedPins[gaugeLedPins.Length-1]);
                    isSmashButtonDone = true;
                }
            }
            //faire l'allumage des leds en fonction du bouton smash

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
        defeats++;
        Debug.Log("GameOver : "+defeats);
        timeSinceButtonReleased = 0;
        timeSinceButtonBlink = 0;
        
    }

    public void ToggleStartStop(){
        gameStarted = !gameStarted;
    }

    public IEnumerator PlaySource(AudioSource source){

        source.enabled = true;
        yield return new WaitForSeconds(source.clip.length);
        source.enabled = false;
    }

    public IEnumerator SmashCoroutine(){
        coroutineStarted = true;
        while(gaugeCurrentScore > 0){
            yield return new WaitForSeconds(1/scoreReductionFrequency);
            gaugeCurrentScore -= scoreReduction;
            for(int i = 0 ; i<gaugeStep.Length ; i++){
                if(gaugeCurrentScore <= gaugeStep[i]){
                    UduinoManager.Instance.digitalWrite(gaugeLedPins[i], State.LOW);
                }
            }
            
            
        }
        coroutineStarted = false;
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
