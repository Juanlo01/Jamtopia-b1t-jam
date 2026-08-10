using UnityEngine;

// Forwards OnTriggerEnter back to the owning TeleportLink. Unity only delivers trigger callbacks
// to components on the same GameObject as the Collider, so TeleportLink adds one of these to each
// of triggerA/triggerB's GameObjects at runtime (see TeleportLink.Wire) - it isn't meant to be
// added by hand.
[DisallowMultipleComponent]
public class TeleportLinkTrigger : MonoBehaviour
{
    private TeleportLink link;
    private TeleportSide side;

    public void Init(TeleportLink owner, TeleportSide ownerSide){
        link = owner;
        side = ownerSide;
    }

    private void OnTriggerEnter(Collider other){
        if(link != null) link.NotifyTriggerEntered(side, other);
    }
}
