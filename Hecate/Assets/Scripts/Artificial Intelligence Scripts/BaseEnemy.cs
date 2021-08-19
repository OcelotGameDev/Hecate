using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TypesOfMonster {Lobisomen, Cachorro, Boss};

public class BaseEnemy : MonoBehaviour, IHittable
{   
    public Transform player, nose;
    public GameObject doggy;
    public Transform[] spawnPos;
    public LayerMask groundLayer, targetMask, obstaclesLayer;
    
    public TypesOfMonster monsterType;
    public float distanceToChase, distanceToAtk, maxHp, maxSpeed, timeToIdle;
    public float sightRadius, sightAngle, groundSight, direction, dashDistance;
    
    
    bool isDead=> currentHp <= 0;
    bool movingRight = true, canDash=false, isWere, isDog, isBoss;
    float speed,currentHp, distToPoint, distanceToTarget;
    int dashMx = 3;
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
    
    public void Hit(int damage = 1, Transform attacker = null)
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
        LifeEvents();
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
                    isWere = true;
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
                    isDog = true;
                }
                break;

            case TypesOfMonster.Boss:{
                    /* Set values related to BOSS Enemy*/
                    maxSpeed = 10;
                    maxHp = 15;
                    sightRadius = 6;
                    sightAngle = 35;
                    aiAction = SightAI;
                    isBoss = true;
                }
                break;
        }
    }

    void SightAI()
    {
        speed = maxSpeed;
        Collider2D sight = Physics2D.OverlapCircle(transform.position, sightRadius, targetMask);//total area of sight

        if (sight != null)
        {   
            Transform targetPoint = sight.transform; //hit position in the world
            Vector2 directionToLook = (player.position - transform.position).normalized;//self explanatory
            
            Debug.DrawLine(transform.position, targetPoint.position, Color.red);

            if (Vector3.Angle(transform.position, directionToLook)<sightAngle/2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, targetPoint.position);
                
                if (!Physics2D.Raycast(transform.position, directionToLook, distanceToTarget, obstaclesLayer))
                {
                    aiAction = ToChase;
                }
            }
        }
        else { aiAction = ToPatrol; }
    }

    /*void Timer2(Action act)
    {
        timeToIdle -= 1 *Time.deltaTime;
        float timeToDash=4;
        timeToDash -= 1 * Time.deltaTime;

        if (!isDog)
        {
            if (timeToIdle <= 0)
            {
                aiAction = act;
                timeToIdle = 5;
            }
        }else if (isDog)
        {
            while (timeToDash > 0)
            {
                canDash = true;
            }
        }
    }*/

    IEnumerator Timer(Action act)
    {
        yield return new WaitForSeconds(timeToIdle);
        aiAction = act;
    }

    void ToIdle()
    {
        Debug.Log("Idle");
        speed = 0;
        StartCoroutine(Timer(SightAI)); ;
    }

    void ToPatrol()
    {
        Debug.Log("Patrol");        
        speed = maxSpeed;

        StartCoroutine(Timer(ToIdle));

        if (hitToGround.collider != false)
        {
            if (movingRight)
            {
                direction = 1;
                rBody.velocity = new Vector2(speed, rBody.velocity.y);
            }
            else
            {
                direction = -1;
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
        if (isWere)
        {
            
        }

        if (isDog)
        {
            canDash = true;
            StartCoroutine(DashTimer());
        }

        if (isBoss)
        {
            BossDash();
        }
    }

    IEnumerator DashTimer()
    {
        if (isDog && canDash)
        {
            rBody.AddForce(new Vector2(dashDistance * direction,0f),ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
            canDash = false;
        }

        if (isBoss && (dashMx <= 0))
        {
            yield return new WaitForSeconds(4f);
            dashMx = 3;
        }
    }

    void BossDash()
    {
        float mxDistance = player.position.x - this.transform.position.x;
        Vector2 newDistance = new Vector2((player.position.x - this.transform.position.x), this.transform.position.x);
        if (dashMx > 0 && canDash)
        {
            if (Vector2.Distance(this.transform.position, newDistance) <= mxDistance)
            {
                dashMx -= dashMx;
                Vector2.MoveTowards(this.transform.position, newDistance, mxDistance);
            }
        }
        else if(dashMx<=0){ StartCoroutine(DashTimer()); }
    }

    void LifeEvents()
    {
        if (isBoss)
        {
            if (currentHp > maxHp/2)
            {

            }

            if (currentHp <= maxHp / 2 && currentHp <= maxHp / 4)
            {
                InvokeDogs();
            }

            if (currentHp <= maxHp / 4)
            {
                aiAction = BossDash;
            }
        }
    }

    void InvokeDogs()
    {   
        GameObject clone;
        for (int i = 0; i < spawnPos.Length; i++)
        {
            i++;
            clone = Instantiate(doggy, spawnPos[i].position, transform.rotation);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            
        }
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