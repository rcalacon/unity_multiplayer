using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidsController : MonoBehaviour
{
    private List<Transform> _boids = new List<Transform>();
    // Flocking Target. Gets Updated On Editor Changes
    public Transform Prefab;
    public Transform Target;

    // Boids Fields
    private int BoidAmount = 20;
    private float NeighborDistance = 0.8f;
    private float MaxVelocity = 0.02f;
    private float MaxRotationAngle = 7f;
    private Vector3 InitialVelocity;

    // Flocking Behavior
    private float CohesionStep = 100f;
    private float CohesionWeight = 0.05f;
    private float SeparationWeight = 0.01f;
    private float AlignmentWeight = 0.01f;
    private float ArrivalSlowingDistance = 2f;
    private float ArrivalMaxSpeed = 0.2f;

    // Flock Type Specific Fields
    private const int LAZY_FLIGHT_CODE = 0;
    private const int CIRCLE_TREE_CODE = 1;
    private const int FOLLOW_THE_LEADER_CODE = 2;
    private string LazyTargetName = "LazyTarget";
    private string TreeTargetName = "TreeTarget";
    private string LeaderTargetName = "LeaderTarget";
    private Vector3 lazyFollowPosition = new Vector3(0f, 1f, -7f);
    private Vector3 lazyFollowRotation = new Vector3(0f, 0f, 0f);
    private Vector3 treePosition = new Vector3(0f, 5f, -7f);
    private Vector3 treeRotation = new Vector3(40f, 0f, 0f);

    private void Start()
    {
        for (var boidIndex = 0; boidIndex < BoidAmount; boidIndex++)
        {
            var position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0f, 2.0f), Random.Range(-1.0f, 1.0f));
            var transform = Instantiate(Prefab, position, Quaternion.identity);

            transform.GetComponent<Boid>().ApplyFlockUpdates(position, InitialVelocity);
            _boids.Add(transform);
        }


        for (var boidIndex = 0; boidIndex < _boids.Count; boidIndex++)
        {
            var boid = _boids[boidIndex].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
        }
    }

    private void UpdateBoids()
    {
        for (var boidIndex = 0; boidIndex < _boids.Count; boidIndex++)
        {
            var boid = _boids[boidIndex].GetComponent<Boid>();
            // Update its neighbors within a distance
            boid.UpdateNeighbors(_boids, NeighborDistance);
            // Steering Behaviors
            var cohesionVector = boid.Cohesion(CohesionStep, CohesionWeight);
            var separationVector = boid.Separation(SeparationWeight);
            var alignmentVector = boid.Alignment(AlignmentWeight);
            var seekVector = boid.Seek(Target);
            var socializeVector = boid.Socialize(_boids);
            var arrivalVector = boid.Arrival(Target, ArrivalSlowingDistance, ArrivalMaxSpeed);
            // Update Boid's Position and Velocity
            var velocity = boid.Velocity + cohesionVector + separationVector + alignmentVector + seekVector + socializeVector + arrivalVector;
            velocity = boid.LimitVelocity(velocity, MaxVelocity);
            velocity = boid.LimitRotation(velocity, MaxRotationAngle, MaxVelocity);
            var position = boid.Position + velocity;
            boid.ApplyFlockUpdates(position, velocity);
        }
    }

    public void UpdateTarget(int flockCode)
    {
        switch (flockCode)
        {
            case LAZY_FLIGHT_CODE:
                GameObject.Find(LeaderTargetName).GetComponent<Renderer>().enabled = false;
                MaxVelocity = 0.05f;
                MaxRotationAngle = 7f;
                Target = GameObject.Find(LazyTargetName).transform;
                break;
            case CIRCLE_TREE_CODE:
                GameObject.Find(LeaderTargetName).GetComponent<Renderer>().enabled = false;
                MaxVelocity = 0.04f;
                MaxRotationAngle = 2f;
                Target = GameObject.Find(TreeTargetName).transform;
                break;
            case FOLLOW_THE_LEADER_CODE:
                GameObject.Find(LeaderTargetName).GetComponent<Renderer>().enabled = true;
                MaxVelocity = 0.02f;
                MaxRotationAngle = 7f;
                Target = GameObject.Find(LeaderTargetName).transform;
                break;
            default:
                Target = GameObject.Find(LeaderTargetName).transform;
                break;
        }
    }

    private void Update()
    {
        UpdateBoids();
    }

    private IEnumerator UpdateOnFrame()
    {
        while (true)
        {
            UpdateBoids();
            yield return new WaitForSeconds(0.5f);
        }
    }

}
