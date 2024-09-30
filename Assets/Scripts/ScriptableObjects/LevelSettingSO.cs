using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettingSO", menuName = "ScriptableObjects/LevelSettingSO")]
public class LevelSettingSO : ScriptableObject
{
    [SerializeField] private int[] _goals;
    [SerializeField] private PongAIDifficultySO[] _difficulty;

    public IEnumerable<int> Goals => _goals;
    public IEnumerable<PongAIDifficultySO> Difficulty => _difficulty;
}
