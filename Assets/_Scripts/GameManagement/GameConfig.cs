using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 0)]
public class GameConfig : ScriptableObject
{
    public int numberOfDustToGather = 15;

    [Header("Player Config")]
    public float maxPlayerSize = 2f;
    public float dashCooldown = 1f;

    public float bunnyMS = 1f;
    public float bunnyJumpSpeed = 3f;
    public float bunnyDashSpeed = 3f;
    public float bunnyStartDustAmount = 100f;
    public float bunnyMaxDustAmount = 200f;
    public float dustSpendMoving = 0.1f;
    public float minPlayerSize = 0.5f; // Relatif à la taille de référence

    [Header("Dust Ball Config")]
    public float spawnChanceEachTick = 0.05f;

    [Header("Vacuum Config")]
    [Range(0f, 180f)]
    public float vaccumAngle = 90; // Degrés
    [Range(0.5f, 5f)]
    public float vaccumRadius = 0.5f; // Metres
    [Range(0.5f, 3f)]
    public float vacuumSpeed = 1.2f;
    public float vacuumHeight = 0.3f;
    public float vacuumMaxDust = 250f;

    [Header("Camera Config")]
    public float distanceToShowVacuum = 10;
}