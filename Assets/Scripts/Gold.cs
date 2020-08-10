using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Gold : MonoBehaviour
{
    public Sprite[] GoldSprite;
    public int count;
    [SerializeField]
    public SpriteRenderer Sprite;

    private bool Pickable = true;
    private float RespawnTimer = 0f;
    public float recoveryTime = 2.5f;

    public float PickupTime; // Total Time it takes the player to respawn
    private float pickupTimer = 1f; // Variable to store the timer of the respawn
    void Start()
    {
        count = Random.Range(0, GoldSprite.Length);
        for (int i = 1; i < GoldSprite.Length + 1; i++)
        {
            if (count > i)
            {
                Sprite.sprite = GoldSprite[i];
                
            }
            else break;
        }
    }
         void Awake()
        {
            Sprite.enabled = true;
            if (Sprite == null)
            {
                Sprite = GetComponent<SpriteRenderer>();

                if (Sprite == null)
                {
                    Debug.Log("This refill has no spriterenderer attached to it");
                }
            }
        }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Pickable)
        {
            var playercomponent = other.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Pickable)
        {
            var playercomponent = other.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }
    }

    void OnPlayerTrigger(Player player)
    {
            Pickable = false;
            // Disable
            Sprite.enabled = false;
            this.RespawnTimer = recoveryTime;

            // Screenshake
            if (PixelCameraController.instance != null)
            {
                PixelCameraController.instance.Shake(0.1f);
            }
    }

}
