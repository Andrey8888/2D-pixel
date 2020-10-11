using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{

    public UnityEvent OnTakeDamageEvent;
    public UnityEvent OnPushedEvent;
    public UnityEvent OnPushedUpEvent;
    public UnityEvent OnBurnedEvent;
    public UnityEvent OnStunedEvent;
    public UnityEvent OnFreezedEvent;
    public UnityEvent OnPoisonedEvent;
    public UnityEvent OnBurnedEndEvent;
    public UnityEvent OnFreezedEndEvent;
    public UnityEvent OnStunedEndEvent;
    public UnityEvent OnPoisonedEndEvent;
    public UnityEvent OnDamageBlockEvent;
    public UnityEvent OnTakeHealEvent;
    public UnityEvent OnDeathEvent;

    [Header("Max/Starting Health")]
    public int maxHealth;
    [Header("Current Health")]
    public int health;

    [Header("IsDeathOrNot")]
    public bool dead = false;

    [Header("Invincible")]
    public bool invincible = false;
    public bool block = false;
    public bool becomeInvincibleOnHit = false;
    public float invincibleTimeOnHit = .5f;
    private float invincibleTimer = 0f;

    [Header("Perform Dead Events after x time")]
    public float DieEventsAfterTime = 1f;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleTimer <= 0f)
            {
                if (invincible)
                    invincible = false;
            }
        }
    }

    public bool TakeDamage(int amount, bool poison, int poisonAmount, int poisonFrequency, int poisonTick, int poisonChance,
        bool fire, int fireAmount, int fireFrequency, int fireTick, int fireChance, bool push, int pushDistance,  
		bool freez, int freezDuration, int freezChance, bool pushUp, int pushUpDistance, bool stun, int stunDuration, int stunChance)
    {
        if (block)
        {
            if (OnDamageBlockEvent != null)
                OnDamageBlockEvent.Invoke();
            block = false;
            return true;
        }
        if (dead || invincible)
        {
            return false;
        }
        health = Mathf.Max(0, health - amount);

        if (OnTakeDamageEvent != null)
            OnTakeDamageEvent.Invoke();

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (becomeInvincibleOnHit)
            {
                invincible = true;
                invincibleTimer = invincibleTimeOnHit;
            }
			
            if (poison)
			{
			    if (Random.Range(0, 100) < poisonChance)
                {
				     StartCoroutine( Poisoned(poisonAmount, poisonFrequency, poisonTick));
				}
			}
			
            if (fire)
			{
			    if (Random.Range(0, 100) < fireChance)
                {
				StartCoroutine(Burned(fireAmount, fireFrequency, fireTick));
				}
			}
			
            if (push)
                Pushed(pushDistance);
				
			if (pushUp)
                PushedUp(pushUpDistance);
				
            if (freez)
				{
					if (Random.Range(0, 100) < freezChance)
					{
						 StartCoroutine(Freezed(freezDuration));
					}
				}
				
			if (stun)
				{
					if (Random.Range(0, 100) < stunChance)
					{
						 StartCoroutine(Stuned(stunDuration));
					}
				}
				
            PixelCameraController.instance.Shake(0.15f);
        }
        return true;
    }

    IEnumerator Poisoned(int amount, int frequency, int tick)
    {
        for (int i = 0; i < tick; i++)
        {
            if (OnPoisonedEvent != null)
                OnPoisonedEvent.Invoke();
            yield return new WaitForSeconds(frequency);
            health = Mathf.Max(0, health - amount);
            if (OnTakeDamageEvent != null)
                OnTakeDamageEvent.Invoke();
            if (health <= 0)
            {
                Die();
            }
            PixelCameraController.instance.Shake(0.15f);
        }
        if (OnPoisonedEndEvent != null)
            OnPoisonedEndEvent.Invoke();
        yield return false;
    }

    IEnumerator Burned(int amount, int frequency, int tick)
    {
        for (int i = 0; i < tick; i++)
        {
            if (OnBurnedEvent != null)
                OnBurnedEvent.Invoke();
            yield return new WaitForSeconds(frequency);
            health = Mathf.Max(0, health - amount);
            if (OnTakeDamageEvent != null)
                OnTakeDamageEvent.Invoke();
            if (health <= 0)
            {
                Die();
            }
            PixelCameraController.instance.Shake(0.15f);
        }
        if (OnBurnedEndEvent != null)
            OnBurnedEndEvent.Invoke();
        yield return false;
    }

    IEnumerator Freezed(int duration)
    {
        if (OnFreezedEvent != null)
            OnFreezedEvent.Invoke();
        yield return new WaitForSeconds(duration);

        if (OnFreezedEndEvent != null)
            OnFreezedEndEvent.Invoke();
        yield return false;
    }
	
	IEnumerator Stuned(int duration)
    {
        if (OnStunedEvent != null)
            OnStunedEvent.Invoke();
        yield return new WaitForSeconds(duration);

        if (OnStunedEndEvent != null)
            OnStunedEndEvent.Invoke();
        yield return false;
    }
	
    public void Pushed(int pushDistance)
    {
        if (OnPushedUpEvent != null)
            OnPushedUpEvent.Invoke();
        Debug.Log("push");
        if (!GetComponentInParent<Enemy>().CheckColAtPlace(Vector2.right * (int)GetComponentInParent<Enemy>().Facing * pushDistance, GetComponentInParent<Enemy>().solid_layer))
        {
            transform.position = new Vector2(transform.position.x + (pushDistance * (int)GetComponentInParent<Enemy>().Facing), transform.position.y);
            PixelCameraController.instance.Shake(0.35f);
        }
    }

    public void PushedUp(int pushUpDistance)
    {
        if (OnPushedEvent != null)
            OnPushedEvent.Invoke();
        Debug.Log("pushUp");
        if (!GetComponentInParent<Enemy>().CheckColAtPlace(Vector2.up * pushUpDistance, GetComponentInParent<Enemy>().solid_layer))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + pushUpDistance);
            PixelCameraController.instance.Shake(0.35f);
        }
    }
	
public bool TakeHeal(int amount)
{
    if (dead || health == maxHealth)
        return false;

    health = Mathf.Min(maxHealth, health + amount);

    if (OnTakeHealEvent != null)
        OnTakeHealEvent.Invoke();

    PixelCameraController.instance.Shake(0.1f);

    return true;
}

public void Die()
{
    dead = true;
    PixelCameraController.instance.Shake(0.25f);

    StartCoroutine(DeathEventsRoutine(DieEventsAfterTime));
}

IEnumerator DeathEventsRoutine(float time)
{
    yield return new WaitForSeconds(time);
    if (OnDeathEvent != null)
        OnDeathEvent.Invoke();
        StopAllCoroutines();
}

public void SetUIHealthBar()
{
    if (UIHealthBar.instance != null)
    {
        UIHealthBar.instance.setHealthBar(health);
    }
}

public void SetUIHealthBarFadeDamage()
{
    if (HealthBarFade.instance != null)
    {
        HealthBarFade.instance.Damage(health);
    }
}

public void SetUIHealthBarFadeHeal()
{
    if (HealthBarFade.instance != null)
    {
        HealthBarFade.instance.Heal(health);
    }
}
}
