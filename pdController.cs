using UnityEngine;
using System.Collections;

public class pdController : MonoBehaviour, IQuadController {
	
	public quadPhysics quadcopter { get; private set; }
	
    // Use this for initialization
    private void Start ()
	{
		quadcopter = (quadPhysics)GetComponent (typeof(quadPhysics));
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
		
		return new Vector4 (
			thrust / 4 - (2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l),
			thrust / 4 + error [2] * Iz / (4 * quadcopter.b) - (error [1] * Iy) / (2 * quadcopter.k * quadcopter.l),
			thrust / 4 - (-2 * quadcopter.b * error [0] * Ix + error [2] * Iz * quadcopter.k * quadcopter.l) / (4 * quadcopter.b * quadcopter.k * quadcopter.l),
			thrust / 4 + error [2] * Iz / (4 * quadcopter.b) + (error [1] * Iy) / (2 * quadcopter.k * quadcopter.l));
	}
}
