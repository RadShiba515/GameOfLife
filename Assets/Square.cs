using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Square class! In charge of individual square changes and sprite management!
public class Square : MonoBehaviour
{
    public bool alive;
    public Vector2Int location;
    SpriteRenderer sr;
    SquareArray parent;

    public Square() {
    }

    // changeState swaps states with no argument...
    public void changeState() {
        alive = !alive;
    }

    // and sets the specified state if given a bool!
    public void changeState(bool state) {
        alive = state;
    }

    void Start()
    {
        // Default value, change to true for some chaos.
        alive = false;
        // Setting up spriterenderer and parent array object reference.
        sr = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        parent = this.GetComponentInParent<SquareArray>();
    }

    // Update is called once per frame
    void Update()
    {
        // ----------UPDATE SPRITE----------
        if(alive && sr.sprite != parent.liveSprite) {
            sr.sprite = parent.liveSprite;
        }
        if(!alive && sr.sprite != parent.deadSprite) {
            sr.sprite = parent.deadSprite;
        }
    }

    private void OnMouseDown() {
        changeState();
        parent.buffer[location.x][location.y] = alive;
        // parent.changeNeighborStates(location.x, location.y);
    }

    private void OnMouseOver() {

        if(Input.GetMouseButtonDown(1)) {
            print(location);
            print('\t' + (parent.getAliveNeighbors(location.x, location.y).ToString()));
        }
    }

}
