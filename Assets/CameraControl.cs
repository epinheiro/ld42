using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    /* Got the base script from \/ /*
    /*    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.*/
    


    float mainSpeed = 20.0f; //regular speed
    private float totalRun = 1.0f;
    private float alreadyScrolled = 0f;
    private bool rotated = false;
    public float borderSize = 30;

    void Update()
    {
        Vector3 p = GetBaseInput();
        p = p * mainSpeed;
        p = p * Time.deltaTime;
        if (rotated) p = -p;
        Vector3 newPosition = transform.position + p;
        transform.position = newPosition;
        

        float cameraDistance = Input.GetAxis("Mouse ScrollWheel") * mainSpeed / 4;        
        if ((alreadyScrolled > 4f && cameraDistance > 0) || (alreadyScrolled < -4f && cameraDistance < 0))
        {
            return;
        }
        alreadyScrolled += cameraDistance;
        
        transform.Translate(new Vector3(0, 0, 1) * cameraDistance);
        Vector3 clampPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (!rotated)
        {
            if (transform.position.z > 8) clampPos.z = 8;
            if (transform.position.z < -12) clampPos.z = -12;
            if (transform.position.x > 25) clampPos.x = 25;
            if (transform.position.x < -13) clampPos.x = -13;
        } else
        {
            if (transform.position.z > 20) clampPos.z = 20;
            if (transform.position.z < 3) clampPos.z = 3;
            if (transform.position.x > 25) clampPos.x = 25;
            if (transform.position.x < -13) clampPos.x = -13;
        }
        
        transform.position = clampPos;
    }

    public void changeToCharacterPosition(Vector3 position, bool firstHalf)
    {
        //Some trigonometry to centralize the player
        float distanceToPlayer = Mathf.Sqrt(3) * transform.position.y / 3;
        transform.position = new Vector3(position.x, transform.position.y, position.z - (firstHalf ? distanceToPlayer : -distanceToPlayer));
        Debug.Log("position " + transform.position + " " + distanceToPlayer + " " + firstHalf);
        if (!firstHalf)
        {
            transform.localRotation = Quaternion.Euler(60, 180, 0);
            rotated = true;
        } else
        {
            transform.localRotation = Quaternion.Euler(60, 0, 0);
            rotated = false;
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        
        Vector3 p_Velocity = new Vector3();
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition))
        {
            return p_Velocity;
        }
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - borderSize)
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < borderSize)
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < borderSize)
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - borderSize)
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}
