using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIBase
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosedPopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
