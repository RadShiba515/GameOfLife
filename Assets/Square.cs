using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Square class! In charge of individual square changes and sprite management!
public class Square : MonoBehaviour
{
    public Vector2Int location;
    SpriteRenderer sr;
    SquareArray parent;

    public Square() {
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
        if(parent.grid[location.x][location.y] && sr.sprite != parent.liveSprite) {
            sr.sprite = parent.liveSprite;
        }
        if(!parent.grid[location.x][location.y] && sr.sprite != parent.deadSprite) {
            sr.sprite = parent.deadSprite;
        }
    }

    float flipCd = 0.2f;
    float lastTime = -1f;
    private void OnMouseOver() {
        if (lastTime == -1f || Time.time - lastTime >= flipCd) {
            if (Input.GetMouseButton(0)) {
                parent.grid[location.x][location.y] = !parent.grid[location.x][location.y];
                parent.buffer[location.x][location.y] = !parent.buffer[location.x][location.y];
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
