using UnityEngine;
using UnityEngine.UI;

public class FinalPanelBehaviour : MonoBehaviour
{
    int idleHash = Animator.StringToHash("Idle");

    Animator anim;
    // Clase que representa el panel de fin de juego
    // TODO

    public void reset()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Reset");
    }

    public void appear()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Appear");
    }

}
