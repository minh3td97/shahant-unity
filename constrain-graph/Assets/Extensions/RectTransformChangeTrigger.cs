using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class RectTransformChangeTrigger : UIBehaviour
{
    [SerializeField] UnityEvent<RectTransform> _onRectTransformDimensionsChangeEvent;

    [System.NonSerialized] RectTransform _rect;
    RectTransform Rect
    {
        get
        {
            if (_rect == null) _rect = GetComponent<RectTransform>();
            return _rect;
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        InvokeRectChangeDimension();
    }

    [ContextMenu("Invoke Rect Change Dimension")]
    public void InvokeRectChangeDimension()
    {
        _onRectTransformDimensionsChangeEvent?.Invoke(Rect);
    }

}
