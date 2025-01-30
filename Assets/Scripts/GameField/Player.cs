using System;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public event EventHandler OnBallCollide;
    public event EventHandler OnPlayerDestroy;

    [SerializeField] private Material[] _materials; 

    private PongGameManager _pongGameManager;
    private int _index;
    private bool _isAlredyInitialize;
    private bool _isNetworkObject;

    public int GetIndex() => _index;

    public void Initialize(int index)
    {
        _index = index;
        GetComponent<MeshRenderer>().material = _materials[_index];
    }

    public void InitializeNetwork(PongGameManager pongGameManager)
    {
        _isNetworkObject = true;
        _pongGameManager = pongGameManager;
        PongGameMultipayer.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChange;
    }

    private void OnPlayerDataNetworkListChange(object sender, EventArgs e)
    {
        if (!_isAlredyInitialize)
        {
            _isAlredyInitialize = true;
            SetPlayerColor(PongGameMultipayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId));
        }
    }

    public void SetPlayerColor(int playerIndex)
    {
        GetComponent<MeshRenderer>().material = _materials[playerIndex];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            OnBallCollide?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void OnDestroy()
    {
        OnPlayerDestroy?.Invoke(this, EventArgs.Empty);
        if (_isNetworkObject)
            PongGameMultipayer.Instance.OnPlayerDataNetworkListChanged -= OnPlayerDataNetworkListChange;
    }
}
