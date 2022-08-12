using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SquareArray : MonoBehaviour {

    // Prefab to create.
    [Tooltip("Prefab to make the grid out of.")]
    public GameObject squarePrefab;
    // "Resolution" of the grid
    [Tooltip("Default value: 112")]
    public int gridWidth;
    [Tooltip("Default value: 55")]
    public int gridHeight;
    // Distance between squares
    [Tooltip("Default value: 0.18")]
    public float xOffset;
    [Tooltip("Default value: 0.18")]
    public float yOffset;

    public Sprite liveSprite;
    public Sprite deadSprite;

    GameObject[][] squares;

    public SquareArray() {

    }

    // Start is called before the first frame update
    void Start()
    {
        // What to do if no square prefab. Try to find one in the files and set up our reference by GUID.
        if(!squarePrefab) {
            Debug.LogWarning("No square prefab specified", squarePrefab);

            string GUID = AssetDatabase.FindAssets("Square")[0];
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            squarePrefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
        }

        string guid = AssetDatabase.FindAssets(liveSprite.name)[0];
        liveSprite = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Sprite)) as Sprite;
        guid = AssetDatabase.FindAssets(deadSprite.name)[0];
        deadSprite = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Sprite)) as Sprite;

        // Start in the bottom left corner, all negative quadrant.
        Vector3 currentPos = new Vector3
            (
            (this.transform.position.x - (xOffset * (gridWidth / 2))) + (xOffset / 2),
            this.transform.position.y - (yOffset * (gridHeight / 2)),
            this.transform.position.z
            );
        squares = new GameObject[gridWidth][];
        for(int x = 0; x < gridWidth; x++) {
            squares[x] = new GameObject[gridHeight];
            for(int y = 0; y < gridHeight; y++) {
                squares[x][y] = Instantiate(squarePrefab, currentPos, Quaternion.identity);
                squares[x][y].transform.parent = this.gameObject.transform;
                squares[x][y].GetComponent<Square>().location = new Vector2Int(x, y);
                currentPos.y += yOffset;
            }
            currentPos.x += xOffset;
            currentPos.y -= (yOffset * gridHeight);
        }
    }

    internal void changeNeighborStates(int x, int y) {
        Square current;
        for (int xOff = -1; xOff <= 1; xOff++) {
            for (int yOff = -1; yOff <= 1; yOff++) {
                int indX = xOff + x;
                int indY = yOff + y;
                if (
                    (xOff == 0 && yOff == 0)
                    || (indX < 0)
                    || (indY < 0)
                    || (indX >= squares.Length)
                    || (indY >= squares[0].Length)
                    ) continue;
                if (squares[x + xOff][y + yOff]) {
                    current = squares[x + xOff][y + yOff].GetComponent<Square>();
                    if (!current.alive) current.changeState(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ----------RULE ENFORCEMENT----------
        Square current;
        int neighbors;
        for(int x = 0; x < gridWidth; x++) {
            for(int y = 0; y < gridHeight; y++) {
                current = squares[x][y].GetComponent<Square>();
                neighbors = getAliveNeighbors(x, y);
                // If alive...
                if(current.alive) {
                    // Underpopulation
                    if (neighbors < 2) current.changeState(false);
                    // Overpopulation
                    if (neighbors > 3) current.changeState(false);
                }
                // If dead...
                else {
                    // Birth
                    if (neighbors == 3) current.changeState(true);
                }
            }
        }

        // ----------TIME CONTROL----------
    }

    internal int getAliveNeighbors(int x, int y) {
        Square current;
        int numAlive = 0;

        for(int xOff = -1; xOff <= 1; xOff++) {
            for(int yOff = -1; yOff <= 1; yOff++) {
                int indX = xOff + x;
                int indY = yOff + y;
                if (
                    // Checking if we're looking at ourself or an out of bounds index.
                    (xOff == 0 && yOff == 0)
                    || (indX < 0)
                    || (indY < 0)
                    || (indX >= squares.Length)
                    || (indY >= squares[0].Length)
                    ) continue;
                if (squares[x + xOff][y + yOff]) {
                    current = squares[x + xOff][y + yOff].GetComponent<Square>();
                    if (current.alive) numAlive++;
                }
            }
        }
        return numAlive;
    }
}
