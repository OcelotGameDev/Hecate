using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfMonster {Level1, Level2, Level3, Boss};

public class BaseEnemy : MonoBehaviour, IHittable
{
    public float speed, groundSight, sightLength, distanceToChase, distanceToAtk, maxHp;
    public Transform player, nose;
    public LayerMask groundLm, playerLm, obstaclesLm;
    public float timeTimer;
    public TypesOfMonster monsterType;
    bool isDead=> currentHp <= 0;
    bool movingRight = true;
    float currentHp, distToPoint;

    RaycastHit2D hitToGround;

    LeonAIBrain brainRef;
    Rigidbody2D rBody;
    Collider2D hitToPlayer;
    

    private void Awake()
    {
        brainRef = new LeonAIBrain();
    }

    void OnEnable()
    {
        SetMonsterType();
        currentHp = maxHp;
    }

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }
    
    public void Hit(int damage = -1)
    {
        if (isDead)
        {
            this.gameObject.SetActive(false);
        }
        else { currentHp -= damage; }
    }

    void SetMonsterType()
    {
        switch (monsterType)
        {
            case TypesOfMonster.Level1:{
                    /* Set values related to EASYgrade Enemy*/
                    speed = 2;
            }
                break;

            case TypesOfMonster.Level2:{
                    /* Set values related to MIDgrade Enemy*/
                    speed = 5;
                }
                break;

            case TypesOfMonster.Level3:{
                    /* Set values related to HIGHgrade Enemy*/
                    speed = 10;
                }
                break;

            case TypesOfMonster.Boss:{
                    /* Set values related to BOSS Enemy*/
                    speed = 15;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        ToSeePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        hitToGround = Physics2D.Raycast(nose.position, Vector2.down, groundSight, groundLm);
    }

    void ToSeePlayer()
    {
        hitToPlayer = Physics2D.OverlapCircle(transform.position, sightLength, playerLm);

        if (hitToPlayer != null)
        {
            brainRef.Brain(ToChase);

            /*distToPoint = Vector2.Distance(transform.position, player.position);
            if (distToPoint<=distanceToChase)
            {
                brainRef.Brain(ToChase);
            }
            else { brainRef.Brain(ToPatrol); Debug.Log("Player Not Seen"); }*/
        }
        else { brainRef.Brain(ToPatrol); Debug.Log("Player Not Seen"); }
    }

    void ToIdle()
    {
        timeTimer = 5;
        timeTimer -= 1 * Time.deltaTime;
        if (timeTimer <= 0)
        {
            timeTimer = 0;
            brainRef.Brain(ToPatrol);
        }
    }

    public void TimeCounter()
    {
        timeTimer -= 1 * Time.deltaTime;
        if (timeTimer <= 0)
        {
            timeTimer = 0;
            brainRef.Brain(ToIdle);
        }
    }

    public void ToPatrol()
    {
        Debug.Log("Patrol");
        if (hitToGround.collider != false)
        {
            if (movingRight)
            {
                rBody.velocity = new Vector2(speed, rBody.velocity.y);
            }
            else
            {
                rBody.velocity = new Vector2(-speed, rBody.velocity.y);
            }
        }else{
            movingRight = !movingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void ToChase()
    {
        Debug.Log("Chase");
        
        float test;
        test = Mathf.Lerp(transform.position.x, player.position.x, speed*Time.deltaTime);
        transform.position = new Vector3 (test, this.transform.position.y, this.transform.position.z);
        distToPoint = Vector2.Distance(transform.position, player.position);

        if (distToPoint<=distanceToAtk)
        {
            brainRef.Brain(ToAttack);
        }
    }

    void ToAttack()
    {
        Debug.Log("Attack");
    }
}