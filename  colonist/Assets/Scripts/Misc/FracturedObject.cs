using UnityEngine;
using System.Collections;

public class FracturedObject : MonoBehaviour {
	public float PieceMinMass = 1;
	public float PieceMaxMass = 3;
	public float ExplosionMinForce = 50;
	public float ExplosionMaxForce = 200;
	public Transform CreateForceAnchorTransform = null;
	public PhysicMaterial PiecePhysicsMaterial = null;
	public float DestroyedDelay = 3;
	// Use this for initialization
	void Start () {
	    Fracturing();
	}
	
	// Update is called once per frame
	void Update () {
	   
	}
	
	void Fracturing()
	{
		foreach(Transform child in this.transform)
		{
			//only create fracture on visible object
			if(child.GetComponent<Renderer>() != null)
			{
				child.gameObject.AddComponent<BoxCollider>();
				child.gameObject.AddComponent<Rigidbody>();
				child.rigidbody.mass = Random.Range(PieceMinMass,PieceMaxMass);
				child.rigidbody.collider.material = PiecePhysicsMaterial;
				child.gameObject.rigidbody.AddExplosionForce(Random.Range(ExplosionMinForce, ExplosionMaxForce), CreateForceAnchorTransform.position, 50);
			}
		}
		Destroy(this.gameObject, DestroyedDelay);
		
	}
	
	
}
