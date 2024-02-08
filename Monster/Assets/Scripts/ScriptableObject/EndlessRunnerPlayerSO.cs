using UnityEngine;

[CreateAssetMenu(fileName = "EndlessRunnerPlayerSO", menuName = "ScriptableObject/EndlessPlayer")]
public class EndlessRunnerPlayerSO : ScriptableObject
{
    public float health;
    public float speed;
    public float ccRecoverTime;
    public float acceleration;
    public float maxAcceleration;
}
