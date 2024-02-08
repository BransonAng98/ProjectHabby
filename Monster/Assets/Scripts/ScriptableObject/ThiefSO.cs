using UnityEngine;

[CreateAssetMenu(fileName = "ThiefSO", menuName = "ScriptableObject/EndlessThief")]
public class ThiefSO : ScriptableObject
{
    public float health;
    public float speed;
    public float acceleration;
    public float maxAcceleration;
}
