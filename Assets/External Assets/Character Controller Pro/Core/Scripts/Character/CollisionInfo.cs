using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{


/// <summary>
/// This struct is a character-oriented version of the HitInfo struct (physics queries), mostly used by the CharacterCollisions class. It contains fields such as edge info, 
/// slope angle info, allowed displacement (movement), etc.
/// </summary>
public struct CollisionInfo
{
    public HitInfo hitInfo;

    /// <summary>
    /// Flag that indicates if the collision test was successful or not.
    /// </summary>
    public bool collision;

    /// <summary>
    /// Available displacement obtained as the result of the collision test. By adding this vector to the character position, the result will represent the closest possible position to the hit surface.
    /// </summary>
    public Vector3 displacement;

    /// <summary>
    /// The angle between the contact normal and the character up vector.
    /// </summary>
    public float contactSlopeAngle;

    /// <summary>
    /// Flag that indicates if the character is standing on an edge or not.
    /// </summary>
    public bool isAnEdge;

    /// <summary>
    /// Flag that indicates if the character is standing on an step or not.
    /// </summary>
    public bool isAStep;

    /// <summary>
    /// Normal vector obtained from the edge detector upper ray.
    /// </summary>
	public Vector3 edgeUpperNormal;

    /// <summary>
    /// Normal vector obtained from the edge detector lower ray.
    /// </summary>
	public Vector3 edgeLowerNormal;

    /// <summary>
    /// Angle between the character up vector and the edge detector upper normal.
    /// </summary>
    public float edgeUpperSlopeAngle;

    /// <summary>
    /// Angle between the character up vector and the edge detector lower normal.
    /// </summary>
	public float edgeLowerSlopeAngle;	

    /// <summary>
    /// Angle between the edge detector upper normal and the edge detector lower normal.
    /// </summary>
    public float edgeAngle;
	
}



}