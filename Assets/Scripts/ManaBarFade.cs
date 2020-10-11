/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using CodeMonkey;

public class ManaBarFade : MonoBehaviour
{

    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = .6f;

    private Image barImage;
    private Image damagedBarImage;
    private Color damagedColor;
    private float damagedHealthFadeTimer;
    private HealthSystem manaSystem;
    private int startMana;

    public static ManaBarFade instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        barImage = transform.Find("bar").GetComponent<Image>();
        damagedBarImage = transform.Find("damagedBar").GetComponent<Image>();
        damagedColor = damagedBarImage.color;
        damagedColor.a = 0f;
        damagedBarImage.color = damagedColor;
    }

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        startMana = player.GetComponent<Mana>().mana;

        manaSystem = new HealthSystem(player.GetComponent<Health>().health);
        SetHealth(manaSystem.GetHealthNormalized());
        manaSystem.OnDamaged += manaSystem_OnDamaged;
        manaSystem.OnHealed += manaSystem_OnHealed;
    }

    private void Update()
    {
        if (damagedColor.a > 0)
        {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                damagedBarImage.color = damagedColor;
            }
        }
    }

    private void manaSystem_OnHealed(object sender, System.EventArgs e)
    {
        SetHealth(manaSystem.GetHealthNormalized());
    }

    private void manaSystem_OnDamaged(object sender, System.EventArgs e)
    {
        if (damagedColor.a <= 0)
        {
            // Damaged bar image is invisible
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
        damagedColor.a = 1;
        damagedBarImage.color = damagedColor;
        damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;

        SetHealth(manaSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized)
    {
        barImage.fillAmount = healthNormalized;
    }

    public void Cast(int coast)
    {
        manaSystem.Damage(startMana - coast);
        startMana = startMana - (startMana - coast);
    }

    public void Refill(int mana)
    {
        manaSystem.Heal(-startMana + mana);
        startMana = startMana + (-startMana + mana);
    }
    public void Refresh()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        manaSystem.healthAmount = player.GetComponent<Mana>().maxMana;
    }
}
