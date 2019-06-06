using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour {

    public int startState;
    public int pathsEachLevel;
    public GameObject Tutorial;
    public GameObject[] GunLevels;
    public GameObject[] DaggerLevels;
    public GameObject[] CannonLevels;
    public GameObject GunTransition;
    public GameObject DaggerTransition;
    public GameObject CannonTransition;

    private Vector3 InitialPosition = new Vector3(10, 16, 100);
    private GameObject pathToDestroy;
    private int lastIndex = 0;
    private int gameSubState = 0;

    // Use this for initialization
    void Start () {
        resetState();
        if (Utility.noMenu)
        {
            Destroy(GameObject.Find("MenuWall"));
            GetComponent<LevelGenerator>().generateFirstPath();
        }
    }

    public void generateFirstPath()
    {
        if (Utility.isTutorial)
        {
            pathToDestroy = Instantiate(Tutorial);
            pathToDestroy.name = "Path0";
            pathToDestroy.transform.position = InitialPosition;
            InitialPosition = new Vector3(10, 16, 2110);
        }
        GameObject[] chosenPaths = choosePaths();
        GameObject floor = Instantiate(chosenPaths[Random.Range(0, chosenPaths.Length)]);
        floor.name = "Path1";
        floor.transform.position = InitialPosition;
        gameSubState++;
    }

    private void generateLevel(GameObject chosenPath, Transform lastPath, int index)
    {
        GameObject floor = Instantiate(chosenPath);
        floor.name = "Path" + index.ToString();
        floor.transform.position = lastPath.position + new Vector3(0, 0, 200);
        pathToDestroy = lastPath.gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Level" && !Utility.isTutorial)
        {
            Transform path = collision.transform.parent;
            int index = int.Parse(path.name.Substring(4));
            if (index > lastIndex)
            {
                Destroy(pathToDestroy);
                GetComponent<PlayControl>().incrementSpeed();

                lastIndex = index;
                int nextIndex = index + 1;

                if (gameSubState == pathsEachLevel)
                {
                    GameObject chosenPath = null;
                    switch (Utility.gameState)
                    {
                        case 0:
                            chosenPath = DaggerTransition;
                            break;
                        case 1:
                            chosenPath = CannonTransition;
                            break;
                        case 2:
                            chosenPath = GunTransition;
                            break;
                        default:
                            break;
                    }
                    generateLevel(chosenPath, path, nextIndex);
                }
                else
                {
                    if (gameSubState > pathsEachLevel)
                    {
                        Utility.gameState++;
                        Utility.gameState %= 3;
                        gameSubState = 0;
                    }
                    GameObject[] chosenPaths = choosePaths();
                    generateLevel(chosenPaths[Random.Range(0, chosenPaths.Length)], path, nextIndex);
                }
                gameSubState++;
            }
        }
    }

    private GameObject[] choosePaths()
    {
        GameObject[] chosenPaths = null;
        switch (Utility.gameState)
        {
            case 0:
                chosenPaths = GunLevels;
                break;
            case 1:
                chosenPaths = DaggerLevels;
                break;
            case 2:
                chosenPaths = CannonLevels;
                break;
            default:
                break;
        }
        return chosenPaths;
    }

    public void resetState()
    {
        Utility.gameState = startState;
    }
}


    
