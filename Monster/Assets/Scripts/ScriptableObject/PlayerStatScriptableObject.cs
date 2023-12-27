using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObject/Player")]
public class PlayerStatScriptableObject : ScriptableObject
{
    public int maxhealth = 100;
    public int health = 100;
    public float speed = 1f;

    public float attackDamage = 10f;
    public float stepDamage = 10f;
    public float attackRange = 5f;
    public float attackSpeed = 0.8f;
    public int ultimateDamage = 0;
    public float maxUltimateCharge = 0.0f;

    public int incomePerSecond = 1;
    public int incomeEarned = 0;
    public int Storedincome = 0;

    public int setUltimate;
    public int ultimateLevel;
    public int upgradeLevel;
    public int levelProgress;

    private void OnEnable()
    {
        health = maxhealth;
    }
}
