using UnityEngine;
using System.Collections;

public class NPCSpawn : MonoBehaviour {

    public GameObject NPC;

    public int Number;

    public WayPoint StartWaypoint;

    public float Radius = 2;

    public LayerMask terrainLayer;

    public IEnumerator Spawn()
    {
        for (int i = 0; i < Number; i++)
        {
            Vector2 randomVector = Random.insideUnitCircle;
            Vector3 pos = new Vector3(transform.position.x + (randomVector * 2).x, transform.position.y, transform.position.z + (randomVector * 2).y);
            GameObject o = (GameObject)GameObject.Instantiate(NPC, pos, transform.rotation);
            PutToGround(o.transform);
            if (StartWaypoint != null)
            {
                o.SendMessage("SetStartWaypoint", StartWaypoint);
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1f)); 
        }
    }

    void PutToGround(Transform t)
    {
        RaycastHit hit;
        if (Physics.Raycast(t.position, Vector3.down, out hit, 9999, terrainLayer.value))
        {
            t.position = hit.point;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
	 
}
