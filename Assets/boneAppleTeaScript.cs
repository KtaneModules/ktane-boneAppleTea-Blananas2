using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class boneAppleTeaScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] buttons; //0=TL, 1=BL, 2=TR, 3=BL, 4=submit
    public TextMesh[] texts; //0=T, 1=B, 2=L, 3=R

    public List<string> characters = new List<string> { " 0 ", " 1 ", " 2 ", " 3 ", " 4 ", " 5 ", " 6 ", " 7 ", " 8 ", " 9 ", " A ", " B ", " C ", " D ", " E ", " F ", " G ", " H ", " I ", " J ", " K ", " L ", " M ", " N ", " O ", " P ", " Q ", " R ", " S ", " T ", " U ", " V ", " W ", " X ", " Y ", " Z ", " & ", " $ " };
    int phrase1 = 0;
    int phrase2 = 0;
    int leftChar = 0;
    int rightChar = 0;
    public List<string> phrases = new List<string> {
        "Bone Apple Tea",
        "Seizure Salad",
        "Hey to break it to ya",
        "This is oak ward",
        "Clea Shay",
        "It’s in tents",
        "Bench watch",
        "You’re an armature",
        "Man hat in",
        "Try all and era",
        "Million Air",
        "Die of beaties",
        "Rush and roulette",
        "Night and shining armour",
        "What a nice jester",
        "In some near",
        "This is my master peace",
        "I’m in a colder sac",
        "Cereal killer",
        "I come here off ten",
        "Slide of ham",
        "Test lah",
        "Refreshing campaign",
        "I’m being more pacific",
        "God blast you",
        "BC soft wear",
        "Sense in humor",
        "The three must of tears",
        "Third da men chin",
        "Prang mantas",
        "Hammy downs",
        "Yum, a case idea",
        "Dandy long legs",
        "Can’t merge, little lone drive",
        "My guest is",
        "Sink",
        "You lake it",
        "Emit da feet"
    };

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons) {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }
    }

    // Use this for initialization
    void Start () {
		phrase1 = UnityEngine.Random.Range(0, 38);
        phrase2 = UnityEngine.Random.Range(0, 38);
        texts[0].text = phrases[phrase1];
        texts[1].text = phrases[phrase2];
        Debug.LogFormat("[Bone Apple Tea #{0}] The top phrase is: \"{1}\"", moduleId, phrases[phrase1]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The bottom phrase is: \"{1}\"", moduleId, phrases[phrase2]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The correct left character is:{1}", moduleId, characters[phrase1]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The correct right character is:{1}", moduleId, characters[phrase2]);
    }

    void buttonPress(KMSelectable button)
    {
        if (moduleSolved == false) {
            button.AddInteractionPunch();
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (button == buttons[4])
            {
                if (leftChar == phrase1 && rightChar == phrase2)
                {
                    Debug.LogFormat("[Bone Apple Tea #{0}] The characters you submitted are{1}and{2}, which are correct. Module solved.", moduleId, characters[leftChar], characters[rightChar]);
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                } else
                {
                    Debug.LogFormat("[Bone Apple Tea #{0}] The characters you submitted are{1}and{2}, which are incorrect. Module striked.", moduleId, characters[leftChar], characters[rightChar]);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else if (button == buttons[0])
            {
                leftChar = (leftChar + 1) % 38;
                texts[2].text = characters[leftChar];
            }
            else if (button == buttons[1])
            {
                leftChar = (leftChar + 37) % 38;
                texts[2].text = characters[leftChar];
            }
            else if (button == buttons[2])
            {
                rightChar = (rightChar + 1) % 38;
                texts[3].text = characters[rightChar];
            }
            else if (button == buttons[3])
            {
                rightChar = (rightChar + 37) % 38;
                texts[3].text = characters[rightChar];
            }
        }
    }
}
