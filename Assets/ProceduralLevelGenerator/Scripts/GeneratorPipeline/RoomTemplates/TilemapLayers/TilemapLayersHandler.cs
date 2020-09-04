namespace Assets.ProceduralLevelGenerator.Scripts.GeneratorPipeline.RoomTemplates.TilemapLayers
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Tilemaps;

	/// <summary>
	/// Basic implementation of tilemap layers handler.
	/// </summary>
	[CreateAssetMenu(menuName = "Dungeon generator/Pipeline/Tilemap layers handler", fileName = "TilemapLayersHandler")]
	public class TilemapLayersHandler : AbstractTilemapLayersHandler
	{
		/// <summary>
		/// Initializes individual tilemap layers.
		/// </summary>
		/// <param name="gameObject"></param>
		public override void InitializeTilemaps(GameObject gameObject)
		{
			var wallsTilemapObject = CreateTilemapGameObject("Walls", gameObject, 1);
			AddCompositeCollider(wallsTilemapObject);

			var backTilemapObject = CreateTilemapGameObject("Back", gameObject, -3);
            backTilemapObject.AddComponent<TilemapCollider2D>();
            backTilemapObject.GetComponent<TilemapCollider2D>().isTrigger = true;
            backTilemapObject.transform.tag = "BackGround";
            //backTilemapObject.GetComponent<TilemapCollider2D>().isTrigger = true;

            var frontTilemapObject = CreateTilemapGameObject("Front", gameObject, 3);

            var collideableTilemapObject = CreateTilemapGameObject("Collideable", gameObject, 2);
			AddCompositeCollider(collideableTilemapObject);

            var other1TilemapObject = CreateTilemapGameObject("Ladder", gameObject, -2);
            other1TilemapObject.AddComponent<TilemapCollider2D>();
            other1TilemapObject.GetComponent<TilemapCollider2D>().isTrigger = true;


            var other2TilemapObject = CreateTilemapGameObject("OneWayPlatforms", gameObject, -2);
            other2TilemapObject.AddComponent<TilemapCollider2D>();
            other2TilemapObject.GetComponent<TilemapCollider2D>().isTrigger = true;

            var other3TilemapObject = CreateTilemapGameObject("Bumpers", gameObject, -4);
            other3TilemapObject.AddComponent<TilemapCollider2D>();

            var other4TilemapObject = CreateTilemapGameObject("Prefabs", gameObject, 3);

        }

		protected GameObject CreateTilemapGameObject(string name, GameObject parentObject, int sortingOrder)
		{
			var tilemapObject = new GameObject(name);
            tilemapObject.transform.localScale = new Vector3(1, 1, 0);

            tilemapObject.transform.SetParent(parentObject.transform);


            if (name == "Walls")
            {
                tilemapObject.layer = LayerMask.NameToLayer("Solid");
            }


            if (name == "Ladder")
            {
                tilemapObject.layer = LayerMask.NameToLayer("Ladders");
            }

            if (name == "OneWayPlatforms")
            {
                tilemapObject.layer = LayerMask.NameToLayer("OneWay");
            }

            if (name == "Bumpers")
            {
                tilemapObject.layer = LayerMask.NameToLayer("Bumper");
            }

            if (name == "Prefabs")
            {
                foreach (Transform child in tilemapObject.transform)
                {
                    child.transform.SetParent(parentObject.transform);
                }
            }

            var tilemap = tilemapObject.AddComponent<Tilemap>();
			var tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();
			tilemapRenderer.sortingOrder = sortingOrder;




                return tilemapObject;
		}

		protected void AddCompositeCollider(GameObject gameObject)
		{
			var tilemapCollider2D = gameObject.AddComponent<TilemapCollider2D>();
			tilemapCollider2D.usedByComposite = true;

			gameObject.AddComponent<CompositeCollider2D>();
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		}
	}
}