using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
   [Header("Look Sensitivity")] 
   public float sensX;
   public float sensY;

   [Header("Clamping")] 
   public float minY;
   public float maxY;

   [Header("Spectator")] 
   public float spectatorMoveSpeed;

    float rotX;
    float rotY;

   private bool _isSpectator;

   private void Start()
   {
      //Lock The cursor At the Start
      Cursor.lockState = CursorLockMode.Locked;
      
   }

   private void LateUpdate()
   {
      rotX += Input.GetAxis("Mouse X")*sensX;
      rotY += Input.GetAxis("Mouse Y") * sensY;
      //Clamp The Vertical Rotation
      rotY=Mathf.Clamp(rotY, minY, maxY);
      
      //Are We Spectating?
      if (_isSpectator)
      {
       //Rotate Camera On Y
       transform.rotation=Quaternion.Euler(-rotY, rotX, 0);
       
       //Movement 
       float x = Input.GetAxis("Horizontal");
       float z = Input.GetAxis("Vertical");
       float y = 0;

       if (Input.GetKey(KeyCode.E))
          y = 1;
       else if (Input.GetKey(KeyCode.Q))
          y = -1;

       Vector3 dir = transform.right*x+transform.up*y+transform.forward*z;
       transform.position += dir * spectatorMoveSpeed * Time.deltaTime;
       



      }
      else
      {
         //Only Rotate On Y
         transform.localRotation=Quaternion.Euler(-rotY, 0, 0);
         transform.parent.rotation=Quaternion.Euler(0, rotX, 0);
         
      }
      
   }


   public void SetAsSpectator()
   {
      _isSpectator = true;
      transform.parent = null;
      
   }
}
