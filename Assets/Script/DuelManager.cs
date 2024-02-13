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
    [SerializeField] GameObject doorScene;

    [SerializeField] EnnemyMoves[] sequence;
    [SerializeField] float timeBetweenMoves;

    [SerializeField] float flashTime;
    [SerializeField] GameObject whiteFlash;
    [SerializeField] GameObject redFlash;
    [SerializeField] GameObject ennemyKatana;
    [SerializeField] GameObject ennemyKatana_attack;

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
        doorScene.SetActive(false);
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
            yield return new WaitForSeconds(timeBetweenMoves / 2);
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
            yield return new WaitForSeconds(timeBetweenMoves/2);
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
            flash.SetActive(flash);
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
            yield return new WaitForSeconds(timeBetweenMoves / 2);
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
                    StartCoroutine(EnemyKatanaStance(1));
                    enemyAttack = true;
                    break;
                case EnnemyMoves.DoubleLightAttack:
                    currentState = EnnemyMoves.DoubleLightAttack;
                    StartCoroutine(EnemyKatanaStance(2));
                    enemyAttack = true;
                    break;
                case EnnemyMoves.HeavyAttack:
                    currentState = EnnemyMoves.HeavyAttack;
                    StartCoroutine(EnemyKatanaStance(1));
                    enemyAttack = true;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(timeBetweenMoves/2);
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

    IEnumerator EnemyKatanaStance(int num)
    {
        for (int i = 0; i < num; i++)
        {
            ennemyKatana.SetActive(false);
            ennemyKatana_attack.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            ennemyKatana.SetActive(true);
            ennemyKatana_attack.SetActive(false);
            yield return new WaitForSeconds(flashTime);
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
