using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupArtifact : Actor
{

    public float Gravity = 900f; // Gravity force
    public float MaxFall = -240f; // Maximun fall speed

    private bool Pickable = true;

    private GameObject inventory;
    public GameObject itemButton;

    public float PickupTime; // Total Time it takes the player to respawn
    private float pickupTimer = 1f; // Variable to store the timer of the respawn

	
    [SerializeField]
    private SpriteRenderer Sprite;

    new void Awake()
    {
        base.Awake();
        if (Sprite == null)
        {
            Sprite = GetComponent<SpriteRenderer>();

            if (Sprite == null)
            {
                Debug.Log("This refill has no spriterenderer attached to it");
            }
        }
        inventory = GameObject.FindGameObjectWithTag("Inventory");
    }

    // Update is called once per frame
    new void Update()
    {
        // Update variables
        wasOnGround = onGround;
        onGround = OnGround();
        // Gravity
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        if (pickupTimer > 0f)
        {
            pickupTimer -= Time.deltaTime;
        }
    }
    void LateUpdate()
    {
        // Vertical movement
        var movev = base.MoveV(Speed.y * Time.deltaTime);
        if (movev)
            Speed.y = 0;
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
        inventory = GameObject.FindGameObjectWithTag("Inventory");
        player.PickUpArtifacts(GetComponent<ItemParameters>());
                Pickable = false;
                // Disable
                Sprite.enabled = false;

                // Screenshake
                if (PixelCameraController.instance != null)
                {
                    PixelCameraController.instance.Shake(0.1f);
                }
            Instantiate(itemButton, inventory.transform, false);
        this.gameObject.SetActive(true);
    }
}
