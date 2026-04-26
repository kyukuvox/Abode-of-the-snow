using UnityEngine;

public class PortalDropTarget : MonoBehaviour
{
    private PortalItemActivator portalActivator;

    void Start()
    {
        portalActivator = GetComponent<PortalItemActivator>();
    }

    public void ReceiveDroppedItem(Item item)
    {
        if (portalActivator == null)
        {
            Debug.Log("Aucun PortalItemActivator sur ce sprite !");
            return;
        }

        portalActivator.TryActivateWithItem(item);
    }
}