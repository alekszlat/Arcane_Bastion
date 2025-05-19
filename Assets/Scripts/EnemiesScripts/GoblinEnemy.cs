using Unity.VisualScripting;
using UnityEngine;

public class GoblinEnemy : EnemyBehaviour // extends basic enemy behavior
{
    [SerializeField] GameObject goblinBase; // Prefab reference for the goblin
    Animator animator; // Animator component reference

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        animator = goblinBase.GetComponent<Animator>(); // Get the Animator component from the goblin prefab
    }

    // Override the Update method to fix CS0114
    public override void Update()
    {
        base.Update(); // Call the base class Update method
        AnimationController(); // Add meaningful logic to avoid UNT0001
    }

    void AnimationController()
    {
        if(isHitAnimation)
        {
            animator.SetBool("isHitting", false);
            animator.SetBool("Walking", true);
            // Unable the animator
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true; // Enable the animator
        }

        if (isAttackAnimation)
        {
            animator.SetBool("isHitting", true); // Trigger the attack animation
        }
    }
}
