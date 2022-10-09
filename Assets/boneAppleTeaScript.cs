using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using KModkit;

public class boneAppleTeaScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] buttons; //0=TL, 1=BL, 2=TR, 3=BL, 4=submit
    public TextMesh[] texts; //0=T, 1=B, 2=L, 3=R

    private Coroutine buttonHold;
    private bool holding = false;
    private int btn = 0;

    private List<string> characters = new List<string> { " 0 ", " 1 ", " 2 ", " 3 ", " 4 ", " 5 ", " 6 ", " 7 ", " 8 ", " 9 ", " A ", " B ", " C ", " D ", " E ", " F ", " G ", " H ", " I ", " J ", " K ", " L ", " M ", " N ", " O ", " P ", " Q ", " R ", " S ", " T ", " U ", " V ", " W ", " X ", " Y ", " Z ", " & ", " $ " };
    int phrase1 = 0;
    int phrase2 = 0;
    int leftChar = 0;
    int rightChar = 0;
    private List<string> phrases = new List<string> {
        "Bone Apple Tea",
        "Seizure Salad",
        "Hey to break it to ya",
        "This is oak ward",
        "Clea Shay",
        "It's in tents",
        "Bench watch",
        "You're an armature",
        "Man hat in",
        "Try all and era",
        "Million Air",
        "Die of beaties",
        "Rush and roulette",
        "Night and shining armour",
        "What a nice jester",
        "In some near",
        "This is my master peace",
        "I'm in a colder sac",
        "Cereal killer",
        "I come here off ten",
        "Slide of ham",
        "Test lah",
        "Refreshing campaign",
        "I'm being more pacific",
        "God blast you",
        "BC soft wear",
        "Sense in humor",
        "The three must of tears",
        "Third da men chin",
        "Prang mantas",
        "Hammy downs",
        "Yum, a case idea",
        "Dandy long legs",
        "Can't merge, little lone drive",
        "My guest is",
        "Sink",
        "You lake it",
        "Emit da feet"
    };

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
            button.OnInteractEnded += delegate { buttonRelease(pressedButton); };
        }
    }

    // Use this for initialization
    void Start()
    {
        phrase1 = UnityEngine.Random.Range(0, 38);
        phrase2 = UnityEngine.Random.Range(0, 38);
        texts[0].text = phrases[phrase1];
        texts[1].text = phrases[phrase2];
        Debug.LogFormat("[Bone Apple Tea #{0}] The top phrase is: \"{1}\"", moduleId, phrases[phrase1]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The bottom phrase is: \"{1}\"", moduleId, phrases[phrase2]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The correct left character is:{1}", moduleId, characters[phrase1]);
        Debug.LogFormat("[Bone Apple Tea #{0}] The correct right character is:{1}", moduleId, characters[phrase2]);
    }

    IEnumerator HoldChecker()
    {
        yield return new WaitForSeconds(.4f);
        holding = true;
        backHere:
        if (holding)
        {
            switch (btn)
            {
                case 0: leftChar = (leftChar + 1) % 38; texts[2].text = characters[leftChar]; break;
                case 1: leftChar = (leftChar + 37) % 38; texts[2].text = characters[leftChar]; break;
                case 2: rightChar = (rightChar + 1) % 38; texts[3].text = characters[rightChar]; break;
                case 3: rightChar = (rightChar + 37) % 38; texts[3].text = characters[rightChar]; break;
                case 4: break;
            }
            yield return new WaitForSeconds(0.075f);
            goto backHere;
        }
    }

    void buttonPress(KMSelectable button)
    {
        if (moduleSolved == false)
        {
            button.AddInteractionPunch();
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (button == buttons[4])
            {
                if (leftChar == phrase1 && rightChar == phrase2)
                {
                    Debug.LogFormat("[Bone Apple Tea #{0}] The characters you submitted are{1}and{2}, which are correct. Module solved.", moduleId, characters[leftChar], characters[rightChar].Substring(0, (characters[rightChar].Length - 1)));
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                }
                else
                {
                    Debug.LogFormat("[Bone Apple Tea #{0}] The characters you submitted are{1}and{2}, which are incorrect. Module striked.", moduleId, characters[leftChar], characters[rightChar].Substring(0, (characters[rightChar].Length - 1)));
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else if (button == buttons[0])
            {
                leftChar = (leftChar + 1) % 38;
                texts[2].text = characters[leftChar];
                btn = 0;
            }
            else if (button == buttons[1])
            {
                leftChar = (leftChar + 37) % 38;
                texts[2].text = characters[leftChar];
                btn = 1;
            }
            else if (button == buttons[2])
            {
                rightChar = (rightChar + 1) % 38;
                texts[3].text = characters[rightChar];
                btn = 2;
            }
            else if (button == buttons[3])
            {
                rightChar = (rightChar + 37) % 38;
                texts[3].text = characters[rightChar];
                btn = 3;
            }
            buttonHold = StartCoroutine(HoldChecker());
        }
    }

    void buttonRelease(KMSelectable button)
    {
        holding = false;
        StopAllCoroutines();
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit 8T [Submits the specified pair of characters, in this example '8' and 'T' respectively]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        string[] parameters = command.Split(' ');
        var m = Regex.Match(parameters[0], @"^\s*(submit|cont)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (!m.Success || parameters.Length != 2 || parameters[1].Length != 2)
            yield break;
        string str = "0123456789abcdefghijklmnopqrstuvwxyz&$";
        int targetA = str.IndexOf(parameters[1][0]);
        int targetB = str.IndexOf(parameters[1][1]);
        if (new[] { targetA, targetB }.Contains(-1))
            yield break;
        yield return null;
        yield return "solve";
        yield return "strike";
        if (leftChar != targetA)
        {
            var distance = (Math.Abs(leftChar - targetA) + 19) % 38 - 19;
            if (leftChar > targetA)
                distance *= -1;
            if (distance > 0)
            {
                buttons[0].OnInteract();
                while (leftChar != targetA)
                    yield return null;
                buttons[0].OnInteractEnded();
                yield return new WaitForSeconds(0.05f);
            }
            else if (distance < 0)
            {
                buttons[1].OnInteract();
                while (leftChar != targetA)
                    yield return null;
                buttons[1].OnInteractEnded();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (rightChar != targetB)
        {
            var distance = (Math.Abs(rightChar - targetB) + 19) % 38 - 19;
            if (rightChar > targetB)
                distance *= -1;
            if (distance > 0)
            {
                buttons[2].OnInteract();
                while (rightChar != targetB)
                    yield return null;
                buttons[2].OnInteractEnded();
                yield return new WaitForSeconds(0.05f);
            }
            else if (distance < 0)
            {
                buttons[3].OnInteract();
                while (rightChar != targetB)
                    yield return null;
                buttons[3].OnInteractEnded();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        buttons[4].OnInteract();
        yield return null;
        buttons[4].OnInteractEnded();
        yield break;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (leftChar != phrase1)
        {
            var distance = (Math.Abs(leftChar - phrase1) + 19) % 38 - 19;
            if (leftChar > phrase1)
                distance *= -1;
            if (distance > 0)
            {
                buttons[0].OnInteract();
                while (leftChar != phrase1)
                    yield return null;
                buttons[0].OnInteractEnded();
            }
            else if (distance < 0)
            {
                buttons[1].OnInteract();
                while (leftChar != phrase1)
                    yield return null;
                buttons[1].OnInteractEnded();
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (rightChar != phrase2)
        {
            var distance = (Math.Abs(rightChar - phrase2) + 19) % 38 - 19;
            if (rightChar > phrase2)
                distance *= -1;
            if (distance > 0)
            {
                buttons[2].OnInteract();
                while (rightChar != phrase2)
                    yield return null;
                buttons[2].OnInteractEnded();
            }
            else if (distance < 0)
            {
                buttons[3].OnInteract();
                while (rightChar != phrase2)
                    yield return null;
                buttons[3].OnInteractEnded();
            }
            yield return new WaitForSeconds(0.1f);
        }
        buttons[4].OnInteract();
        yield return null;
        buttons[4].OnInteractEnded();
    }
}