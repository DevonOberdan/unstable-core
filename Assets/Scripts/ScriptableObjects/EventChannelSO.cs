using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableObjectLibrary {

    //[CreateAssetMenu(fileName = "EventChannelSO", menuName = "EventChannelSO", order=0)]
    public class EventChannelSO : ScriptableObject
    {
        public Action<DataConfigSO> OnRequestEvent;

        public void Raise(DataConfigSO data)
        {
            OnRequestEvent?.Invoke(data);
        }
    }
}
