using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithPointerUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private const string AUDIO_TAG = "Audio";

    [SerializeField] private GameObject _pointer;

    private bool _isManualSelect;

    private void Awake()
    {
        _pointer.SetActive(false);
        
        GetComponent<Button>().onClick.AddListener( () => { AudioManager.Instance.PlaySFX(AudioManager.Instance.ButtonClick); });
    }

    public void ManualSelect()
    {
        _isManualSelect = true;
    }
    
    public void OnSelect(BaseEventData eventData)
    {   
        _pointer.gameObject.SetActive(true);
        if (!_isManualSelect && !IsMouseClick())
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.ButtonSelect);
        }

        _isManualSelect = false;
    }

    private bool IsMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            var res = results.Where(h => h.gameObject.TryGetComponent<Button>(out Button button)).FirstOrDefault();
            if(res.gameObject != null)
                return true;
        }
        return false;
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
