using UnityEngine;
using System.Collections;

public class pdController : MonoBehaviour, IQuadController {
	
	public quadPhysics quadcopter { get; private set; }
	private Backpropagation bp = null;
    // Use this for initialization
    private void Start ()
	{
		quadcopter = (quadPhysics)GetComponent (typeof(quadPhysics));
		bp = GameObject.Find("GM").GetComponent<Backpropagation>();
		reset ();
	}

    // Update is called once per frame
    private void Update ()
	{
		
	}
	
	public void reset ()
	{

	}
	
	public Vector4 getInputs (Vector3 x, Vector3 xdot, Vector3 theta, Vector3 thetadot)
	{
		float dt = Time.deltaTime * 1;
		
		float kd = 1.0f;
		float kp = 1.0f;
		
		// total thrust
		float thrust = quadcopter.m * quadcopter.g / (quadcopter.k * Mathf.Cos (theta[0]) * Mathf.Cos (theta[1]));
		//float thrust = quadcopter.m / (quadcopter.k * Mathf.Cos (theta[0]) * Mathf.Cos (theta[1])); //funny, like in a space
		//Debug.Log("Thrust : " + thrust + " theta[0]: " + theta[0]);
		// compute error
		Vector3 err = kd * thetadot + kp * theta;
		
		var inputs = err2inputs (err, thrust);
		
		if (float.IsNaN (inputs [0]))
			Debug.Break ();
		/*
        inputs[0] = Mathf.Max(inputs[0], 0);
        inputs[1] = Mathf.Max(inputs[1], 0);
        inputs[2] = Mathf.Max(inputs[2], 0);
        inputs[3] = Mathf.Max(inputs[3], 0);
		*/
		return inputs;
	}
	
	private Vector4 err2inputs (Vector3 error, float thrust)
	{
	
		float Ix = quadcopter.i [0, 0],
			Iy = quadcopter.i [1, 1],
			Iz = quadcopter.i [2, 2];
			
			// float[] ans = new float[4]; ;
			// ans = bp.feedForwardContinue(transform.rotation[0],transform.rotation[1],transform.rotation[2],transform.rotation[3]);
			
			// Debug.Log("PID[0]: " + thrust / 4 - (2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l));
			// Debug.Log("BackP[0]: " + ans[0]);
			// return new Vector4 (
			// ans[2]*10000,		//funny if you will place 1000
			// ans[3]*10000,
			// ans[2]*10000,
			// ans[3]*10000);
			//Debug.Log("PID[0]: " + thrust / 4 - (2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l));
			//Debug.Log("BackP[0]: " + ans[0]);
		//Debug.Log(thrust / 4 - (2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l));
		if (quadcopter.quadmode == 1)		//if PID is on, control quad with PID controller
		return new Vector4 (
			thrust / 4 - (2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l),		//funny if you will place 1000
			thrust / 4 + error [2] * Iz / (4 * quadcopter.b) - (error [1] * Iy) / (2 * quadcopter.k * quadcopter.l),
			thrust / 4 - (-2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l),
			thrust / 4 + error [2] * Iz / (4 * quadcopter.b) + (error [1] * Iy) / (2 * quadcopter.k * quadcopter.l));
			else
			return new Vector4();
	}
}