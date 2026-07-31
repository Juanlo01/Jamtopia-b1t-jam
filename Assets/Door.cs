using UnityEngine;
using UnityEngine.Rendering;

public class Door : MonoBehaviour, IInteractable
{

    private SpriteRenderer sr;
    private Color originalcolor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalcolor = sr.color;
    }

    public void Interact()
    {
        OpenDoor();
    }

    public void OnNotTouchingPlayer()
    {
        sr.color = originalcolor;
    }

    public void OnTouchingPlayer()
    {
        sr.color = Color.blue;
    }

    void OpenDoor(){
        Destroy(gameObject);
    }
}
