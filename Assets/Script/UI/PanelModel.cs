
using UnityEngine;

public class PanelModel : MonoBehaviour
{
    public UIPanel PanelType;

    
    public void EnablePanel(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
