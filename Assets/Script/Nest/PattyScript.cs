using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PattyScript : MonoBehaviour {

    public enum Side
    {
        None = 0, 
        Top = 1, 
        Bottom = 2,
    }

    public Collider _panCol = null;

    float _tpoSide = 0f;
    float _bottomSide = 0f;

    //float _stayTime = 0f;
    //bool _enter = false;
    Side _panSide = Side.None;


    private void OnCollisionEnter(Collision collision)
    {
        if( collision.collider == _panCol)
        {
            if (Vector3.Angle(transform.up, Vector3.up) <= 90)
                _panSide = Side.Top;
            else
                _panSide = Side.Bottom;

            Debug.Log(_panSide + "   " +  gameObject.name + "  " + collision.gameObject.name);
        }
            
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == _panCol)
        {
            _panSide = Side.None;
         
        }
            
    }

    private void Update()
    {
        
    }
}
