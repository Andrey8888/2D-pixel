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

public class RuneCountBarFade : MonoBehaviour {

    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = .6f;

    private Image barImage;
    private Image damagedBarImage;
    private Color damagedColor;
    private float damagedHealthFadeTimer;
    private HealthSystem healthSystem;
    private int startHealth;
	private GameObject HealthText;
	
    public static RuneCountBarFade instance = null;

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
		
		HealthText = GameObject.FindGameObjectWithTag("HealthCount");
		
        barImage = transform.Find("bar").GetComponent<Image>();
        damagedBarImage = transform.Find("damagedBar").GetComponent<Image>();
        damagedColor = damagedBarImage.color;
        damagedColor.a = 0f;
        damagedBarImage.color = damagedColor;
    }

    private void Start() {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        startHealth = player.GetComponent<Health>().health;
		HealthText.GetComponent<Text>().text = startHealth.ToString();
		
        healthSystem = new HealthSystem(player.GetComponent<Health>().health);
        SetHealth(healthSystem.GetHealthNormalized());
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
    }

    private void Update() {
        if (damagedColor.a > 0) {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0) {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                damagedBarImage.color = damagedColor;
            }
        }
    }

    private void HealthSystem_OnHealed(object sender, System.EventArgs e) {
        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e) {
        if (damagedColor.a <= 0) {
            // Damaged bar image is invisible
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
        damagedColor.a = 1;
        damagedBarImage.color = damagedColor;
        damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;

        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized) {
        barImage.fillAmount = healthNormalized;
    }

    public void Damage(int dmg)
    {
        healthSystem.Damage(startHealth - dmg);
        startHealth = startHealth - (startHealth - dmg);
		HealthText.GetComponent<Text>().text = startHealth.ToString();
    }

    public void Heal(int heal)
    {
        healthSystem.Heal(-startHealth + heal);
        startHealth = startHealth + (-startHealth + heal);
		HealthText.GetComponent<Text>().text = startHealth.ToString();
    }
    public void Refresh()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        healthSystem.healthAmount = player.GetComponent<Health>().maxHealth;
		HealthText.GetComponent<Text>().text = startHealth.ToString();
    }
}
