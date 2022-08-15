using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// SquareArray is our component class in charge of creating and managing the
// array of squares!
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

    // Time between ticks in the timescale.
    public float tickTime;

    // Sprites for the squares to access.
    public Sprite liveSprite;
    public Sprite deadSprite;

    // Our timescale component and squares array!
    TimeScale ts;
    GameObject[][] squares;
    internal bool[][] buffer;

    // Start is called before the first frame update
    void Start()
    {
        // ----------INITIALIZATION----------

        // What to do if no square prefab. Try to find one in the files and set up our reference by GUID.
        if(!squarePrefab) {
            Debug.LogWarning("No square prefab specified", squarePrefab);

            string GUID = AssetDatabase.FindAssets("Square")[0];
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            squarePrefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
        }

        // If no sprites set, find them!
        if (!liveSprite) {
            string guid = AssetDatabase.FindAssets(liveSprite.name)[0];
            liveSprite = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Sprite)) as Sprite;
        } if(!deadSprite) {
            string guid = AssetDatabase.FindAssets(deadSprite.name)[0];
            deadSprite = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Sprite)) as Sprite;
        }

        ts = this.gameObject.GetComponent<TimeScale>();
        // Creating our 2D buffer array. For some reason cpp doesn't let you
        // do new bool[width][height]?? idk why. but this works!
        buffer = new bool[gridWidth][];
        for (int i = 0; i < buffer.Length; i++) {
            buffer[i] = new bool[gridHeight];
            for(int j = 0; j < buffer[0].Length; j++) {
                buffer[i][j] = false;
            }
        }

        // ----------GRID CREATION----------

        // We're using vectors to keep track of grid positions!
        Vector3 currentPos = new Vector3
            (
            (this.transform.position.x - (xOffset * (gridWidth / 2))) + (xOffset / 2),
            this.transform.position.y - (yOffset * (gridHeight / 2)),
            this.transform.position.z
            );

        // Instantiate x axis
        squares = new GameObject[gridWidth][];

        for(int x = 0; x < gridWidth; x++) {

            // Instantiate y axis
            squares[x] = new GameObject[gridHeight];

            for(int y = 0; y < gridHeight; y++) {

                // First, we create a square prefab in each position at the currentPos vector!
                squares[x][y] = Instantiate(squarePrefab, currentPos, Quaternion.identity);
                // Then, we set its parent to the square array object!
                squares[x][y].transform.parent = this.gameObject.transform;
                // Finally, we tell it where it is via its location vector!
                squares[x][y].GetComponent<Square>().location = new Vector2Int(x, y);
                buffer[x][y] = squares[x][y].GetComponent<Square>().alive;

                currentPos.y += yOffset;
            }
            // After each column, go to the next x value and reset the y value.
            currentPos.x += xOffset;
            currentPos.y -= (yOffset * gridHeight);
        }
    }

    // changeNeighborStates takes one square and revives it and all neighbors.
    internal void changeNeighborStates(int x, int y) {
        // Reference variable for current square
        Square current;
        // We're using the same x and y offset loop variables from the rule enforcement function.
        for (int xOff = -1; xOff <= 1; xOff++) {
            for (int yOff = -1; yOff <= 1; yOff++) {
                // Storing these for quick comparisons
                int indX = xOff + x;
                int indY = yOff + y;
                if (
                    // Corner cases
                    (xOff == 0 && yOff == 0)
                    || (indX < 0)
                    || (indY < 0)
                    || (indX >= squares.Length)
                    || (indY >= squares[0].Length)
                    ) continue;
                // Actual purpose of the function!
                if (squares[indX][indY]) {
                    current = squares[indX][indY].GetComponent<Square>();
                    if (!current.alive) current.changeState(true);
                }
            }
        }
    }

    // Tick variables
    float lastTick = -1.0f;
    float currentTime;

    // Update is called once per frame
    void Update()
    {
        // Last tick tracks when the board last progressed, currentTime
        // is the current time for comparison.
        currentTime = Time.time;

        if(Input.GetKeyDown(KeyCode.R)) {
            Randomize();
        } if(Input.GetKeyDown(KeyCode.C)) {
            Clear();
        }

        // State machine! Our TimeScale object takes input while we check its state
        switch(ts.state) {              //      STATES

            case 0:                     // 0 - Idle
                // Do nothing!
                break;
            case 1:                     // 1 - Play
                // Every {tickTime} seconds, progress the board by 1 tick.
                if(lastTick == -1.0f || currentTime - lastTick >= tickTime) {
                    ruleEnforcement();
                }
                break;
            case 2:                     // 2 - Step forward
                // Progress by 1 tick then stop.
                ruleEnforcement();
                ts.state = 0;
                break;
            case 3:                     // 3 - Step backward
                print("no.");
                break;
            default:
                break;
        }
    }

    // Rule Enforcement function, progresses the board by 1 tick.
    // New modification: ONLY reading from the board is important now.
    // The only thing we should be writing to is the buffer!
    void ruleEnforcement() {
        // Reference variable
        Square current;
        // Neighbor count!
        int neighbors = 0;

        // Loop through the grid...
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                // Store current square component
                current = squares[x][y].GetComponent<Square>();
                // ...and store number of living neighbors
                neighbors = getAliveNeighbors(x, y);

                // Now that we use a buffer, we update their status in the
                // buffer, not the grid directly.
                // If alive...
                if (current.alive) {
                    // Underpopulation
                    if (neighbors < 2) buffer[x][y] = false;
                    // Overpopulation
                    if (neighbors > 3) buffer[x][y] = false;
                } else if /* dead and */(neighbors == 3) {
                    buffer[x][y] = true;
                }
            }
        }
        Tick();
        lastTick = Time.time;
    }

    // Function to count the number of living neighbors around a square.
    internal int getAliveNeighbors(int x, int y) {
        // Neighbor count
        int numAlive = 0;
        int indX;
        int indY;

        // Loop from bottom left to top right of our location
        for (int xOff = -1; xOff <= 1; xOff++) {
            for(int yOff = -1; yOff <= 1; yOff++) {
                // Store offset coordinates for neighbors
                indX = xOff + x;
                indY = yOff + y;
                if (
                    // Checking if we're looking at ourself or an out of bounds index.
                    (xOff == 0 && yOff == 0)
                    || (indX < 0)
                    || (indY < 0)
                    || (indX >= squares.Length)
                    || (indY >= squares[0].Length)
                    ) continue;
                // And count it if it's alive!
                if (squares[indX][indY].GetComponent<Square>().alive == true) {
                    numAlive++;
                }
            }
        }
        // Return number of alive neighbors!
        return numAlive;
    }

    void Randomize() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                buffer[x][y] = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
                squares[x][y].GetComponent<Square>().alive = buffer[x][y];
            }
        }
    }

    void Clear() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                squares[x][y].GetComponent<Square>().alive = false;
                buffer[x][y] = false;
            }
        }
    }


    // Tick function. Reads from the buffer and updates the board.
    void Tick() {
        Square cur;

        for(int x = 0; x < gridWidth; x++) {
            for(int y = 0; y < gridHeight; y++) {
                cur = squares[x][y].GetComponent<Square>();
                if (cur.alive != buffer[x][y]) {
                    squares[x][y].GetComponent<Square>().changeState(buffer[x][y]);
                }
            }
        }
    }

}
