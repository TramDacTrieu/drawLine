using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using Spine;
using System;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEditor;

public enum AttackName
{
    Attack,
    Attack1,
    Attack2,
    Attack3
}


public class MonsterAI : MonoBehaviour, IDamable {
    const string LAYER_ENEMY = "Enemy";
    const string LAYER_ALLY = "Ally";
    
    public bool IsEnemy {
        set {
            isEnemy = value;
            Init(monsterData, isEnemy);
        }
        get { return isEnemy; }
    }

    public MonsterData monsterData;
    public UnityEvent idleEvent;
    public UnityEvent attackEvent;

    public string walkAnimationName;
    public string attackAnimationName;
    public string dieAnimationName;

    // enemy
    public List<string> enemyTags;

    protected Transform healthBar;
    protected TextMeshPro txtName;
    protected BattleStat battleStat;
    protected HitParam hitParam;

    protected Transform attackTarget;

    protected AnimationSetter animationSetter;
    protected Camera mainCamera;

    protected string enemyLayer;
    protected float baseScale;

    float nextAttack;
    bool isDied = false;
    float count = 0;

    [SerializeField] bool isEnemy = false;

    public Transform AttackTarget => attackTarget;
    public BattleStat BattleStat => battleStat;

    [HideInInspector]
    public bool active = true;

    public virtual void Awake() {
        healthBar = gameObject.GetChildComponent<Transform>("hpBox/hpBar");
        txtName = gameObject.GetChildComponent<TextMeshPro>("txtName");
        animationSetter = GetComponentInChildren<AnimationSetter>();

        if (animationSetter == null) {
            Debug.LogError($"No animation setter in {gameObject}'s children");
        }

        if (GetComponent<Collider2D>() == null) {
            Debug.LogError($"No collider2d in {gameObject}");
        }
        mainCamera = Camera.main;

        InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    public virtual void OnEnable() {
        isDied = false;

        Init(monsterData, isEnemy);
    }

    public void Init(MonsterData data, bool isEnemy = false) {
        battleStat = new BattleStat(data);
        battleStat.attackRange = Random.Range(battleStat.attackRange * 0.75f, battleStat.attackRange * 1.25f);
        battleStat.visionRange = Random.Range(battleStat.visionRange * 0.75f, battleStat.visionRange * 1.25f);
        healthBar.transform.localScale = new Vector3(1, 1, 1);
        //txtName.text = data.name;
        attackTarget = null;

        hitParam = new HitParam();
        hitParam.damage = battleStat.damage;
        hitParam.owner = transform;

        if (isEnemy) {
            gameObject.layer = LayerMask.NameToLayer(LAYER_ENEMY);
            enemyLayer = LAYER_ALLY;
        } else {
            gameObject.layer = LayerMask.NameToLayer(LAYER_ALLY);
            enemyLayer = LAYER_ENEMY;
        }
    }

    public virtual void Update()
    {
        if (isDied || !active) return;

        if (attackTarget == null) {
            if (idleEvent != null) idleEvent.Invoke();
            else {
                animationSetter.SetAnimation(walkAnimationName);
            }
            return;
        }

        if (Vector3.Distance(transform.position, attackTarget.position) > battleStat.attackRange)
        {
            StateChase();
        }
        else
        {
            StateAttack();
            txtName.text = "attack " + attackTarget.name;
        }
    }

    //public void OnInspectorGUI() {
    //    GUILayout.Label("This is a Label in a Custom Editor");
    //}

    //TODO: cần tối ưu performance chỗ này
    public void UpdateTarget() {
        if (attackTarget != null && attackTarget.gameObject.activeSelf == true) return;

        float shortestDistance = Mathf.Infinity;
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask(enemyLayer));
        Collider2D nearestEnemy = null;
        
        foreach (Collider2D enemy in allEnemies) {
            if (!enemyTags.Contains(enemy.tag)) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        try {
            if (nearestEnemy != null && shortestDistance <= battleStat.visionRange)
            { 
                attackTarget = nearestEnemy.transform;
            } else {
                attackTarget = null;
            }
        } catch {
            Debug.LogError("Error at game object " + gameObject.name);
        }
    }

    void StateChase() {
        animationSetter.SetAnimation(walkAnimationName);
        Vector3 direction = attackTarget.position - transform.position;
        var translateVector = direction.normalized * battleStat.speed * Time.deltaTime;
        transform.Translate(translateVector);

        count += Time.deltaTime;
        if (count > 1f) {
            count = 0;
            txtName.text = translateVector.magnitude.ToString();
        }
        animationSetter.SetFacing(attackTarget.transform.position);
    }

    public virtual void StateAttack() {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + battleStat.attackCountDown;
            Attack();
        }

        animationSetter.SetAnimation(attackAnimationName);
        animationSetter.SetFacing(attackTarget.transform.position);
    }

    public virtual void Attack()
    {
        attackTarget.GetComponent<IDamable>().TakeDame(new HitParam
        {
            damage = battleStat.damage,
            owner = transform
        });
    }

    public virtual void TakeDame(HitParam hitParam) {
        if (isDied) return;

        if (attackTarget == null || attackTarget.gameObject.activeSelf == false) {
            attackTarget = hitParam.owner;
        } else if (hitParam.owner != null){
            if (transform.DistanceTo(hitParam.owner) < transform.DistanceTo(attackTarget)) {
                attackTarget = hitParam.owner;
            }
        }

        battleStat.hp -= hitParam.damage;
        healthBar.transform.localScale = new Vector3(battleStat.hp / battleStat.maxhp, 1);
        if (battleStat.hp < 0) {
            Die();
        }
    }

    public virtual void Die() {
        isDied = true;
        var obj = gameObject;
        animationSetter.SetAnimation(dieAnimationName, ()=> { 
            if (obj != null && obj.activeSelf == true) {
                obj.SetActive(false);
            }
        });
        healthBar.transform.localScale = new Vector3(0, 1, 1);
    }

    public bool IsDead() {
        return battleStat.hp < 0;
    }

    void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "OnAttack")
        {
            Attack();
        }
    }
}