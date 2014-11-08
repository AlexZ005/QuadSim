using System;
using UnityEngine;

public interface IQuadController {
	quadPhysics quadcopter { get; }
    Vector4 getInputs( Vector3 x, Vector3 xdot, Vector3 theta, Vector3 thetadot );
	void reset();
}
