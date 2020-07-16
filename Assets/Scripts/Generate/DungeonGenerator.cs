using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public int widthMinRoom;
    public int widthMaxRoom;
    public int heightMinRoom;
    public int heightMaxRoom;

    public int minCorridorLength;
    public int maxCorridorLength;
    public int maxFeatures;
    int countFeatures;
    public List<Feature> allFeatures;

    public Tile wall;
    public Tile wall2;
    public Tile flor;
    public Tile flor2;
    public Tilemap SolidMap;
    public Tilemap BackGroundMap;
    public GameObject[] roomPrefabs;
    public GameObject grid;

    public void InitializeDungeon()
    {
        MapManager.map = new MyTile[mapWidth, mapHeight];
    }

    public void GenerateDungeon()
    {
        GenerateFeature("Room", new Wall(), true);

        Feature originFeature;

        string type;

        for (int i = 0; i < 5000; i++)
        {

            int rnd = Random.Range(0, allFeatures.Count - 1);

            if (allFeatures.Count == 1)
            {
                originFeature = allFeatures[0];
            }
            else originFeature = allFeatures[rnd];

            if (allFeatures[rnd].numberOfExits != 0 && allFeatures[rnd].type == "Corridor")
            {
                continue;
            }

            if (allFeatures[rnd].numberOfExits >= 2 && allFeatures[rnd].type == "Room")
            {
                continue;
            }

            Wall wall = ChoseWall(originFeature);

            if (wall == null) continue;

            if (originFeature.type == "Room")
            {
                type = "Corridor";
            }
            else
            {
                if (Random.Range(0, 100) < 60)
                {
                    type = "Room";
                }
                else
                {
                    type = "Corridor";
                }
            }

            if (GenerateFeature(type, wall))
            {
                originFeature.numberOfExits++;
                if (originFeature.walls[0].hasFeature)
                    originFeature.numberOfExitSouth++;
                if (originFeature.walls[1].hasFeature)
                    originFeature.numberOfExitNorth++;
                if (originFeature.walls[2].hasFeature)
                    originFeature.numberOfExitWest++;
                if (originFeature.walls[3].hasFeature)
                    originFeature.numberOfExitEast++;

                if (originFeature.width == 20 & originFeature.height == 20)
                {
                    originFeature.prefabMaps = true;
                    //RemoveRoom(originFeature);
                }
            }
            if (countFeatures >= maxFeatures)
            {
                break;
            }
        }

        //for (int i = 0; i < allFeatures.Count-1; i++)
        //{
        //    if (allFeatures[i].numberOfExits <= 1 && allFeatures[i].type == "Corridor")
        //    {
        //        allFeatures.Remove(allFeatures[i]);
        //    }
        //}

        DrawMap();
    }

    bool GenerateFeature(string type, Wall wall, bool isFirst = false)
    {
        Feature room = new Feature();
        room.positions = new List<Vector2Int>();

        int roomWidth = 0;
        int roomHeight = 0;

        if (type == "Room")
        {
            roomWidth = Random.Range(widthMinRoom, widthMaxRoom);
            roomHeight = Random.Range(heightMinRoom, heightMaxRoom);
        }
        else
        {
            switch (wall.direction)
            {
                case "South":
                    roomWidth = Random.Range(3, 11);
                    roomHeight = Random.Range(minCorridorLength, maxCorridorLength);
                    break;
                case "North":
                    roomWidth = Random.Range(3, 11);
                    roomHeight = Random.Range(minCorridorLength, maxCorridorLength);
                    break;
                case "West":
                    roomWidth = Random.Range(minCorridorLength, maxCorridorLength);
                    roomHeight = Random.Range(7, 11);
                    break;
                case "East":
                    roomWidth = Random.Range(minCorridorLength, maxCorridorLength);
                    roomHeight = Random.Range(7, 11);
                    break;

            }
        }

        int xStartingPoint;
        int yStartingPoint;

        if (isFirst)
        {
            xStartingPoint = mapWidth / 2;
            yStartingPoint = mapHeight / 2;
        }
        else
        {
            int id;
            if (wall.positions.Count == 3) id = 1;
            else id = Random.Range(3, wall.positions.Count - 4);

            xStartingPoint = wall.positions[id].x;
            yStartingPoint = wall.positions[id].y;
        }

        Vector2Int lastWallPosition = new Vector2Int(xStartingPoint, yStartingPoint);

        if (isFirst)
        {
            xStartingPoint -= Random.Range(1, roomWidth);
            yStartingPoint -= Random.Range(1, roomHeight);
        }
        else
        {
            switch (wall.direction)
            {
                case "South":
                    if (type == "Room") xStartingPoint -= Random.Range(1, roomWidth - 2);
                    else xStartingPoint--;
                    yStartingPoint -= Random.Range(1, roomHeight - 2);
                    break;
                case "North":
                    if (type == "Room") xStartingPoint -= Random.Range(1, roomWidth - 2);
                    else xStartingPoint--;
                    yStartingPoint++;
                    break;
                case "West":
                    xStartingPoint -= roomWidth;
                    if (type == "Room") yStartingPoint -= Random.Range(1, roomHeight - 2);
                    else yStartingPoint--;
                    break;
                case "East":
                    xStartingPoint++;
                    if (type == "Room") yStartingPoint -= Random.Range(1, roomHeight - 2);
                    else yStartingPoint--;
                    break;
            }
        }

        if (!CheckIfHasSpace(new Vector2Int(xStartingPoint, yStartingPoint), new Vector2Int(xStartingPoint + roomWidth - 1, yStartingPoint + roomHeight - 1)))
        {
            return false;
        }

        room.walls = new Wall[4];

        for (int i = 0; i < room.walls.Length; i++)
        {
            room.walls[i] = new Wall();
            room.walls[i].positions = new List<Vector2Int>();
            room.walls[i].length = 0;

            switch (i)
            {
                case 0:
                    room.walls[i].direction = "South";
                    break;
                case 1:
                    room.walls[i].direction = "North";
                    break;
                case 2:
                    room.walls[i].direction = "West";
                    break;
                case 3:
                    room.walls[i].direction = "East";
                    break;
            }
        }

        for (int y = 0; y < roomHeight; y++)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                Vector2Int position = new Vector2Int();
                position.x = xStartingPoint + x;
                position.y = yStartingPoint + y;

                room.positions.Add(position);

                MapManager.map[position.x, position.y] = new MyTile();
                MapManager.map[position.x, position.y].xPosition = position.x;
                MapManager.map[position.x, position.y].yPosition = position.y;

                if (((x % 2 == 1 & y % 2 != 1) || (x % 2 != 1 & y % 2 == 1)))
                {
                    MapManager.map[position.x, position.y].type = "Floor";
                }
                else
                {
                    MapManager.map[position.x, position.y].type = "Floor2";
                }

                if (y == 0)
                {
                    room.walls[0].positions.Add(position);
                    room.walls[0].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                }
                if (y == (roomHeight - 1))
                {
                    room.walls[1].positions.Add(position);
                    room.walls[1].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                }
                if (x == 0)
                {
                    room.walls[2].positions.Add(position);
                    room.walls[2].length++;
                    MapManager.map[position.x, position.y].type = "Wall2";
                }
                if (x == (roomWidth - 1))
                {
                    room.walls[3].positions.Add(position);
                    room.walls[3].length++;
                    MapManager.map[position.x, position.y].type = "Wall2";
                }
                //все углы
    }
}

        if (!isFirst)
        {
            MapManager.map[lastWallPosition.x, lastWallPosition.y].type = "Floor";
            switch (wall.direction)
            {
                case "South":
                    MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].type = "Floor";
                    room.numberOfExitSouth++;
                    break;
                case "North":
                    MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].type = "Floor";
                    room.numberOfExitNorth++;
                    break;
                case "West":
                    MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].type = "Floor";
						room.numberOfExitWest++;
                        MapManager.map[lastWallPosition.x - 1, lastWallPosition.y + 1].type = "Floor";
                        MapManager.map[lastWallPosition.x - 1, lastWallPosition.y + 2].type = "Floor";

                        MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].type = "Floor";
                        MapManager.map[lastWallPosition.x, lastWallPosition.y + 2].type = "Floor";
                    break;
                case "East":
                    MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].type = "Floor";
						room.numberOfExitEast++;
                        MapManager.map[lastWallPosition.x + 1, lastWallPosition.y + 1].type = "Floor";
                        MapManager.map[lastWallPosition.x + 1, lastWallPosition.y + 2].type = "Floor";

                        MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].type = "Floor";
                        MapManager.map[lastWallPosition.x, lastWallPosition.y + 2].type = "Floor";
                    break;
            }
        }

        room.width = roomWidth;
        room.height = roomHeight;
        room.type = type;
        allFeatures.Add(room);
        countFeatures++;
        return true;
    }

    bool CheckIfHasSpace(Vector2Int start, Vector2Int end)
{
    for (int y = start.y; y <= end.y; y++)
    {
        for (int x = start.x; x <= end.x; x++)
        {
            if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) return false;
            if (MapManager.map[x, y] != null) return false;
        }
    }

    return true;
}

