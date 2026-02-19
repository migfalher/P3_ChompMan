using Unity.VisualScripting;
using UnityEngine;

public static class GlobalVariables
{
    // variables
    private static int dificulty = 0;
    private static bool victory = false;
    private static int spheresCounter = 0;
    private static int ghostCounter = 0;
    private static int killsCounter = 0;
    private static string timeText = "";

    // getters and setters
    public static int getDificulty() { return dificulty; }
    public static void setDificulty(int _dificulty) { dificulty = _dificulty; }
    public static bool getVictory() { return victory; }
    public static void setVictory(bool _victory) { victory = _victory; }
    public static int getSpheresCounter() { return spheresCounter; }
    public static void fulfillSpheresCounter(int _ammount) { spheresCounter = _ammount; }
    public static int getGhostCounter() { return ghostCounter; }
    public static void subtractGhostCounter() { ghostCounter = (ghostCounter < 0) ? 0 : ghostCounter - 1; }
    public static void updateGhostCounter(int _ammount) { ghostCounter = _ammount; }
    public static void subtractSpheresCounter() { spheresCounter = (spheresCounter < 0) ? 0 : spheresCounter - 1; }
    public static int getKillsCounter() { return killsCounter; }
    public static void addKillsCounter() { killsCounter++; }
    public static string getTimeText() { return timeText; }
    public static void setTimeText(string _timeText) { timeText = _timeText; }

    // reset all values
    public static void reset()
    {
        dificulty = 0;
        victory = false;
        spheresCounter = 0;
        ghostCounter = 0;
        killsCounter = 0;
        timeText = "";
    }
}
