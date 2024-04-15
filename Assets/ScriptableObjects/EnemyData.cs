using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string name;
    public float meleeRange;
    public float rangedRange;
    public float timeBetweenAttacks;
    public float damage;
    public float maxHealth;
    public float moveSpeed;
    public List<GameObject> projectilePrefabs;
    public bool weaponEnabled;
}
