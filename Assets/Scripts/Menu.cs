using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    private GameObject InGame;
    private GameObject Die;
    private GameObject Player;
    private bool InGameMenu = false;
    private bool DieMenu = false;
    private List<Transform> InGameButton;
    private List<Transform> DieButton;
    private Text Score;
    private int InGameIndex = 0;
    private int DieIndex = 0;
    private float t;
    private bool next;
    private Color button = new Color(20/255f, 86/255f, 112/255f);
    private Color select = new Color(1/255f, 132/255f, 183/255f);
    // Use this for initialization
    void Start () {
        InGame = GameObject.Find("In game menu");
        Die = GameObject.Find("Die menu");
        Player = GameObject.Find("Player");
        InGame.SetActive(false);
        Die.SetActive(false);
        InGameButton = new List<Transform>();
        DieButton = new List<Transform>();
        foreach (Transform child in InGame.transform)
        {
            if (child.name != "White" && child.name != "Paused")
                InGameButton.Add(child);
        }
        foreach (Transform child in Die.transform)
        {
            if (child.name == "Score") Score = child.gameObject.GetComponent<Text>();
            else if (child.name != "White" && child.name != "Game Over") DieButton.Add(child);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Utility.die != 0 && !DieMenu)
        {
            Die.SetActive(true);
            GameObject gg = GameObject.Find("Game Over");
            if (Utility.die == 1)
            {
                gg.GetComponent<Text>().text = "Try to jump!";
            } else
            {
                gg.GetComponent<Text>().text = "Avoid obstacles!";
            }
            DieMenu = true;
            Time.timeScale = 0;
            Player.GetComponent<PlayControl>().enabled = false;
            Score.text = "Score: " + Utility.score;
        }
        if (DieMenu)
        {
            DieButton[DieIndex].GetComponent<Image>().color = button;
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)[1] > 0.5f && next)
            {
                if (DieIndex == 0) DieIndex = 0;
                else DieIndex -= 1;
                t = 0;
                next = false;
            } else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)[1] < -0.5f && next)
            {
                if (DieIndex == 2) InGameIndex = 2;
                else DieIndex += 1;
                t = 0;
                next = false;
            }
            DieButton[DieIndex].GetComponent<Image>().color = select;

            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) && next)
            {
                t = 0;
                next = false;
                switch (DieIndex)
                {
                    case 0:
                        restart();
                        break;
                    case 1:
                        mainMenu();
                        break;
                    case 2:
                        Utility.Exit();
                        break;
                }
            }
            if (!next)
            {
                t += 1;
                if (t > 30) next = true;
            }
        }


        if (!DieMenu && !InGameMenu && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            InGame.SetActive(true);
            InGameMenu = true;
            Time.timeScale = 0;
            Player.GetComponent<PlayControl>().enabled = false;
        }
        if (InGameMenu)
        {
            InGameButton[InGameIndex].GetComponent<Image>().color = button;
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)[1] > 0.5f && next)
            {
                if (InGameIndex == 0) InGameIndex = 0;
                else InGameIndex -= 1;
                t = 0;
                next = false;
            } else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)[1] < -0.5f && next)
            {
                if (InGameIndex == 3) InGameIndex = 3;
                else InGameIndex += 1;
                t = 0;
                next = false;
            }
            InGameButton[InGameIndex].GetComponent<Image>().color = select;

            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) && next)
            {
                t = 0;
                next = false;
                switch (InGameIndex)
                {
                    case 0:
                        InGame.SetActive(false);
                        InGameMenu = false;
                        Time.timeScale = 1;
                        Player.GetComponent<PlayControl>().enabled = true;
                        break;
                    case 1:
                        restart();
                        break;
                    case 2:
                        mainMenu();
                        break;
                    case 3:
                        Utility.Exit();
                        break;
                }
            }
            if (!next)
            {
                t += 1;
                if (t > 30) next = true;
            }
        }
    }

    private void restart ()
    {
        Time.timeScale = 1;
        Utility.isTutorial = false;
        Utility.noMenu = true;
        SceneManager.LoadScene(0);
    }

    private void mainMenu ()
    {
        Time.timeScale = 1;
        Utility.noMenu = false;
        SceneManager.LoadScene(0);
    }
}
