using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{


    [Header("Owner of the projectile Spawner")]
    public Health owner;

    [Header("Projectile to Shoot")]
    public Projectile[] projectile;
    public Projectile[] powerProjectile;
    public RangedWeapon RangedWeaponType;

    [Header("Place to spawn the projectile at")]
    public Transform[] gunBarrel;
    public Transform[] gunBarrelDuck;
    public Transform[] PowerGunBarrel;
    public Transform[] PowerGunBarrelDuck;


    private Projectile curProjectile;
    private Transform curGunBarrel;
    private Transform curBarrelDuck;
    private Projectile curPowerProjectile;
    private Transform curPowerGunBarrel;
    private Transform curPowerGunBarrelDuck;

    void Awake()
    {
        if (owner == null)
        {
            owner = GetComponentInParent<Health>();

            if (owner == null)
            {
                Debug.Log("This projectileSpawner has no owner/health attached to it");
            }
        }
    }


    public void Update()
    {
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;

        var playercomponent = GetComponentInParent<Player>();
        if (playercomponent.RangedPowerAttackAiming)
        {
            float dist = Vector2.Distance(playercomponent.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (dist < 200)
            {
                playercomponent.aimSprite.GetComponent<SpriteRenderer>().sprite = playercomponent.AimSpriteGreen;
            }
            else
            {
                playercomponent.aimSprite.GetComponent<SpriteRenderer>().sprite = playercomponent.AimSpriteRed;
            }
        }
    }

    public void ChangeProjectile(int i)
    {
        curProjectile = projectile[i];
        curGunBarrel = gunBarrel[i];
        curBarrelDuck = gunBarrelDuck[i];
        curPowerProjectile = powerProjectile[i];
        curPowerGunBarrel = PowerGunBarrel[i];
        curPowerGunBarrelDuck = PowerGunBarrelDuck[i];
    }

    public void InstantiateProjectile()
    {
        //Instantiate the projectile prefab
        var p = Instantiate(curProjectile, curGunBarrel.position, Quaternion.identity) as Projectile;

        // Shoot based on the X scale of our parent object (base facing), which should be 1 for right and -1 for left 
        var parentXScale = Mathf.Sign(transform.parent.localScale.x);

        // Set the localscale so the projectiles faces the right direction based on the parent object (base)
        p.transform.localScale = new Vector3(parentXScale * p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z);

        if (owner != null)
        {
            p.owner = owner; // Set it's owner 
        }

        // Change the X speed based on the facing of the parent object
        p.Speed.x *= parentXScale;

        // Do a small screenshake to add a little bit of extra "feel"
        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(new Vector2(parentXScale, 0f), .05f);
        }
    }

    public void InstantiatePowerProjectile()
    {
        var playercomponent = GetComponentInParent<Player>();

        if (playercomponent.RangedPowerAttackAiming)
        {

            var cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit2D = Physics2D.Raycast(cursor, Vector2.zero); // Vector2.zero если нужен рейкаст именно под курсором

            if (hit2D.collider.tag == "BackGround")
            {
                float dist = Vector2.Distance(playercomponent.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (dist < 200)
                {
                    InstantiateFreePosition(cursor);
                }
                else
                {
                    playercomponent.rangedPowerAttackCooldownTimer = 0;
                    playercomponent.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
                }
            }
            else
            {
                playercomponent.aimSprite.GetComponent<SpriteRenderer>().sprite = playercomponent.AimSpriteRed;
                playercomponent.rangedPowerAttackCooldownTimer = 0;
                playercomponent.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
            }
        }
        else
        {
            InstantiateFreePosition(curPowerGunBarrel.position);
        }
    }

    public void DuckInstantiateProjectile()
    {
        //Instantiate the projectile prefab
        var p = Instantiate(curProjectile, curBarrelDuck.position, Quaternion.identity) as Projectile;

        // Shoot based on the X scale of our parent object (base facing), which should be 1 for right and -1 for left 
        var parentXScale = Mathf.Sign(transform.parent.localScale.x);

        // Set the localscale so the projectiles faces the right direction based on the parent object (base)
        p.transform.localScale = new Vector3(parentXScale * p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z);

        if (owner != null)
        {
            p.owner = owner; // Set it's owner 
        }

        // Change the X speed based on the facing of the parent object
        p.Speed.x *= parentXScale;

        // Do a small screenshake to add a little bit of extra "feel"
        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(new Vector2(parentXScale, 0f), .05f);
        }
    }

    public void DuckInstantiatePowerProjectile()
    {
        var playercomponent = GetComponentInParent<Player>();

        Projectile p = null;

        if (playercomponent.RangedPowerAttackAiming)
        {
            if (playercomponent.RangedPowerAttackAiming)
            {
                var cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit2D = Physics2D.Raycast(cursor, Vector2.zero); // Vector2.zero если нужен рейкаст именно под курсором

                if (hit2D.collider.tag == "BackGround")
                {
                    float dist = Vector2.Distance(playercomponent.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (dist < 100)
                    {
                        InstantiateFreePosition(cursor);
                    }
                    else
                    {
                        playercomponent.rangedPowerAttackCooldownTimer = 0;
                        playercomponent.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
                    }
                }
                else
                {
                    playercomponent.aimSprite.GetComponent<SpriteRenderer>().sprite = playercomponent.AimSpriteRed;
                    playercomponent.rangedPowerAttackCooldownTimer = 0;
                    playercomponent.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
                }
            }
            else
            {
                InstantiateFreePosition(curPowerGunBarrelDuck.position);
            }
        }
    }

    public void InstantiateFreePosition(Vector2 pos)
    {
        Projectile p = null;

        p = Instantiate(curPowerProjectile, pos, Quaternion.identity) as Projectile;

        // Shoot based on the X scale of our parent object (base facing), which should be 1 for right and -1 for left 
        var parentXScale = Mathf.Sign(transform.parent.localScale.x);

        // Set the localscale so the projectiles faces the right direction based on the parent object (base)
        p.transform.localScale = new Vector3(parentXScale * p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z);

        if (owner != null)
        {
            p.owner = owner; // Set it's owner 
        }

        // Change the X speed based on the facing of the parent object
        p.Speed.x *= parentXScale;

        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(new Vector2(parentXScale, 0f), .05f);
        }
    }
}
