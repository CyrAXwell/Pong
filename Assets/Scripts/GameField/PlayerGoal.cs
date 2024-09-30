using System;
using UnityEngine;

public class PlayerGoal : MonoBehaviour
{
    public event EventHandler<OnGoalEventArgs> OnGoal;  
    public class OnGoalEventArgs : EventArgs
    {
        public GoalSide goalSide;
    }

    [SerializeField] private GoalSide _goalSide;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            OnGoal?.Invoke(this, new OnGoalEventArgs { goalSide = _goalSide});
        }
    }
}
