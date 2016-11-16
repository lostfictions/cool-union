using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TriggerMessage : MonoBehaviour
{
    public event Action<TriggerMessage, Collider> TriggerEnterAction;
    public event Action<TriggerMessage, Collider> TriggerStayAction;
    public event Action<TriggerMessage, Collider> TriggerExitAction;

    void OnTriggerEnter(Collider other)
    {
        if(TriggerEnterAction != null) {
            TriggerEnterAction(this, other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(TriggerStayAction != null) {
            TriggerStayAction(this, other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(TriggerExitAction != null) {
            TriggerExitAction(this, other);
        }
    }
}
