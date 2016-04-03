using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Rigidbody2D[] floorTile;

    public BoxCollider2D wallTile;
    public BoxCollider2D barrelTile;
    public BoxCollider2D boxTile;

    private ArrayList boxes = new ArrayList();
    private ArrayList barrels = new ArrayList();

    public float barrelX;
    public float barrelY;

    public float boxX;
    public float boxY;

    public float wallTileSize = 3.2f;

    public float floorTileSize = 3.2014f;

    public Rigidbody2D buildingFloorTile;

    public float buildingFloorTileSize = 4.81f;

	public float complexSizeX;
	public float complexSizeY;

    public float buildingSize = 30;

	// Use this for initialization
	void Start () {
		float xStart = (complexSizeX / 2) * floorTileSize;
		float yStart = (complexSizeY / 2) * floorTileSize;
        groundInitialise(xStart, yStart);
        barrelBoxGeneration(xStart, yStart);
        barrelBoxProcedural();
        barrelBoxProcedural();
        outerWallGeneration(xStart, yStart);
        buildingGeneration(xStart, yStart);
    }

    void outerWallGeneration(float xStart, float yStart) 
    {
        Instantiate(wallTile, new Vector3(-xStart - wallTileSize, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation);
        for (float i = -xStart; i < xStart; i+= wallTileSize)
        {
            Instantiate(wallTile, new Vector3(i, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation);
            Instantiate(wallTile, new Vector3(i, yStart, 0), wallTile.GetComponent<Transform>().rotation);
        }

        Instantiate(wallTile, new Vector3(-xStart - wallTileSize, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation);
        for (float i = -yStart; i < yStart; i+= wallTileSize)
        {
            Instantiate(wallTile, new Vector3(-xStart - wallTileSize, i, 0), wallTile.GetComponent<Transform>().rotation);
            Instantiate(wallTile, new Vector3(xStart, i, 0), wallTile.GetComponent<Transform>().rotation);

        }
    }

    void barrelBoxGeneration(float xStart, float yStart)
    {
        for (int i = (int)-xStart; i < xStart; i+= 4)
        {
            for (int j = (int)-yStart; j < yStart; j += 4)
            {
                float random = Random.Range(0, 200);
                if (random == 98)
                {
                    boxes.Add(createBoxTile(i, j));
                }
                if (random == 99)
                {
                    barrels.Add(createBarrelTile(i, j));
                }
            }
        }
    }

    void barrelBoxProcedural()
    {
        ArrayList boxesToAdd = new ArrayList();
        ArrayList barrelsToAdd = new ArrayList();
        foreach(BoxCollider2D i in boxes)
        {
            if (Random.Range(0,3) == 1)
            {
                if (Random.Range(0, 2) == 1)
                    boxesToAdd.Add(createBoxTile(i.GetComponent<Transform>().position.x - boxX, i.GetComponent<Transform>().position.y));
                if (Random.Range(0, 2) == 1)
                    boxesToAdd.Add(createBoxTile(i.GetComponent<Transform>().position.x + boxX, i.GetComponent<Transform>().position.y));
                if (Random.Range(0, 2) == 1)
                    boxesToAdd.Add(createBoxTile(i.GetComponent<Transform>().position.x, i.GetComponent<Transform>().position.y + boxY));
                if (Random.Range(0, 2) == 1)
                    boxesToAdd.Add(createBoxTile(i.GetComponent<Transform>().position.x, i.GetComponent<Transform>().position.y - boxY));
            }
            else
            {
                Destroy(i.gameObject);
            }
        }
        foreach (BoxCollider2D i in boxesToAdd)
            boxes.Add(i);
        foreach (BoxCollider2D i in barrels)
        {
            if (Random.Range(0, 3) == 1)
            {
                if (Random.Range(0, 2) == 1)
                    barrelsToAdd.Add(createBarrelTile(i.GetComponent<Transform>().position.x - barrelX, i.GetComponent<Transform>().position.y));
                if (Random.Range(0, 2) == 1)
                    barrelsToAdd.Add(createBarrelTile(i.GetComponent<Transform>().position.x + barrelX, i.GetComponent<Transform>().position.y));
                if (Random.Range(0, 2) == 1)
                    barrelsToAdd.Add(createBarrelTile(i.GetComponent<Transform>().position.x, i.GetComponent<Transform>().position.y + barrelY));
                if (Random.Range(0, 2) == 1)
                    barrelsToAdd.Add(createBarrelTile(i.GetComponent<Transform>().position.x, i.GetComponent<Transform>().position.y - barrelY));
            }
            else
            {
                Destroy(i.gameObject);
            }
        }
        foreach (BoxCollider2D i in barrelsToAdd)
            barrels.Add(i);
    }

    void buildingGeneration(float xStart, float yStart)
    {
        float xQuadrantSize = complexSizeX / 2;
        float yQuadrantSize = complexSizeY / 3;
        Vector2[,] buildingLocations = new Vector2[2,3];
        ArrayList buildingTilePositions = new ArrayList();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buildingLocations[i, j] = new Vector2(Random.Range(-xStart + (xQuadrantSize * i * floorTileSize) + (xStart / 2), -xStart + (xQuadrantSize * (i + 1) * floorTileSize) - (xStart / 2)), Random.Range(-yStart + (yQuadrantSize * floorTileSize * j) + (yStart / 2), -yStart + (yQuadrantSize * floorTileSize * (j + 1)) - (yStart / 2)));
                Rigidbody2D[] buildingTilesArray = new Rigidbody2D[(int)buildingSize];
                buildingTilesArray[0] = createBuildingTile(buildingLocations[i, j].x, buildingLocations[i, j].y);
                buildingTilePositions.Add(buildingTilesArray[0].position);
                int currentTile = 1;
                while (buildingSize > currentTile)
                {
                    for (int k = 0; k < currentTile; k++)
                    {
                        //left
                        if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[k].position.x - buildingFloorTileSize, buildingTilesArray[k].position.y)))
                        {
                            if (Random.Range(0,2) == 1)
                            {
                                buildingTilesArray[currentTile] = createBuildingTile(buildingTilesArray[k].position.x - buildingFloorTileSize, buildingTilesArray[k].position.y);
                                buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                                currentTile += 1;
                            }
                        }
                        //right
                        if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[k].position.x + buildingFloorTileSize, buildingTilesArray[k].position.y)))
                        {
                            if (Random.Range(0, 2) == 1)
                            {
                                buildingTilesArray[currentTile] = createBuildingTile(buildingTilesArray[k].position.x + buildingFloorTileSize, buildingTilesArray[k].position.y);
                                buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                                currentTile += 1;
                            }
                        }
                        //up
                        if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[k].position.x, buildingTilesArray[k].position.y + buildingFloorTileSize)))
                        {
                            if (Random.Range(0, 2) == 1)
                            {
                                buildingTilesArray[currentTile] = createBuildingTile(buildingTilesArray[k].position.x, buildingTilesArray[k].position.y + buildingFloorTileSize);
                                buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                                currentTile += 1;
                            }
                        }
                        //down
                        if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[k].position.x, buildingTilesArray[k].position.y - buildingFloorTileSize)))
                        {
                            if (Random.Range(0, 2) == 1)
                            {
                                buildingTilesArray[currentTile] = createBuildingTile(buildingTilesArray[k].position.x, buildingTilesArray[k].position.y - buildingFloorTileSize);
                                buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                                currentTile += 1;
                            }
                        }
                    }
                }
                currentTile = 0;
                //Wall placement
                while (buildingSize > currentTile)
                {
                    //left
                    if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[currentTile].position.x - buildingFloorTileSize, buildingTilesArray[currentTile].position.y)))
                    {
                        if (currentTile <= buildingSize - 5)
                        {
                            createWallTile(buildingTilesArray[currentTile].position.x - buildingFloorTileSize, buildingTilesArray[currentTile].position.y);
                            buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                        }
                    }
                    //right
                    if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[currentTile].position.x + buildingFloorTileSize, buildingTilesArray[currentTile].position.y)))
                    {
                        if (currentTile <= buildingSize - 5)
                        {
                            createWallTile(buildingTilesArray[currentTile].position.x + buildingFloorTileSize, buildingTilesArray[currentTile].position.y);
                            buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                        }
                    }
                    //up
                    if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[currentTile].position.x, buildingTilesArray[currentTile].position.y + buildingFloorTileSize)))
                    {
                        if (currentTile <= buildingSize - 5)
                        {
                            createWallTile(buildingTilesArray[currentTile].position.x, buildingTilesArray[currentTile].position.y + buildingFloorTileSize);
                            buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                        }
                    }
                    //down
                    if (currentTile < buildingSize && tileDoesNotExist(buildingTilePositions, new Vector2(buildingTilesArray[currentTile].position.x, buildingTilesArray[currentTile].position.y - buildingFloorTileSize)))
                    {
                        if (currentTile <= buildingSize - 5)
                        {
                            createWallTile(buildingTilesArray[currentTile].position.x, buildingTilesArray[currentTile].position.y - buildingFloorTileSize);
                            buildingTilePositions.Add(buildingTilesArray[currentTile].position);
                        }
                    }
                    currentTile += 1;
                }
            }
        }
    }
    
    BoxCollider2D createBoxTile(float x, float y)
    {
        return (BoxCollider2D)Instantiate(boxTile, new Vector3(x, y, 0), boxTile.GetComponent<Transform>().rotation);
    }

    BoxCollider2D createBarrelTile(float x, float y)
    {
        return (BoxCollider2D)Instantiate(barrelTile, new Vector3(x, y, 0), boxTile.GetComponent<Transform>().rotation);
    }

    Rigidbody2D createBuildingTile(float x, float y)
    {
        return (Rigidbody2D)Instantiate(buildingFloorTile, new Vector3(x, y, 0), buildingFloorTile.GetComponent<Transform>().rotation);
    }

    void createWallTile(float x, float y)
    {
        BoxCollider2D newWall = (BoxCollider2D)Instantiate(wallTile, new Vector3(x, y, 0), wallTile.GetComponent<Transform>().rotation);
        foreach(BoxCollider2D i in boxes)
        {
            if (newWall.bounds.Intersects(i.bounds))
            {
                Destroy(newWall.gameObject);
                return;
            }
        }
        foreach (BoxCollider2D i in barrels)
        {
            if (newWall.bounds.Intersects(i.bounds))
            {
                Destroy(newWall.gameObject);
                return;
            }
        }
    }

    bool tileDoesNotExist(ArrayList tilePositions, Vector2 check)
    {
        foreach (Vector2 i in tilePositions)
        {
            if (i == check)
            {
                return false;
            }
        }
        return true;
    }

    void groundInitialise(float xStart, float yStart)
    {
        for (int i = 0; i < complexSizeX; i++)
        {
            for (int j = 0; j < complexSizeY; j++)
            {
                spawnNewFloorTile((-xStart) + (i * floorTileSize), (-yStart) + (j * floorTileSize));
            }
        }
    }

	void spawnNewFloorTile(float x, float y) {
		Rigidbody2D floorTileClone = (Rigidbody2D)Instantiate (floorTile[Random.Range(0,3)], new Vector3 (x, y, 0), floorTile[Random.Range(0, 3)].GetComponent<Transform> ().rotation);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
