using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControll : MonoBehaviour
{
    private float angle;

    private Vector2 MousePos;
    private Vector2 target;
    private Vector2 AtkPos;

    Transform obj1;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.position;
        obj1 = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveAtk();
    }
    
    void MoveAtk()
    {
        if (Input.GetMouseButton(0))
        {
            MousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x,
                    Input.mousePosition.y));
            AtkPos = MousePos - new Vector2(obj1.position.x, obj1.position.y);
            angle = Mathf.Atan2(AtkPos.y, AtkPos.x) * Mathf.Rad2Deg;

            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

}
