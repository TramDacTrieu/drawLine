using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStat 
{
    public float speed = 4f;
    public float baseExp = 200;
    public float currentExp = 0;
    public float nextLvlExp = 0;

    public float expGainWhenKill = 42f;
    public float goldGainWhenKill = 20f;
    public float foodGainWhenKill = 84f;
    public float foodConsumePerSec = 6f;
    public float shopPrice = 400f;

    public float hp = 200;
    public float maxhp = 200;
    public float damage = 20;
    public float attackRange = 2f;
    public float visionRange = 20f;
    public float attackCountDown = 1f;
    public int formationSortingOrder = 0;
    public float followRange =3;

    public BattleStat(MonsterData data) {
        this.speed = data.speed;
        this.hp = data.maxhp;
        this.maxhp = data.maxhp;
        this.damage = data.damage;
        this.attackRange = data.attackRange;
        this.visionRange = data.visionRange;
        this.attackCountDown = data.attackSpeed;
        //this.formationSortingOrder = data.formationSortingOrder;
        //this.goldGainWhenKill = data.goldGainWhenKill;
        //this.followRange = data.followRange;
    }
}
