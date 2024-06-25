using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphere_script_orbit : MonoBehaviour
{


    public float degrees_rel_hub;

    public GameObject hub_obj;
    public GameObject other_sphere_obj;

    private Vector3 velocity_vector;
    private float sphere_velocity;
    private float radius_slop;


    public GameObject earthObject;
    private Vector3 toEarthVector;

    private float gravConst;
    private float earthMass;
    private float gravAcc;

    private void Awake()
    {
        radius_slop = .01f;

        gravConst = 6.67384e-11f;
        earthMass = 5.972e+24f;


    }



    // Start is called before the first frame update
    void Start()
    {
        //***** SET UP SPHERE POSITION, ORIENTATION, AND INITIAL VELOCITY VECTOR *****

        //calculate unit vector direction of "wheel spoke" 
        Vector3 vector_from_hub = (Quaternion.AngleAxis(degrees_rel_hub, hub_obj.transform.up) * hub_obj.transform.right).normalized;

        //move sphere to start position
        transform.position = hub_obj.transform.position + (vector_from_hub * hub_obj.GetComponent<hub_script_orbit>().radius_length);

        //make sphere rotation match hub rotation
        transform.rotation = hub_obj.transform.rotation;

        //get specified tangential velocity magnitude from hub object
        sphere_velocity = hub_obj.GetComponent<hub_script_orbit>().velocity_start;

        //calculate initial velocity_vector normalized and multiply by sphere velocity
        velocity_vector = Vector3.Cross(transform.position - hub_obj.transform.position, hub_obj.GetComponent<hub_script_orbit>().axis_vector).normalized * sphere_velocity;

        //************************************************************************************


    }

    private void FixedUpdate()
    {
        //update sphere position based on its current velocity vector
        transform.position = transform.position + (velocity_vector * Time.deltaTime);

        //update sphere rotation to keep things neat
        transform.rotation = Quaternion.LookRotation(hub_obj.transform.position - transform.position, hub_obj.GetComponent<hub_script_orbit>().axis_vector);


        //**************VELOCITY VECTOR UPDATED USING ACCELERATIONS IN SYSTEM********************

        //update sphere velocity vector by applying centripital acceleration*time vector to it
        //---NOTE: we are keeping the radius length constant which produces an assumed constant accelertion acting on the sphere
        velocity_vector = velocity_vector + ((hub_obj.transform.position - transform.position).normalized * Mathf.Pow(velocity_vector.magnitude, 2) / hub_obj.GetComponent<hub_script_orbit>().radius_length * Time.deltaTime);

        //ADDS THE EFFECT OF GRAVITY TO THE SPHERE MOTION
        //get vector to Earth center
        toEarthVector = (earthObject.transform.position - transform.position);

        //calculate gravitational force magnitude
        //---NOTE: the sim assumes length units are in km, so must multiply by length 1000 to get meters so we can have the standard ms^2 units
        gravAcc = (gravConst * earthMass / Mathf.Pow(toEarthVector.magnitude * 1000, 2f));

        //add grav acceleration * time to sphere's velocity vector
        velocity_vector = velocity_vector + toEarthVector.normalized * gravAcc;

        //*********************************************************************
        
    }

    // Update is called once per frame
    void Update()
    {

        //*****************Debug lines used to visual vectors involved

        //Debug.Log("sphere position:"+ transform.position + "other postition:"+other_sphere_obj.transform.position);
        //Debug.Log("velocity vector:" + velocity_vector);
        Debug.DrawLine(transform.position, transform.position + velocity_vector.normalized *200, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + transform.up *200, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 200, Color.red);

    }
}
