using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MapGenerator : NetworkBehaviour {

	public GameObject[] floorTile;

    public GameObject wallTile;
    public GameObject barrelTile;
    public GameObject boxTile;

    [SyncVar]
    private ArrayList boxes = new ArrayList();
    [SyncVar]
    private ArrayList barrels = new ArrayList();

    [SyncVar]
    public float barrelX;
    [SyncVar]
    public float barrelY;

    [SyncVar]
    public float boxX;
    [SyncVar]
    public float boxY;

    [SyncVar]
    public float wallTileSize = 3.2f;

    [SyncVar]
    public float floorTileSize = 3.2014f;

    public GameObject buildingFloorTile;

    [SyncVar]
    public float buildingFloorTileSize = 4.81f;

    [SyncVar]
    public float complexSizeX;
    [SyncVar]
    public float complexSizeY;

    [SyncVar]
    public float buildingSize = 30;

    [SyncVar]
    public bool initialisedMap = false;

	// Use this for initialization
	void Start () {
        float xStart = (complexSizeX / 2) * floorTileSize;
        float yStart = (complexSizeY / 2) * floorTileSize;
        //groundInitialise(xStart, yStart);
        outerWallGeneration(xStart, yStart);
    }

    void Update()
    {
        if (!initialisedMap)
        {
                float xStart = (complexSizeX / 2) * floorTileSize;
                float yStart = (complexSizeY / 2) * floorTileSize;
                barrelBoxGeneration(xStart, yStart);
                barrelBoxProcedural();
                barrelBoxProcedural();
                buildingGeneration(xStart, yStart);
                initialisedMap = true;
        }
    }

    void outerWallGeneration(float xStart, float yStart) 
    {
        ((GameObject)Instantiate(wallTile, new Vector3(-xStart - wallTileSize, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);
        for (float i = -xStart; i < xStart; i+= wallTileSize)
        {
            ((GameObject)Instantiate(wallTile, new Vector3(i, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);
            ((GameObject)Instantiate(wallTile, new Vector3(i, yStart, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);
        }

        ((GameObject)Instantiate(wallTile, new Vector3(-xStart - wallTileSize, -yStart - wallTileSize, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);
        for (float i = -yStart; i < yStart; i+= wallTileSize)
        {
            ((GameObject)Instantiate(wallTile, new Vector3(-xStart - wallTileSize, i, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);
            ((GameObject)Instantiate(wallTile, new Vector3(xStart, i, 0), wallTile.GetComponent<Transform>().rotation)).transform.SetParent(transform, false);

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
        GameObject boxTileCreated = (GameObject)Instantiate(boxTile, new Vector3(x, y, 0), boxTile.GetComponent<Transform>().rotation);
        NetworkServer.Spawn(boxTileCreated);
        boxTileCreated.transform.SetParent(transform, false);
        return boxTileCreated.GetComponent<BoxCollider2D>();
    }

    BoxCollider2D createBarrelTile(float x, float y)
    {
        GameObject barrelTileCreated = (GameObject) Instantiate(barrelTile, new Vector3(x, y, 0), boxTile.GetComponent<Transform>().rotation);
        NetworkServer.Spawn(barrelTileCreated);
        barrelTileCreated.transform.SetParent(transform, false);
        return barrelTileCreated.GetComponent<BoxCollider2D>();
    }

    Rigidbody2D createBuildingTile(float x, float y)
    {
        GameObject buildingTileCreated = (GameObject)Instantiate(buildingFloorTile, new Vector3(x, y, 0), buildingFloorTile.GetComponent<Transform>().rotation);
        NetworkServer.Spawn(buildingTileCreated);
        buildingTileCreated.transform.SetParent(transform, false);
        return buildingTileCreated.GetComponent<Rigidbody2D>();
    }

    void createWallTile(float x, float y)
    {
        GameObject newWall = (GameObject)Instantiate(wallTile, new Vector3(x, y, 0), wallTile.GetComponent<Transform>().rotation);
        NetworkServer.Spawn(newWall);
        foreach (BoxCollider2D i in boxes)
        {
            if (newWall.GetComponent<BoxCollider2D>().bounds.Intersects(i.bounds))
            {
                Destroy(newWall.gameObject);
                return;
            }
        }
        foreach (BoxCollider2D i in barrels)
        {
            if (newWall.GetComponent<BoxCollider2D>().bounds.Intersects(i.bounds))
            {
                Destroy(newWall.gameObject);
                return;
            }
        }
        newWall.transform.SetParent(transform, false);
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
        int randomFloor = Random.Range(0, 50);
        if (randomFloor > 0 && randomFloor < 49)
            randomFloor = 1;
        else if (randomFloor == 49)
            randomFloor = 2;
        GameObject floorTileClone = (GameObject)Instantiate(floorTile[randomFloor], new Vector3(x, y, 0), floorTile[randomFloor].GetComponent<Transform>().rotation);
        floorTileClone.transform.SetParent(transform, false);
    }
}
