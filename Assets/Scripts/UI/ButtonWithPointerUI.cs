using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithPointerUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private const string AUDIO_TAG = "Audio";

    [SerializeField] private GameObject _pointer;

    private bool _isManualSelect;
    private AudioManager _audioManager;
    
    private void Awake()
    {
        _pointer.SetActive(false);

        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        GetComponent<Button>().onClick.AddListener( () => { _audioManager.PlaySFX(_audioManager.ButtonClick); });
    }

    public void ManualSelect()
    {
        _isManualSelect = true;
    } 
    
    public void OnSelect(BaseEventData eventData)
    {
        _pointer.gameObject.SetActive(true);
        if (!_isManualSelect)
            _audioManager.PlaySFX(_audioManager.ButtonSelect);

        _isManualSelect = false;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _pointer.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _pointer.gameObject.SetActive(false);
    }
}
