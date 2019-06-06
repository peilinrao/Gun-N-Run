using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {
    public static int score;
    public static bool isTutorial;
    public static int gameState;
    public static int die;
    public static bool noMenu;

    public static void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
