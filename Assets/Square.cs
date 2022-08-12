using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Square : MonoBehaviour
{
    public bool alive;
    public Vector2Int location;
    SpriteRenderer sr;
    SquareArray parent;

    public Square() {
    }

    public void changeState() {
        alive = !alive;
    }

    public void changeState(bool state) {
        alive = state;
    }

    // Start is called before the first frame update
    void Start()
    {
        alive = false;
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
        // print(parent.getAliveNeighbors(location.x, location.y));
        changeState();
        parent.changeNeighborStates(location.x, location.y);
    }
}
