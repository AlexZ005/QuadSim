using UnityEngine;
using System.Collections;

public class quadPhysics : MonoBehaviour
{
	private Backpropagation bp = null;
	
	public float LifeValue = 2.0f;
	public float HaveKnife = 1.0f;
	public float HaveGun = 1.0f;
	
	public float BulletAmount = 2.0f;
	// physics coefficients
	public float k;				// thrust coefficient
	public float b;				// drag coefficient
	public float l;				// distance between motor and center
	public float m;				// total mass of quadcopter
	public float g;				// gravity accel. on -z (abs value)
	public float kd;			// global drag coefficient

	// quadcopter parameters
	private Vector3 xdot;		// movement speed
	private Vector3 theta;		// rotations (yaw, pitch, roll)
	private Vector3 thetadot;	// rotation derivatives
	private Vector3 omega;		// angular velocity vector
	private Vector3 omegadot;	// angular velocity derivative
	private Vector4 rpm;		// (squared) blades rotation speeds
	public Matrix4x4 i;			// inertia tensor
	
	public float maxInput;		// maximum input per engine
	
	// will control the engines rotation speeds
	private IQuadController controller;
	private Vector4 lastInput;
	
	// line renderers to visually show thrust values
	private LineRenderer lrEngine1;
	private LineRenderer lrEngine2;
	private LineRenderer lrEngine3;
	private LineRenderer lrEngine4;
	
	// Use this for initialization
	private void Start ()
	{
		bp = GameObject.Find("GM").GetComponent<Backpropagation>();
		controller = (IQuadController)GetComponent (typeof(IQuadController));
		
		lrEngine1 = GameObject.Find ("quadcopter/Plane_002").GetComponent<LineRenderer> ();
		lrEngine2 = GameObject.Find ("quadcopter/Plane").GetComponent<LineRenderer> ();
		lrEngine3 = GameObject.Find ("quadcopter/Plane_001").GetComponent<LineRenderer> ();
		lrEngine4 = GameObject.Find ("quadcopter/Plane_003").GetComponent<LineRenderer> ();
		
		reset ();
	}
	
	private void reset ()
	{
		k = 0.01f;
		b = 1e-7f;
		l = 15f;
		m = 0.5f;
		g = 0f;
		kd = 0.25f;
		
		i = createMatrix3 (0.005f, 0, 0,
			0, 0.005f, 0,
			0, 0, 0.01f);
		
		maxInput = 10000;
		
		omega = Vector3.zero;
		omegadot = Vector3.zero;
		
		theta = Random.insideUnitSphere;
		thetadot = Random.insideUnitSphere;
		
		transform.position = Vector3.zero;
		xdot = Vector3.zero;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (Input.GetKeyDown ("space"))
		Destroy(gameObject);


		float dt = Time.deltaTime * 1;

		Vector4 inputs = controller.getInputs (transform.position, xdot, theta, thetadot);
		lastInput = inputs;

		omega = thetadot2omega (thetadot, theta);
		omegadot = angular_acceleration (inputs);
		Vector3 acc = acceleration (inputs, omega);

		omega += omegadot * dt;
		thetadot = omega2thetadot (omega, theta);
		theta += thetadot * dt;
		xdot += acc * dt;
		transform.position += xdot * dt;
		transform.rotation = Quaternion.Euler (theta * 360 / 6.28f);

		transform.Rotate (new Vector3 (-90, 0, 0));

		// visual thrust indicators update
		float scale = 0.01f;
		lrEngine1.SetPosition (1, new Vector3 (0, 0, inputs [0] * scale));
		lrEngine2.SetPosition (1, new Vector3 (0, 0, inputs [1] * scale));
		lrEngine3.SetPosition (1, new Vector3 (0, 0, inputs [2] * scale));
		lrEngine4.SetPosition (1, new Vector3 (0, 0, inputs [3] * scale));
		

		if (Input.GetKeyDown (KeyCode.X)) {
			transform.Rotate (new Vector3 (25, 0, 0));
				}

		string cmd = bp.action (new float[4]{LifeValue,HaveKnife,HaveGun,BulletAmount});
		switch(cmd){
		case "A":
			transform.Rotate (new Vector3 (25, 0, 0));
			Debug.Log("a");
			break;
		case "B":
			transform.Rotate (new Vector3 (25, 0, 0));
			Debug.Log("b");
			break;
		case "C":
			transform.Rotate (new Vector3 (25, 0, 0));
			Debug.Log("c");
			break;
		case "D":
			transform.Rotate (new Vector3 (25, 0, 0));
			Debug.Log("d");
			break;	
		}


		if (Input.GetKeyDown (KeyCode.R)) {
			this.reset ();
						controller.reset ();
				}
	}
	
	
	private void OnGUI ()
	{
	GUI.color = Color.red;
		if (GUI.Button (new Rect (500, 10, 100, 20), "Restart")) {
			this.reset ();
			controller.reset ();
		}
		
		
		GUI.Box(new Rect(0,0,Screen.width,Screen.height),"This is a title");
		
		GUI.color = Color.white;
	
		GUI.Box(new Rect(5, 20, 200, 250), "Parameters");
	
		GUI.Label (new Rect (10, 50, Screen.width, Screen.height),
			"\nx:        " + transform.position.ToString () +
			"\nxdot:     " + xdot.ToString () +
			"\n\nomega:    " + omega.ToString () +
			"\nomegadot: " + omegadot.ToString () + 
			"\n\ntheta:    " + theta.ToString () +
			"\nthetadot: " + thetadot.ToString () + 
			"\n\nrotation: " + transform.rotation.ToString () +
			"\n\ninputs:\n   " + lastInput.ToString ());
	}
	
