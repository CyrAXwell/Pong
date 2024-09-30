using UnityEngine;

[CreateAssetMenu(fileName = "PongAIDifficultySO", menuName = "ScriptableObjects/PongAIDifficultySO")]
public class PongAIDifficultySO : ScriptableObject
{
    [SerializeField] private GameDifficulty _difficulty;
    [SerializeField] private float _speedMultiplier;

    public GameDifficulty Difficulty => _difficulty;
    public float SpeedMultiplier => _speedMultiplier;
}
