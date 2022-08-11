using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Parallax : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 previousCamPosition; //take change in position to calculate which way to move

    [SerializeField] private bool infiniteScrollX;
    [SerializeField] private bool infiniteScrollY;
    [SerializeField] private Vector2 scrollSpeed;

    private Vector2 textureSize;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        previousCamPosition = camTransform.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        //get the scale of texture
        textureSize.x = texture.width * transform.localScale.x / sprite.pixelsPerUnit;
        textureSize.y = texture.height * transform.localScale.y / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = camTransform.position - previousCamPosition;
        //shifting the position. directly shifts it = teleportation based movement
        //moves the world by the difference * speed
        transform.position += new Vector3(deltaMovement.x * scrollSpeed.x, deltaMovement.y * scrollSpeed.y, 0f);
        previousCamPosition = camTransform.position;

        //if we are allowed to horizontally scroll AND camera is past texture size
        if (infiniteScrollX && Mathf.Abs(camTransform.position.x - transform.position.x) > textureSize.x)
        {
            //move texture to make it seem like everything stays loaded
            float offsetPositionX = (camTransform.position.x - transform.position.x) % textureSize.x; //% = modulus = remainder
            //move this object's position to the cam transform + the offset
            transform.position = new Vector3(camTransform.position.x + offsetPositionX, transform.position.y, 0f);
        }

        if (infiniteScrollY)
        {
            //move texture to make it seem like everything stays loaded
            float offsetPositionY = (camTransform.position.y - transform.position.y) % textureSize.y; //% = modulus = remainder
            //move this object's position to the cam transform + the offset
            transform.position = new Vector3(camTransform.position.x, camTransform.position.y + offsetPositionY, 0f);
        }
    }
}