Wall ChoseWall(Feature feature)
{
    for (int i = 0; i < 10; i++)
    {
        int id = Random.Range(0, 100) / 25;
        if (!feature.walls[id].hasFeature)
        {
            return feature.walls[id];
        }
    }
    return null;
}

void RemoveRoom(Feature feature)
{

    for (int y = 0; y < feature.positions.Count; y++)
    {
        for (int x = 0; x < feature.positions.Count; x++)
        {
            MapManager.map[feature.positions[x].x, feature.positions[y].y].type = "Empty";

            if (x == (int)feature.positions.Count / 2 && y == (int)feature.positions.Count / 2)
            {
                MapManager.map[feature.positions[x].x, feature.positions[y].y].type = "Centr";
            }

        }
    }
}

void DrawMap()
{

    for (int y = (mapHeight - 1); y >= 0; y--)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            if (MapManager.map[x, y] != null)
            {
                switch (MapManager.map[x, y].type)
                {
                    case "Wall":
                        SolidMap.SetTile(new Vector3Int(x, y, 0), wall);
                        break;
                    case "Wall2":
                        SolidMap.SetTile(new Vector3Int(x, y, 0), wall2);
                        break;
                    case "Floor":
                        BackGroundMap.SetTile(new Vector3Int(x, y, 0), flor);
                        break;
                    case "Floor2":
                        BackGroundMap.SetTile(new Vector3Int(x, y, 0), flor2);
                        break;
                    //case "Centr":
                    //    SolidMap.SetTile(new Vector3Int(x, y, 0), flor);
                    //    break;
                }
            }
            else
            {
                //пустота
            }

            if (x == (mapWidth - 1))
            {
                // новая строка?
            }
        }
    }
}

}

