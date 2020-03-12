using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon Data", order = 51)]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] public string weaponName = "Weapon";

    [Header("Stats")]
    [SerializeField] public GameObject bulletPrefab;

    public enum InstantiateMode { SINGLE, SPREAD };
    [SerializeField] public InstantiateMode instantiateMode = InstantiateMode.SINGLE;

    [SerializeField, Range(1, 50)] public int bulletsSpawnedPerShot = 1;
    [SerializeField, Range(0.1f, 10f)] public float bulletDuration = 2f;
    [SerializeField, Range(0.5f, 100f)] public float bulletSpeed = 40f;
    [SerializeField, Range(1, 300)] public float maxAmmo = 150f;
    [SerializeField, Range(0, 20)] public int ammoSpentPerShot = 1;
    [SerializeField, Range(0.001f, 3f)] public float fireRate = 0.2f;
    [SerializeField, Range(-2, 2)] public float recoil = 0.1f;    

    [Header("Graphics and Audio")]
    [SerializeField] public Sprite weaponSprite;
    [SerializeField] public bool isTwoHanded = true;
    [SerializeField] public Sprite ejectedCasingSprite;
    [SerializeField] public AudioClip shootingAudio;
    [SerializeField, Range(0.2f, 1.8f)] public float audioPitch;
}
