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
    [Tooltip("Default value: 50")]
    [Range(1, 500)]
    public int gridWidth;
    [Tooltip("Default value: 50")]
    [Range(1, 500)]
    public int gridHeight;

    // Distance between squares
    [Tooltip("Default value: 0.18")]
    public float xOffset;
    [Tooltip("Default value: 0.18")]
    public float yOffset;

    internal int generation;

    // Time between ticks in the timescale.
    public float tickTime;

    // Sprites for the squares to access.
    public Sprite liveSprite;
    public Sprite deadSprite;

    // Default state to initialize all squares in.
    public bool defaultState;

    // Our timescale component and squares array!
    TimeScale ts;
    GameObject[][] squares;
    internal bool[][] grid;
    internal bool[][] buffer;

    // Start is called before the first frame update
    void Start()
    {
        // ----------INITIALIZATION----------

        // What to do if no square prefab!
        if(squarePrefab == null) {
            Debug.LogError("No square prefab specified", squarePrefab);
        }

        // If no sprites set
        if (liveSprite == null) {
            Debug.LogError("No alive sprite set", liveSprite);
        } if(deadSprite == null) {
            Debug.LogError("No dead sprite set", deadSprite);
        }

        ts = this.gameObject.GetComponent<TimeScale>();
        generation = 0;

        int prefsWidth = PlayerPrefs.GetInt("gWidth");
        if (prefsWidth <= 0 || prefsWidth > 500) {
            gridWidth = 50;
        } else {
            gridWidth = PlayerPrefs.GetInt("gWidth");
        }

        int prefsHeight = PlayerPrefs.GetInt("gHeight");
        if (prefsHeight <= 0 || prefsHeight > 500) {
            gridHeight = 50;
        } else {
            gridHeight = PlayerPrefs.GetInt("gHeight");
        }

        // Creating our 2D buffer array. For some reason cpp doesn't let you
        // do new bool[width][height]?? idk why. but this works!

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
        buffer = new bool[gridWidth][];
        grid = new bool[gridWidth][];

        for (int x = 0; x < gridWidth; x++) {

            // Instantiate y axis
            squares[x] = new GameObject[gridHeight];
            buffer[x] = new bool[gridHeight];
            grid[x] = new bool[gridHeight];

            for(int y = 0; y < gridHeight; y++) {

                // First, we create a square prefab in each position at the currentPos vector!
                squares[x][y] = Instantiate(squarePrefab, currentPos, Quaternion.identity);
                // Then, we set its parent to the square array object!
                squares[x][y].transform.parent = this.gameObject.transform;
                // Finally, we tell it where it is via its location vector!
                squares[x][y].GetComponent<Square>().location = new Vector2Int(x, y);

                grid[x][y] = defaultState;
                buffer[x][y] = grid[x][y];

                currentPos.y += yOffset;
            }
            // After each column, go to the next x value and reset the y value.
            currentPos.x += xOffset;
            currentPos.y -= (yOffset * gridHeight);
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
        // Neighbor count!
        int neighbors = 0;

        // Loop through the grid...
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                // ...and store number of living neighbors
                neighbors = getAliveNeighbors(x, y);

                // Now that we use a buffer, we update their status in the
                // buffer, not the grid directly.
                // If alive...
                if (grid[x][y]) {
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
                    || (indX >= grid.Length)
                    || (indY >= grid[0].Length)
                    ) continue;
                // And count it if it's alive!
                if (grid[indX][indY]) {
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
                grid[x][y] = buffer[x][y];
            }
        }
        generation = 0;
    }

    void Clear() {
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                grid[x][y] = false;
                buffer[x][y] = false;
            }
        }
        generation = 0;
    }


    // Tick function. Reads from the buffer and updates the board.
    void Tick() {
        for(int x = 0; x < gridWidth; x++)
            for(int y = 0; y < gridHeight; y++)
                if (grid[x][y] != buffer[x][y])
                    grid[x][y] = buffer[x][y];
        generation++;
    }

}
