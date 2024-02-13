using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DuelManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnnemyAction
    {
        public float timing;
        public EnnemyMoves move;
    }

    public enum EnnemyMoves 
    {
        Pause,
        LightAttack,
        HeavyAttack,
        ResetAction,
        Open
    }
    [SerializeField] GameObject doorScene;
    [SerializeField] EnnemiesStats ennemyStats;
    [SerializeField] Player playerStats;

    [Header("Move Sequence")]
    [SerializeField] EnnemyAction[] sequence;
    [SerializeField] float blockWindow;
    AudioSource audioSource;
    [SerializeField] AudioClip tick;
    [SerializeField] AudioClip carillon;
    EnnemyMoves currentState;

    [Header("Ennemy Feedback")]
    [SerializeField] float flashTime;
    [SerializeField] GameObject whiteFlash;
    [SerializeField] GameObject redFlash;
    [SerializeField] GameObject ennemyKatana;
    [SerializeField] GameObject ennemyKatana_attack;
    [SerializeField] AudioClip swordHit;
    [SerializeField] AudioClip swordBlock;
    [SerializeField] ParticleSystem blood;
    [SerializeField] ParticleSystem sparks;

    bool canBlock;
    int enemyAttack;
    int ennemyHealth;
    bool actionPerformed;
    bool dodging;
    int dodgingCD;

    [Header("Player Stances")]
    [SerializeField] GameObject player;
    Vector3 playerInitialPos;
    [SerializeField] GameObject playerKatana;
    [SerializeField] GameObject playerKatana_attack;
    [SerializeField] GameObject playerKatana_block;
    [SerializeField] float dodgeDistance;

    [Header("UI")]
    [SerializeField] TMP_Text playerHealthUI;
    [SerializeField] TMP_Text ennemyHealthUI;
    void Start()
    {
        doorScene.SetActive(false);
       audioSource = GetComponent<AudioSource>();
        StartCoroutine(EnnemySequence());
        playerInitialPos = player.transform.position;
        ennemyHealth = ennemyStats.health;
        UpdateUI(playerHealthUI, playerStats.health.ToString());
        UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
    }

    IEnumerator EnnemySequence()
    {
        for (int i = 0; i < sequence.Length-1; i++)
        {
            audioSource.PlayOneShot(tick);
            yield return new WaitForSeconds(sequence[i].timing / 2);
            switch (sequence[i].move)
            {
                case EnnemyMoves.Pause:
                    break;
                case EnnemyMoves.LightAttack:
                    StartCoroutine(Flash(whiteFlash, 1));
                    break;
                case EnnemyMoves.HeavyAttack:
                    StartCoroutine(Flash(redFlash, 1));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(sequence[i].timing / 2);
        }
        audioSource.PlayOneShot(carillon);
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
        foreach (EnnemyAction action in sequence)
        {
            //audioSource.PlayOneShot(tick);

            //init action
            currentState = action.move;
            if (dodgingCD <= 0)
            {
                dodging = false;
            }
            actionPerformed = false;
            playerKatana.SetActive(true);
            playerKatana_block.SetActive(false);
            player.transform.position = playerInitialPos;
            enemyAttack = 0;
            StartCoroutine(InputActionWindow(action.timing));
            
            switch (action.move)
            {
                case EnnemyMoves.LightAttack:
                    enemyAttack = 1;
                    yield return new WaitForSeconds(action.timing / 2);
                    StartCoroutine(EnemyKatanaStance(1));
                    break;
                case EnnemyMoves.HeavyAttack:
                    //currentState = EnnemyMoves.HeavyAttack;
                    enemyAttack = 3;
                    yield return new WaitForSeconds(action.timing / 2);
                    StartCoroutine(EnemyKatanaStance(1));
                    break;
                default:
                    yield return new WaitForSeconds(action.timing / 2);
                    break;
            }

            yield return new WaitForSeconds(action.timing / 2);
        }

        yield return null;
    }

    IEnumerator InputActionWindow(float actionTime)
    {
        canBlock = false;
        yield return new WaitForSeconds((actionTime - blockWindow) / 2);
        canBlock = true;
        yield return new WaitForSeconds(blockWindow);
        canBlock = false;
        if (enemyAttack > 0)
        {
            playerStats.health -= enemyAttack * ennemyStats.damage;
            UpdateUI(playerHealthUI, playerStats.health.ToString());
            audioSource.PlayOneShot(swordHit);
            blood.Play();
            enemyAttack = 0;
        }
        if (dodgingCD > 0 && currentState != EnnemyMoves.ResetAction)
        {
            dodgingCD--;
        }
        yield return new WaitForSeconds((actionTime - blockWindow) / 2);
        
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
    IEnumerator PlayerKatanaStance(GameObject katana, int num)
    {
        for (int i = 0; i < num; i++)
        {
            playerKatana.SetActive(false);
            katana.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            playerKatana.SetActive(true);
            katana.SetActive(false);
            yield return new WaitForSeconds(flashTime);
        }
        yield return null;
    }
    //player actions
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !actionPerformed)
        {
            if (currentState == EnnemyMoves.Open)
            {
                print("hit");
                ennemyHealth -= playerStats.damage;
                UpdateUI(ennemyHealthUI, ennemyHealth.ToString());
            }
            else
            {
                playerStats.health -= enemyAttack * ennemyStats.damage;
                UpdateUI(playerHealthUI, playerStats.health.ToString());
                audioSource.PlayOneShot(swordHit);
                blood.Play();
                enemyAttack = 0;
            }
            actionPerformed = true;
        }
    }
    public void Block(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {

            if (currentState == EnnemyMoves.LightAttack && canBlock)
            {
                print("block");
                enemyAttack --;
                audioSource.PlayOneShot(swordBlock);
                sparks.Play();
            }
            actionPerformed = true;
            playerKatana.SetActive(false);
            playerKatana_block.SetActive(true);
        }        
    }
    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed && !dodging && !actionPerformed)
        {
            if (currentState == EnnemyMoves.LightAttack || currentState == EnnemyMoves.HeavyAttack)
            {
                if (canBlock)
                {
                    print("dodge");
                    enemyAttack = 0;
                }
            }
            dodging = true;
            dodgingCD = 2;
            actionPerformed = true;
            player.transform.position -= Vector3.right * dodgeDistance;
        }
    }

    void UpdateUI(TMP_Text ui, string newText)
    {
        ui.text = newText;
    }
}
