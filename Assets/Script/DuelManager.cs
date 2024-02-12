using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuelManager : MonoBehaviour
{
    public enum EnnemyMoves 
    {
        LightAttack,
        DoubleLightAttack,
        HeavyAttack,
        Pause,
    }

    [SerializeField] EnnemyMoves[] sequence;
    [SerializeField] float timeBetweenMoves;

    [SerializeField] float flashTime;
    [SerializeField] GameObject whiteFlash;
    [SerializeField] GameObject redFlash;

    AudioSource audioSource;
    [SerializeField] AudioClip tick;
    [SerializeField] AudioClip carillon;
    bool playingSequence;
    bool canAttack;
    bool enemyAttack;
    bool actionPerformed;
    bool dodging;
    int dodgingCD;
    EnnemyMoves currentState;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(EnnemySequence());
        canAttack = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator EnnemySequence()
    {
        playingSequence = true;
        canAttack = false;
        foreach (EnnemyMoves move in sequence)
        {
            audioSource.PlayOneShot(tick);
            switch (move)
            {
                case EnnemyMoves.Pause:
                    break;
                case EnnemyMoves.LightAttack:
                    StartCoroutine(Flash(whiteFlash, 1));
                    break;
                case EnnemyMoves.DoubleLightAttack:
                    StartCoroutine(Flash(whiteFlash, 2));
                    break;
                case EnnemyMoves.HeavyAttack:
                    StartCoroutine(Flash(redFlash, 1));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(timeBetweenMoves);
        }
        audioSource.PlayOneShot(carillon);
        playingSequence = false;
        StartCoroutine(PlayerSequence());
        yield return null;
    }

    IEnumerator Flash(GameObject flash, int num)
    {
        for (int i = 0; i < num; i++)
        {
            flash.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            flash.SetActive(false);
            yield return new WaitForSeconds(flashTime);
        }
        yield return null;
    }

    IEnumerator PlayerSequence()
    {
        foreach (EnnemyMoves move in sequence)
        {
            audioSource.PlayOneShot(tick);
            if (dodgingCD <= 0)
            {
                dodging = false;
            }
            actionPerformed = false;
            switch (move)
            {
                case EnnemyMoves.Pause:
                    currentState = EnnemyMoves.Pause;
                    break;
                case EnnemyMoves.LightAttack:
                    currentState = EnnemyMoves.LightAttack;
                    enemyAttack = true;
                    break;
                case EnnemyMoves.DoubleLightAttack:
                    currentState = EnnemyMoves.DoubleLightAttack;
                    enemyAttack = true;
                    break;
                case EnnemyMoves.HeavyAttack:
                    currentState = EnnemyMoves.HeavyAttack;
                    enemyAttack = true;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(timeBetweenMoves);
            if (enemyAttack)
            {
                print("hurt");
            }
            if(dodgingCD > 0)
            {
                dodgingCD--; 
            }
        }
        yield return null;
    }

    //player actions
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {
            print("attack");
            if (canAttack)
            {
                print("hit");
            }
            if(enemyAttack == true)
            {
                print("hurt");
            }
            actionPerformed = true;
        }
    }
    public void Block(InputAction.CallbackContext context)
    {
        if (context.canceled && !dodging && !actionPerformed)
        {
            if (currentState == EnnemyMoves.LightAttack)
            {
                print("block");
                enemyAttack = false;
            }
            actionPerformed = true;
        }
        if (context.performed && !dodging)
        {
            if (currentState == EnnemyMoves.DoubleLightAttack)
            {
                print("double block");
                enemyAttack = false;
            }
            actionPerformed = true;
        }
        
    }
    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {
            if (currentState == EnnemyMoves.LightAttack || currentState == EnnemyMoves.HeavyAttack)
            {
                print("dodge");
                enemyAttack = false; 
            }
            dodging = true;
            dodgingCD = 2;
            actionPerformed = true;
        }
    }
}
