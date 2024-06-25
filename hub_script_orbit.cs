using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hub_script_orbit : MonoBehaviour
{
    public float longitude;
    public float latitude;

    public float altitude;

    public float radius_length;

    private Vector3 gravity_up_vector;

  
    public float velocity_start;

    public float vert_axis_start_angle;
 
    public Vector3 axis_vector;


    public GameObject right_sphere_obj;
    public GameObject left_sphere_obj;

    private Vector3 ideal_position_vector;

    private Vector3 temp_vector;



    private void Awake()
    {
        //set hub position in 3D space, assume Earth is at 0,0,0

        transform.position = Quaternion.Euler(0, longitude, latitude) * (Vector3.right * (6371+altitude));

        //set the up-direction based on gravity direction (radial vector from center)
        gravity_up_vector = (transform.position - Vector3.zero).normalized;
        transform.up = gravity_up_vector;
        
        //set transform.forward direction, which is North
        if (Vector3.Angle(gravity_up_vector, Vector3.up) == 0 || Vector3.Angle(gravity_up_vector, Vector3.up) == 180)
        {
            //if gravity_up is aligned with world up or down, just default to world forward
            //transform.forward = Vector3.forward;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, gravity_up_vector);
        }
        else
        {
            //use cross product of gravity_up_vector and world_up to set transform to globe-east direction
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(gravity_up_vector, Vector3.up), gravity_up_vector);
           
            
        }

        //tilt transform to match desired rotation axis tilt
        transform.rotation *= Quaternion.AngleAxis(vert_axis_start_angle, transform.forward);
       

        //setup rotation axis_vector to match now titled transform.up
        axis_vector = transform.up;

        ideal_position_vector = transform.right;

    }


    // Start is called before the first frame update
    void Start()
    {


    }

    private void FixedUpdate()
    {

        //locate hub equidistant between both spheres
        //but only allow for y movement...
        //...this assumes that the theoretical hub shouldn't drift from its original central position, but it can move up and down
        temp_vector = (right_sphere_obj.transform.position + (left_sphere_obj.transform.position - right_sphere_obj.transform.position) / 2);
        //******************************change this later so that it becomes a projection onto the earth center vector...
        //...this will allow the rotating system to be moved to different longitudes and latitudes
        transform.position = new Vector3( transform.position.x, temp_vector.y, transform.position.z );    


        //update hub orientation to keep things neat
        transform.rotation = Quaternion.LookRotation(right_sphere_obj.transform.position - transform.position, axis_vector);

        //rotate ideal position vector
        //...this is a diagnostic to show where the sphere should be according to a perfect calculation of centripital movement, i.e. dt -> 0
        ideal_position_vector = Quaternion.AngleAxis(-(Mathf.Rad2Deg * (velocity_start / radius_length) * Time.deltaTime), axis_vector) * ideal_position_vector;
    }

    // Update is called once per frame
    void Update()
    {
        //****Various Debugs
        //****DrawLines show visualize vector positions

        //Debug.Log("transform.up =" + transform.up + "axis vector" + axis_vector);
        //Debug.Log("transform.right = " + transform.right + "spoke vector = " + testvect);
        //Debug.Log("Angle diff"+ Vector3.Angle(transform.right, right_sphere_obj.transform.position - transform.position));
        //Debug.Log("gravity up vector" + gravity_up_vector);
        //Debug.Log("current radius distance:" + (right_sphere_obj.transform.position - transform.position).magnitude );

        Debug.DrawLine(transform.position, transform.position + transform.up * 200, Color.magenta);
        Debug.DrawLine(transform.position, transform.position + transform.forward *200, Color.red);
        Debug.DrawLine(transform.position, transform.position + axis_vector *200, Color.green);

        //Debug.Log("ideal position vector" + ideal_position_vector);
        Debug.DrawLine(transform.position, transform.position + ideal_position_vector * radius_length, Color.cyan);

    }
}
