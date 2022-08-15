using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Square class! In charge of individual square changes and sprite management!
public class Square : MonoBehaviour
{
    public bool alive = false;
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

    float flipCd = 0.2f;
    float lastTime = -1f;
    private void OnMouseOver() {
        if (lastTime == -1f || Time.time - lastTime >= flipCd) {
            if (Input.GetMouseButton(0)) {
                changeState();
                parent.buffer[location.x][location.y] = alive;
                lastTime = Time.time;
            }
            if (Input.GetMouseButton(1)) {
                print(location);
                print('\t' + (parent.getAliveNeighbors(location.x, location.y).ToString()));
                lastTime = Time.time;
            }
        }
    }
}
