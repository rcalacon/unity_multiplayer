using System;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Velocity;
    public List<Transform> Neighbors;

    public Boid(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(velocity));
    }

    public void ApplyFlockUpdates(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(velocity));
    }

    public void UpdateNeighbors(List<Transform> boids, float distance)
    {
        var neighbors = new List<Transform>();

        for (var neighborIndex = 0; neighborIndex < boids.Count; neighborIndex++)
        {
            if (Position != boids[neighborIndex].position)
            {
                if (Vector3.Distance(boids[neighborIndex].position, Position) < distance)
                {
                    neighbors.Add(boids[neighborIndex]);
                }
            }
        }
        Neighbors = neighbors;
    }

    public Vector3 Cohesion(float cohesionSteps, float weight)
    {
        var perceivedCenter = Vector3.zero;

        if (Neighbors.Count == 0 || cohesionSteps < 1) return perceivedCenter;

        for (var neighborIndex = 0; neighborIndex < Neighbors.Count; neighborIndex++)
        {
            var neighbor = Neighbors[neighborIndex].GetComponent<Boid>();
            if (perceivedCenter == Vector3.zero)
            {
                perceivedCenter = neighbor.Position;
            }
            else
            {
                perceivedCenter = perceivedCenter + neighbor.Position;
            }
        }
        perceivedCenter = perceivedCenter / Neighbors.Count;
        return (perceivedCenter - Position) / cohesionSteps * weight;
    }

    public Vector3 Separation(float weight)
    {
        var c = Vector3.zero;

        for (var i = 0; i < Neighbors.Count; ++i)
        {
            var neighbor = Neighbors[i].GetComponent<Boid>();
            var distance = Vector3.Distance(Position, neighbor.Position);

            c = c + Vector3.Normalize(Position - neighbor.Position) / Mathf.Pow(distance, 2);
        }
        return c * weight;
    }

    public Vector3 Alignment(float weight)
    {
        Vector3 pv = Vector3.zero;

        if (Neighbors.Count == 0) return pv;

        for (var neighborIndex = 0; neighborIndex < Neighbors.Count; neighborIndex++)
        {
            var neighbor = Neighbors[neighborIndex].GetComponent<Boid>();
            pv = pv + neighbor.Velocity;
        }
        if (Neighbors.Count > 1)
        {
            pv = pv / (Neighbors.Count);
        }
        return (pv - Velocity) * weight;
    }

    public Vector3 Seek(Transform target)
    {
        var desiredVelocity = (target.position - Position);
        return desiredVelocity - Velocity;
    }

    public Vector3 Socialize(List<Transform> boids)
    {
         var pc = Vector3.zero; 

        if (Neighbors.Count != 0) return pc;

        for (var i = 0; i < boids.Count; ++i)
        {
            var boid = boids[i].GetComponent<Boid>();
            if (Position != boid.Position)
            {
                if (pc == Vector3.zero)
                {
                    pc = boid.Position;
                }
                else
                {
                    pc = pc + boid.Position;
                }
            }
        }
        if (boids.Count > 1)
        {
            pc = pc / (boids.Count - 1);
        }
        return Vector3.Normalize(pc - Position);
    }

    public Vector3 Arrival(Transform target, float slowingDistance, float maxSpeed)
    {
        var desiredVelocity = Vector3.zero;
        if (slowingDistance < 0.0001f) return desiredVelocity;

        var targetOffset = target.position - Position;
        var distance = Vector3.Distance(target.position, Position);
        var rampedSpeed = maxSpeed * (distance / slowingDistance);
        var clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);
        if (distance > 0)
        {
            desiredVelocity = (clippedSpeed / distance) * targetOffset;
        }
        return desiredVelocity - Velocity;
    }

    public Vector3 LimitVelocity(Vector3 v, float limit)
    {
        if (v.magnitude > limit)
        {
            v = v / v.magnitude * limit;
        }
        return v;
    }

    public Vector3 LimitRotation(Vector3 v, float maxAngle, float maxSpeed)
    {
        return Vector3.RotateTowards(Velocity, v, maxAngle * Mathf.Deg2Rad, maxSpeed);
    }
}
