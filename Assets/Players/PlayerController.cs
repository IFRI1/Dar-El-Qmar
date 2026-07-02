using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"{gameObject.name} has NO Animator!");
        }
        else
        {
            Debug.Log($"{gameObject.name} Animator found.");
        }
    }

    public void PlayHands()
    {
        animator.SetTrigger("Hands");
    }

    public void PlaySalute()
    {
        Debug.Log($"PlaySalute called on {gameObject.name}");

        animator.SetTrigger("Salute");
    }

    public void PlayHelloSir()
    {
        animator.SetTrigger("HelloSir");
    }

    public void PlayHelloMadam()
    {
        animator.SetTrigger("HelloMadam");
    }

    public void PlayLose()
    {
        animator.SetTrigger("Lose");
    }
}
