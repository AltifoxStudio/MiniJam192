using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 0)]
public class GameConfig : ScriptableObject
{
    public int numberOfDustToGather = 15;

    [Header("Player Config")]
    [Range(1f, 3f)]
    public float maxPlayerSize = 2f;

    [Range(0.5f, 3f)]
    public float bunnyMS = 1f;

    [Range(0.5f, 3f)]
    public float minPlayerSize = 0.5f; // Relatif à la taille de référence

    [Header("Vacuum Config")]
    [Range(0f, 180f)]
    public float vaccumAngle = 90; // Degrés
    [Range(0.5f, 5f)]
    public float vaccumRadius = 0.5f; // Metres
    [Range(0.5f, 3f)]
    public float vacuumSpeed = 1.2f;
}