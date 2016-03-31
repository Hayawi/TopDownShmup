using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Rigidbody2D floorTile;

	public float complexSizeX;
	public float complexSizeY;

	// Use this for initialization
	void Start () {
		float xStart = (complexSizeX / 2) * 4.81f;
		float yStart = (complexSizeY / 2) * 4.81f;
		for (int i = 0; i < complexSizeX; i++) {
			for (int j = 0; j < complexSizeY; j++) {
				spawnNewFloorTile ((-xStart) + (i * 4.81f), (-yStart) + (j * 4.81f)); 
			}
		}
	}

	void spawnNewFloorTile(float x, float y) {
		Rigidbody2D floorTileClone = (Rigidbody2D)Instantiate (floorTile, new Vector3 (x, y, 0), floorTile.GetComponent<Transform> ().rotation);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
