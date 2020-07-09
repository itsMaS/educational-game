using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : ScriptableObject
{
    Action OnChange;

    private void OnValidate()
    {
        Validation();
        OnChange?.Invoke();
    }

    public void Subscribe(Action method)
    {
        OnChange += method;
    }
    public void Unsubscribe(Action method)
    {
        OnChange -= method;
    }

    public virtual void Validation()
    {

    }
}
