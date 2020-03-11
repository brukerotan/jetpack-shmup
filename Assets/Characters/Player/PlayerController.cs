using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float horizontalLimit = 10f;
    [SerializeField] float jetpackForce = 20f;
    [SerializeField] float jetpackLimit = 15f;
    
    [SerializeField] float fallLimit = -15f;
    Rigidbody2D rb;
    Vector2 movementInput;
    Vector2 aimDirection;

    [Header("Sprites & Animation")]
    [SerializeField] Transform handPivot;
    [SerializeField] SpriteRenderer leftHand;
    [SerializeField] SpriteRenderer rightHand;
    [SerializeField] SpriteRenderer pistolSpriteRenderer;
    [SerializeField] SpriteRenderer shotgunSpriteRenderer;
    bool isWeaponTwoHanded = false;
    SpriteRenderer playerSpriteRenderer;
    [SerializeField] float playerSpriteTilt = 10f;

    [Header("Particle Effects")]
    [SerializeField] Transform particleEffectsParent;   //This will at the same time the sprite is flipped
    [SerializeField] ParticleSystem jetpackParticle;
    [SerializeField] ParticleSystem jetpackParticle2;

    [Header("Audio")]
    [SerializeField] AudioSource jetpackAudioSource;
    [SerializeField] float jetpackAudioInterval = 0.05f;
    float jetpackAudioTimer = 0;

    [Header("Weapons")]
    [SerializeField] WeaponData currentWeapon;
    [SerializeField] List<WeaponData> weaponInventory = new List<WeaponData>();
    float firerateTimer = 0f;
    [SerializeField] AudioSource weaponAudioSource;

    bool gamepadAim = false;
    bool isJetpackOn = false;
    bool isDead = false;
    bool kickBack = false;
    
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;

        rb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");


        if (gamepadAim)
        {
            aimDirection = new Vector2(movementInput.x, movementInput.y).normalized;
        }
        else
        {
            aimDirection = GetMouseDirection();
        }

        isJetpackOn = Input.GetButton("Jump") || movementInput.y == 1.0f ? true : false;

        AimHands(aimDirection);

        firerateTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && firerateTimer >= currentWeapon.fireRate)
        {
            Fire();
            firerateTimer = 0;
        }

        FlipSprites();
        AnimateSprite();
        ControlParticles();

        if (isJetpackOn)
        {
            jetpackAudioTimer += Time.deltaTime;
            if(jetpackAudioTimer > jetpackAudioInterval)
            {
                jetpackAudioTimer = 0;
                jetpackAudioSource.Play();
            }

            jetpackAudioSource.pitch = Mathf.Clamp(rb.velocity.y * Random.Range(0.1f, 0.3f), 0.5f, 1.2f);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            isDead = !isDead;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            isWeaponTwoHanded = !isWeaponTwoHanded;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            gamepadAim = !gamepadAim;
        }
    }
  
    void FixedUpdate()
    {
        if (rb.velocity.y < fallLimit)
        {
            rb.velocity = new Vector2(rb.velocity.x, fallLimit);
        }

        if (isDead)
        {
            //Rigidbody can't be affected by the player
            return;
        }

        if (isJetpackOn && rb.velocity.y <= jetpackLimit)
        {
            rb.AddForce(new Vector2(0, jetpackForce), ForceMode2D.Force);
        }       
        
        rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);

        if (kickBack)
        {
            kickBack = false;
            rb.MovePosition(rb.position - aimDirection * currentWeapon.recoil);
        }
    }

    Vector2 GetMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        direction.Normalize();
        return direction;
    }

    void AimHands(Vector2 direction)
    {
        handPivot.transform.right = direction;

        if (isWeaponTwoHanded)
        {
            rightHand.enabled = true;
            shotgunSpriteRenderer.enabled = true;
            pistolSpriteRenderer.enabled = false;
            leftHand.transform.localEulerAngles = new Vector3(0, 0, -180f);
        }
        else
        {
            rightHand.enabled = false;
            shotgunSpriteRenderer.enabled = false;
            pistolSpriteRenderer.enabled = true;
            leftHand.transform.localEulerAngles = Vector3.zero;
        }
    }

    void Fire()
    {
        switch (currentWeapon.instantiateMode)
        {
            case WeaponData.InstantiateMode.SINGLE:
                for (int i = 0; i < currentWeapon.bulletsSpawnedPerShot; i++)
                {
                    Vector3 randomPosOffset = new Vector3(0, Random.Range(-0.4f, 0.4f));
                    Vector3 randomEulerOffset = new Vector3(0, 0, Random.Range(-4, 4));
                    GameObject bullet = Instantiate(currentWeapon.bulletPrefab,
                                        pistolSpriteRenderer.transform.position + randomPosOffset,
                                        handPivot.rotation * Quaternion.Euler(randomEulerOffset));
                    bullet.GetComponent<Bullet>().speed = currentWeapon.bulletSpeed * Random.Range(0.8f, 1.2f);
                    Destroy(bullet.gameObject, currentWeapon.bulletDuration * Random.Range(0.9f, 1.1f));

                    weaponAudioSource.PlayOneShot(currentWeapon.shootingAudio);
                    weaponAudioSource.pitch = Random.Range(currentWeapon.audioPitch * 0.75f, currentWeapon.audioPitch * 1.25f);
                }
                break;
            case WeaponData.InstantiateMode.SPREAD:
                for (int i = 0; i < currentWeapon.bulletsSpawnedPerShot; i++)
                {
                    Vector3 randomPosOffset = new Vector3(0, Random.Range(-0.4f, 0.4f));
                    Vector3 spreadEulerOffset = new Vector3(0, 0, 18 * (Mathf.RoundToInt((-currentWeapon.bulletsSpawnedPerShot / 2) - 0.1f) + i));
                    Vector3 randomEulerOffset = new Vector3(0, 0, Random.Range(-4, 4));
                    GameObject bullet = Instantiate(currentWeapon.bulletPrefab,
                                        pistolSpriteRenderer.transform.position + randomPosOffset,
                                        handPivot.rotation * Quaternion.Euler(randomEulerOffset) * Quaternion.Euler(spreadEulerOffset));
                    bullet.GetComponent<Bullet>().speed = currentWeapon.bulletSpeed * Random.Range(0.8f, 1.2f);
                    Destroy(bullet.gameObject, currentWeapon.bulletDuration * Random.Range(0.9f, 1.1f));

                    weaponAudioSource.PlayOneShot(currentWeapon.shootingAudio);
                    weaponAudioSource.pitch = Random.Range(currentWeapon.audioPitch * 0.75f, currentWeapon.audioPitch * 1.25f);
                }
                break;
            default:
                break;
        }
        
        kickBack = true;
    }

    private void FlipSprites()
    {
        if (aimDirection.x < 0f && !isDead)
        {
            playerSpriteRenderer.flipX = true;
            pistolSpriteRenderer.flipY = true;
            shotgunSpriteRenderer.flipY = true;
            leftHand.flipY = true;
            rightHand.flipY = true;
        }
        else
        {
            playerSpriteRenderer.flipX = false;
            pistolSpriteRenderer.flipY = false;
            shotgunSpriteRenderer.flipY = false;
            leftHand.flipY = false;
            rightHand.flipY = false;
        }       
    }

    void AnimateSprite()
    {
        if (!isDead)
        {
            playerSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, -movementInput.x * playerSpriteTilt);
            playerSpriteRenderer.color = Color.white;
            playerSpriteRenderer.transform.localPosition = Vector3.zero;
        }
        else
        {
            playerSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 90);
            playerSpriteRenderer.color = Color.grey;
            playerSpriteRenderer.transform.localPosition = new Vector3(0, -0.5f);
        }
    }

    void ControlParticles()
    {
        particleEffectsParent.transform.localEulerAngles = playerSpriteRenderer.flipX ? new Vector3(0, -180, 0) : Vector3.zero;
        if (isJetpackOn)
        {
            if (!jetpackParticle.isPlaying)
            {
                jetpackParticle.Play();
                jetpackParticle2.Play();
            }
        }
        else
        {
            jetpackParticle.Stop();
            jetpackParticle2.Stop();
        }
    }
}
