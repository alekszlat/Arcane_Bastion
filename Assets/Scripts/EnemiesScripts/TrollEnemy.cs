using UnityEngine;

public class TrollEnemy : EnemyBehaviour//extends basic enemy behavior
{
    [SerializeField] GameObject TrollBase;
    Animator animator;

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        animator = TrollBase.GetComponent<Animator>(); // Get the Animator component from the goblin prefab
    }

    // Override the Update method to fix CS0114
    public override void Update()
    {
        base.Update(); // Call the base class Update method
        AnimationController(); // Add meaningful logic to avoid UNT0001
    }


    void AnimationController()
    {
        if (isHitAnimation)
        {
            animator.SetBool("isHitting", false);
            animator.SetBool("isWalking", true);
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
