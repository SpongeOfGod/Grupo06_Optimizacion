using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpSettings", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class PowerUpSettings : ScriptableObject
{
    public int QuantityPerLevel = 3;

    [Header("Multiball")]
    public int BallsToSpawn = 2;

    [Header("ScoreMultiplier")]
    public float duration = 10f;
    public float scoreMultiplier = 2f;

    [Header("LongPlayer")]
    public float LongMultiplier = 2.5f;

    [Header("ShortPlayer")]
    public float ShortMultiplier = 0.5f;

    [Header("Fireball")]
    public float fireballDuration = 5f;

    [Header("SpeedIncrease")]
    public float speedIncreaseAmount = 3f;
    public float SpeedDuration = 5f;

    [Header("Explosion")]
    public float Radius = 1.5f;
}