	private Vector3 angular_acceleration (Vector4 inputs)
	{
		Vector3 tau = torques (inputs);
		return Matrix4x4.Inverse (i) * (tau - Vector3.Cross (omega, i * omega));
	}

	private Vector3 torques (Vector4 inputs)
	{
		return new Vector3 (
            l * k * (inputs [0] - inputs [2]),
			b * (inputs [0] - inputs [1] + inputs [2] - inputs [3]),
            l * k * (inputs [1] - inputs [3]));
	}

	private Vector3 thrust (Vector4 inputs)
	{
		return new Vector3 (0, k * (inputs [0] + inputs [1] + inputs [2] + inputs [3]), 0);
	}

	private Vector3 acceleration (Vector4 inputs, Vector3 omega)
	{
		Vector3 gravity = new Vector3 (0, -g, 0);
		Vector3 rot = transform.rotation * Vector3.forward * 6.28f / 360;
		
		Vector3 t = rotation (theta) * thrust (inputs);
		Vector3 fd = xdot * -kd;

		return gravity + 1 / m * t + fd;
	}

	private Matrix4x4 rotation (Vector3 angles)
	{
		return transform.localToWorldMatrix;
		// ?
		
		float psi = angles [0],
			theta = angles [1],
			phi = angles [2];
		
		var m = new Matrix4x4 ();
		m.SetRow (0, new Vector4 (
			Mathf.Cos (phi) * Mathf.Cos (theta),
			Mathf.Cos (theta) * Mathf.Sin (phi),
			-Mathf.Sin (theta), 1));
		
		m.SetRow (1, new Vector4 (
			Mathf.Cos (phi) * Mathf.Sin (theta) * Mathf.Sin (psi) - Mathf.Cos (psi) * Mathf.Sin (phi),
        	Mathf.Cos (phi) * Mathf.Cos (psi) + Mathf.Sin (phi) * Mathf.Sin (theta) * Mathf.Sin (psi),
        	Mathf.Cos (theta) * Mathf.Sin (psi), 1));
		
		m.SetRow (2, new Vector4 (
			Mathf.Sin (phi) * Mathf.Sin (psi) + Mathf.Cos (phi) * Mathf.Cos (psi) * Mathf.Sin (theta),
        	Mathf.Cos (psi) * Mathf.Sin (phi) * Mathf.Sin (theta) - Mathf.Cos (phi) * Mathf.Sin (psi),
        	Mathf.Cos (theta) * Mathf.Cos (psi), 1));
		
		m.SetRow (3, new Vector4 (1, 1, 1, 1));
		return m;
	}
	
	private static Vector3 thetadot2omega (Vector3 thetadot, Vector3 angles)
	{
		float phi = angles [0];
		float theta = angles [1];

		Matrix4x4 w = createMatrix3 (
			1, 0, -Mathf.Sin (theta),
			0, -Mathf.Sin (phi), Mathf.Cos (theta) * Mathf.Cos (phi),
			0, Mathf.Cos (phi), Mathf.Cos (theta) * Mathf.Sin (phi));

		return w * thetadot;
	}

	private static Vector3 omega2thetadot (Vector3 om, Vector3 angles)
	{
		float phi = angles [0];
		float theta = angles [1];

		Matrix4x4 w = createMatrix3 (
			1, 0, -Mathf.Sin (theta),
			0, -Mathf.Sin (phi), Mathf.Cos (theta) * Mathf.Cos (phi),
			0, Mathf.Cos (phi), Mathf.Cos (theta) * Mathf.Sin (phi));
		
		return Matrix4x4.Inverse (w) * om;
	}
	
	private static Matrix4x4 createMatrix3 (float a11, float a12, float a13,
		float a21, float a22, float a23,
		float a31, float a32, float a33)
	{
		Matrix4x4 m = new Matrix4x4 ();
		
//		m.SetRow (0, new Vector4 (a11, a12, a13, 0));
//		m.SetRow (1, new Vector4 (a21, a22, a23, 0));
//		m.SetRow (2, new Vector4 (a31, a32, a33, 0));
//		m.SetRow (3, new Vector4 (0, 0, 0, 1));
		
		return m;
	}
}
