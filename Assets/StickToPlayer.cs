using UnityEngine;

public class StickToPlayer : MonoBehaviour
{
    public Player player;
    public bool keepPhysics;

    void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody != null
            && other.rigidbody.mass < player.maxGrabMassRatio * player.totalMass
            && other.rigidbody.gameObject.GetComponent<StickToPlayer>() == null)
        {
            var otherObject = other.rigidbody.gameObject;

            // var otherStick = otherObject.AddComponent<StickToPlayer>();
            // otherStick.player = player;
            player.totalMass += other.rigidbody.mass;

            var fixedJoint = otherObject.GetComponent<FixedJoint>();
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
            }

            /*foreach (var collider in otherObject.GetComponentsInChildren<Collider>())
            {
                Destroy(collider);
            }*/

            var newFixedJoint = otherObject.AddComponent<FixedJoint>();
            newFixedJoint.connectedBody = player.myRigidbody;
            other.rigidbody.mass *= .1f;

            otherObject.layer = 8;
        }
    }
}