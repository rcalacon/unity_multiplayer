using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidsController : MonoBehaviour
{
    private List<Transform> _boids = new List<Transform>();
    //Geomtery
    public Transform Prefab;
    public Transform Target;
    //Boids
    private int NumberOfBoids = 20;
    private float NeighborDistance = 0.8f;        // 0.8
    private float MaxVelocity = 0.03f;
    private float MaxRotationAngle = 7f;
    private Vector3 InitialVelocity;
    //Cohesion
    private float CohesionStep = 100f;            // 100
    private float CohesionWeight = 0.05f;          // 0.05
    //Separation
    private float SeparationWeight = 0.01f;        // 0.01`
    //Alignment
    private float AlignmentWeight = 0.01f;         // 0.01
    //Seek
    private float SeekWeight = 0f;              // 0
    //Socialize
    private float SocializeWeight = 0f;         // 0
    //Arrival
    private float ArrivalSlowingDistance = 2f;  // 2
    private float ArrivalMaxSpeed = 0.2f;         // 0.2

    private const int LAZY_FLIGHT_CODE = 0;
    private const int CIRCLE_TREE_CODE = 1;
    private const int FOLLOW_THE_LEADER = 2;
    private string TreeTargetName = "Tree";
    private string LeaderTargetName = "Target";

    // Use this for initialization
    private void Start()
    {
        for (var i = 0; i < NumberOfBoids; ++i)
        {
            var position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0f, 2.0f), Random.Range(-1.0f, 1.0f));
            var transform = Instantiate(Prefab, position, Quaternion.identity);

            transform.GetComponent<Boid>().UpdateBoid(position, InitialVelocity);
            _boids.Add(transform);
        }

        //	    StartCoroutine(UpdateOnFrame());

        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            boid.UpdateNeighbors(_boids, NeighborDistance);
            //	        boid.PrintNeighbors();
        }
    }

    private void UpdateBoids()
    {
        for (var i = 0; i < _boids.Count; ++i)
        {
            var boid = _boids[i].GetComponent<Boid>();
            // Update its neighbors within a distance
            boid.UpdateNeighbors(_boids, NeighborDistance);
            // Steering Behaviors
            var cohesionVector = boid.Cohesion(CohesionStep, CohesionWeight);
            var separationVector = boid.Separation(SeparationWeight);
            var alignmentVector = boid.Alignment(AlignmentWeight);
            var seekVector = boid.Seek(Target, SeekWeight);
            var socializeVector = boid.Socialize(_boids, SocializeWeight);
            var arrivalVector = boid.Arrival(Target, ArrivalSlowingDistance, ArrivalMaxSpeed);
            // Update Boid's Position and Velocity
            var velocity = boid.Velocity + cohesionVector + separationVector + alignmentVector + seekVector + socializeVector + arrivalVector;
            velocity = boid.LimitVelocity(velocity, MaxVelocity);
            velocity = boid.LimitRotation(velocity, MaxRotationAngle, MaxVelocity);
            var position = boid.Position + velocity;
            boid.UpdateBoid(position, velocity);
        }
    }

    public void UpdateTarget(int flockCode)
    {
        switch (flockCode)
        {
            case LAZY_FLIGHT_CODE:
                break;
            case CIRCLE_TREE_CODE:
                Target = GameObject.Find(TreeTargetName).transform;
                break;
            case FOLLOW_THE_LEADER:
                Target = GameObject.Find(LeaderTargetName).transform;
                break;
            default:
                Target = GameObject.Find(LeaderTargetName).transform;
                break;
        }
    }

    // Update is called once per frame
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
