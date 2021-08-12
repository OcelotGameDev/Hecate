using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfMonster {Lobisomen, Cachorro, Boss};

public class BaseEnemy : MonoBehaviour, IHittable
{   
    public TypesOfMonster monsterType;
    public float distanceToChase, distanceToAtk, maxHp, maxSpeed, timeToIdle;
    public float sightRadius, sightAngle, groundSight;
    public Transform player, target, nose;
    public LayerMask groundLayer, targetMask, obstaclesLayer;
    
    bool isDead=> currentHp <= 0;
    bool movingRight = true, canDash=false;
    float speed,currentHp, distToPoint, distanceToTarget;
    Action aiAction;
    RaycastHit2D hitToGround;

    LeonAIBrain brainRef;
    Rigidbody2D rBody;

    private void Awake()
    {
        brainRef = new LeonAIBrain();
    }

    void OnEnable()
    {
        SetMonsterType();
        speed = maxSpeed;
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

    private void FixedUpdate()
    {
        brainRef.Brain(aiAction);
    }

    // Update is called once per frame
    void Update()
    {
        hitToGround = Physics2D.Raycast(nose.position, Vector2.down, groundSight, groundLayer);
    }
    
    void SetMonsterType()
    {
        switch (monsterType)
        {
            case TypesOfMonster.Lobisomen:
                {
                    /* Set values related to EASYgrade Enemy*/
                    maxSpeed = 2;
                    maxHp = 5;
                    sightRadius = 3;
                    sightAngle = 35;
                    aiAction = SightAI;
                }
                break;

            case TypesOfMonster.Cachorro:
                {
                    /* Set values related to MIDgrade Enemy*/
                    maxSpeed = 5;
                    maxHp = 10;
                    sightRadius = 4;
                    sightAngle = 35;
                    aiAction = SightAI;
                    canDash = true;
                }
                break;

            case TypesOfMonster.Boss:{
                    /* Set values related to BOSS Enemy*/
                    maxSpeed = 15;
                    maxHp = 20;
                    sightRadius = 6;
                    sightAngle = 35;
                    aiAction = SightAI;
                    canDash = true;
                }
                break;
        }
    }

    void SightAI()
    {
        Collider2D sight = Physics2D.OverlapCircle(transform.position, sightRadius, targetMask);//total area of sight

        if (sight != null)
        {   
            target = sight.transform; //hit position in the world
            Vector2 directionToLook = (player.position - transform.position).normalized;//self explanatory
            
            Debug.DrawLine(transform.position, target.position, Color.red);

            if (Vector3.Angle(transform.position, directionToLook)<sightAngle/2 && !canDash)
            {
                distanceToTarget = Vector2.Distance(transform.position, target.position);
                
                if (!Physics2D.Raycast(transform.position, directionToLook, distanceToTarget, obstaclesLayer))
                {
                    aiAction = ToChase;
                }
            }else if (Vector3.Angle(transform.position, directionToLook) < sightAngle / 2 && canDash)
            {
                distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToLook, distanceToTarget, obstaclesLayer))
                {
                    aiAction = Dash;
                }
            }
        }
        else { aiAction = ToPatrol;}
    }

    void Timer(Action act)
    {
        timeToIdle -= 1 * Time.deltaTime;
        if (timeToIdle <= 0)
        {
            aiAction = act;
            timeToIdle += 5;
        }
    }

    void ToIdle()
    {
        speed = 0;
        Timer(SightAI);
    }

    public void ToPatrol()
    {   
        speed = maxSpeed;
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
        Timer(ToIdle);
    }

    void ToChase()
    {
        Debug.Log("Chase");
        float test;
        test = Mathf.Lerp(transform.position.x, player.position.x, maxSpeed*Time.deltaTime);
        transform.position = new Vector3 (test, this.transform.position.y, this.transform.position.z);
        distToPoint = Vector2.Distance(transform.position, player.position);

        if (distToPoint<=distanceToAtk)
        {
            aiAction = ToAttack;
        }
    }

    void ToAttack()
    {
        Debug.Log("Attack");
    }

    void Dash()
    {
        Vector2.MoveTowards(this.transform.position, target.position, distanceToTarget);
    }

    void OnDrawGizmos()
    {
        Color newColor;
        newColor = new Color(1, 1, 1, 0.25f);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = newColor;
        Gizmos.DrawSphere(transform.position, sightRadius);
    }

}