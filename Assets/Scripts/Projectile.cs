using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	[Header ("Speed")]
	public Vector2 Speed;

	[Header ("Damage")]
	public int DamageOnHit;

	[Header ("Layers")]
	public LayerMask solid_layer;

	[HideInInspector]
	public Health owner; // owner of the projectile
	private Vector2 Position; // Current position
	private Vector2 movementCounter = Vector2.zero;  // Counter for subpixel movement
	private Rigidbody2D rb2D; // Cached Rigidbody2D attached to the projectile

    public float InitVelocity;          // Начальная скорость стрелы
    private float curVelocity;          // Текущая скорость
    private float startAngle;           // Начальный угол стрельбы
    private float timer = 0;            // счетчик времени
    private Vector2[] positionsMassive; //
    public float destroyDelay = 5.0f;   // задержка перед удалением стрелы после попадание в объект
    private bool isCollision;           //
    private float destroyTime;          // Время уничтожения стрелы
    private float colAngle;             // Угловое положение стрелы в момент удара
    public float changAngle;            //
    public Collider2D OurCollider;
    List<Health> healthsDamaged = new List<Health>(); // List to store healths damaged

    public enum Owner
    {
        Enemy,
        Player,
        Cannon
    }
    public Owner OwnerType = Owner.Player;
    public enum MovementType
    {
        Ballistic,
        Line
    }
    public MovementType Movement = MovementType.Ballistic;
    void Start()
    {
        if (OwnerType == Owner.Player)
        {
            Player archer = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
            InitVelocity = archer.Velocity;
        }

        if (OwnerType == Owner.Enemy)
        {
            WormEnemy enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponentInChildren<WormEnemy>();
            InitVelocity = 700;
        }

        startAngle = Mathf.Deg2Rad * transform.eulerAngles.z;
        if (transform.eulerAngles.z > 90)
            changAngle = 90 - (transform.eulerAngles.z - 270);
        else
            changAngle = transform.eulerAngles.z;
        positionsMassive = new Vector2[2];
        isCollision = false;
        OurCollider = gameObject.GetComponentInChildren<Collider2D>();
        OurCollider.isTrigger = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (isCollision == false)
        {
            ArrowMove();
            ArrowRotate();
        }
        else
        {
            if (timer > destroyTime)
                Destroy(gameObject);
            else
            {
                transform.position = positionsMassive[0] + (positionsMassive[1] - positionsMassive[0]);
                transform.eulerAngles = new Vector3(0, 0, colAngle);
            }
        }
    }

    public void ArrowRotate()
    {
        if (Movement == MovementType.Ballistic)
        {// Стрела всегда смотрит в направление вектора скорости
            float angle = Vector3.Angle(Vector3.right, positionsMassive[1] - positionsMassive[0]);
            if (positionsMassive[1].y < positionsMassive[0].y)
            {
                angle = 360 - angle;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
            changAngle = angle;
            colAngle = transform.eulerAngles.z;
        }
    }

    private void ArrowMove()
    {
        if (Movement == MovementType.Ballistic)
        {// для полета стрелы используется уравнение равнопеременного движения        
            positionsMassive[0] = transform.position;
            transform.position += new Vector3((InitVelocity * (float)Math.Cos(startAngle) * timer) * Time.deltaTime,
                                              (InitVelocity * (float)Math.Sin(startAngle) * timer - 98f * timer * timer / 2) * Time.deltaTime);
            float Vx = (InitVelocity * (float)Math.Cos(startAngle));
            float Vy = (InitVelocity * (float)Math.Sin(startAngle) - 98f * timer);
            curVelocity = Mathf.Sqrt(Vx * Vx + Vy * Vy);
            positionsMassive[1] = transform.position;
        }
        if (Movement == MovementType.Line)
        {
            transform.position += new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * InitVelocity * Time.deltaTime;
            curVelocity = InitVelocity;
        }
    }

    void DestroyMe () {
		Destroy (gameObject);
	}
    void OnTriggerEnter2D(Collider2D col) {
        // if the projectile hit's a solid object, destroy it
        if (!col.gameObject.CompareTag("DestructibleObjects"))
            { 
            if (col.gameObject.layer == (int)Mathf.Log(solid_layer.value, 2)) {
                    DestroyMe();
                    return;
                }
            }
		var component = col.GetComponent<Health> ();
		// If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
		if (component != null && component != owner && !healthsDamaged.Contains(component)) {
			// Add the health component to the list of damaged healths
			healthsDamaged.Add (component);

			// Apply the damage
			var didDamage = component.TakeDamage (DamageOnHit);
			// Destroy the projectile after applying damage
			if (didDamage) {
				DestroyMe ();
			}
		}
	}
}

