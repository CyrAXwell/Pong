using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event EventHandler OnBallCollide;

    [SerializeField] private Material[] _materials; 
    [SerializeField] private Light[] _lights;

    private int _index;

    public void Initialize(int index)
    {
        _index = index;

        foreach (Light light in _lights)
                light.gameObject.SetActive(false);
        
        _lights[_index].gameObject.SetActive(true);
        GetComponent<MeshRenderer>().material = _materials[_index];
    }

    public int GetIndex() => _index;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            ball.OnPlayerBounce(this);

            OnBallCollide?.Invoke(this, EventArgs.Empty);
        }
        
    }
}
