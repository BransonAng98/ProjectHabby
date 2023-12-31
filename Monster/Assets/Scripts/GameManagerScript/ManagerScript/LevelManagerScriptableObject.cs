using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "ScriptableObject/Level")]
public class LevelManagerScriptableObject : ScriptableObject
{
    public int worldID;
    public int buildingsInScene;
    public float baseScore;
    public float destructionLevel;
    public int cityLevel;
    public float currentDestruction;
    public bool cutscenePlayed;
    public bool tutorialPlayed;
    public bool levelselecttutorialPlayed;
    public bool upgradetutorialPlayed;
    public bool loopGame;
    public float baseTime;
    public bool landmarkTutorialPlayed;

    private void OnEnable()
    {
        currentDestruction = 0;
        destructionLevel = 0;
    }
}
