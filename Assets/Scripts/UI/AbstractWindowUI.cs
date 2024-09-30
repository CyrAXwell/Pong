using System;
using UnityEngine;

public abstract class AbstractWindowUI : MonoBehaviour, IShowWindowUI, IHideWindowUI
{
    public event EventHandler OnCloseWindow;

    public virtual void Show()
    {
        gameObject.SetActive(true); 
    }

    public virtual void Hide()
    {
       gameObject.SetActive(false); 
    }  

    public virtual void NotifyOnHide()
    {
        OnCloseWindow?.Invoke(this, EventArgs.Empty);
    }
}
