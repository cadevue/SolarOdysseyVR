using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

enum Planets
{
    Sun,
    Mercury,
    Venus,
    Earth,
    Mars,
}

public class FollowSpline : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] float speed = 0.1f;
    [SerializeField] GameObject spaceship;

    private float progressRatio;
    private float progress;
    private float totalLength;

    private void Start()
    {
        //PlanetTour();
    }

    private void Update()
    {   
    }

    private Spline GetSolarSystemSpline(string planet)
    {
        int planetIndex = (int)System.Enum.Parse(typeof(Planets), planet, true);

        return splineContainer[planetIndex*2];
    }

    private bool SpaceshipDirection(string currentPlanet, string targetPlanet)
    {
        return true;
    }

    public void PlanetTour()
    {
        SplinePath spaceshipPath = CreateSpaceshipPath("sun", "sun", true);
        StartCoroutine(FollowCoroutine(spaceshipPath));
    }
        

    IEnumerator FollowCoroutine(SplinePath path)
    {
        while (true)
        {
            progressRatio = 0f;

            while (progressRatio <= 1f)
            {
                var pos = path.EvaluatePosition(progressRatio);
                var direction = path.EvaluateTangent(progressRatio);

                spaceship.transform.position = pos;
                spaceship.transform.LookAt(pos + direction);

                progressRatio += speed * Time.deltaTime * 0.001f;

                yield return null;
            }

            break;
        }
    }

    private Spline ReturnToMainPath(string planet)
    {
        BezierKnot nearestOrbitKnot = GetNearestKnot(planet);

        Vector3 localShipPos = splineContainer.transform.InverseTransformPoint(spaceship.transform.position);

        Spline returnSpline = new Spline();
        returnSpline.Add(new BezierKnot((float3)localShipPos));
        returnSpline.Add(nearestOrbitKnot);

        return returnSpline;
    }

    private BezierKnot GetNearestKnot(string planet)
    {
        Spline firstSpline = GetSolarSystemSpline(planet);
        float minDistance = float.MaxValue;

        BezierKnot[] knotList = firstSpline.ToArray();
        BezierKnot nearestKnot = knotList[0];

        Vector3 localShipPos = splineContainer.transform.InverseTransformPoint(spaceship.transform.position);

        foreach (BezierKnot knot in knotList)
        {
            Vector3 knotDistance = localShipPos - (Vector3)knot.Position;

            if (minDistance > knotDistance.magnitude)
            {
                minDistance = knotDistance.magnitude;
                nearestKnot = knot; 
            }
        }

        return nearestKnot;
    }

    private int GetNearestOrbitKnotIndex(string planet)
    {
        Spline planetPathOrbit = GetSolarSystemSpline(planet);
        BezierKnot nearestOrbitKnot = GetNearestKnot(planet);
        int nearestOrbitKnotIndex = planetPathOrbit.IndexOf(nearestOrbitKnot);

        return nearestOrbitKnotIndex;
    }

    private List<SplineSlice<Spline>> PassingPlanet(string currentPlanet, string targetPlanet, bool isGoingInnerSolarSystem = true)
    {
        List<SplineSlice<Spline>> pathSlices = new List<SplineSlice<Spline>>();

        var localToWorldMatrix = splineContainer.transform.localToWorldMatrix;

        int currentPlanetIndex = (int)System.Enum.Parse(typeof(Planets), currentPlanet, true);
        int targetPlanetIndex = (int)System.Enum.Parse(typeof(Planets), targetPlanet, true);

        if (isGoingInnerSolarSystem)
        {
            for (int i = currentPlanetIndex * 2 - 1; i > targetPlanetIndex * 2; i--)
            {
                if (i % 2 == 0)
                {
                    var splinePath = splineContainer.Splines[i];
                    pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(0, 5), localToWorldMatrix));
                }
                else
                {
                    var splinePath = splineContainer.Splines[i];
                    pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(0, 4), localToWorldMatrix));
                }
            }
        }
        else
        {
            for (int i = currentPlanetIndex * 2 + 1; i < targetPlanetIndex * 2; i++)
            {
                if (i % 2 == 0)
                {
                    var splinePath = splineContainer.Splines[i];
                    pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(4, 5), localToWorldMatrix));
                }
                else
                {
                    var splinePath = splineContainer.Splines[i];
                    pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(3, -4), localToWorldMatrix));
                }
            }
        }

        return pathSlices;
    }

    private List<SplineSlice<Spline>> FullSpaceshipTour()
    {
        List<SplineSlice<Spline>> pathSlices = new List<SplineSlice<Spline>>();

        var localToWorldMatrix = splineContainer.transform.localToWorldMatrix;

        int startIndex = splineContainer.Splines.Count;

        for (int i = startIndex - 1; i == 0; i--)
        {
            if (i % 2 == 0)
            {
                var splinePath = splineContainer.Splines[i];
                pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(0, 13), localToWorldMatrix));
            }
            else
            {
                var splinePath = splineContainer.Splines[i];
                pathSlices.Add(new SplineSlice<Spline>(splinePath, new SplineRange(0, 4), localToWorldMatrix));
            }
        }

        return pathSlices;
    }


    private SplinePath CreateSpaceshipPath(string currentOrbit, string targetOrbit, bool isGoingInnerSolarSystem = true)
    {
        var localToWorldMatrix = splineContainer.transform.localToWorldMatrix;

        Spline backToOrbitPath = ReturnToMainPath(currentOrbit);
        Spline currentPlanetSplines = GetSolarSystemSpline(currentOrbit);

        int nearestKnotIndex = GetNearestOrbitKnotIndex(currentOrbit);

        if (currentOrbit.Equals(targetOrbit))
        {
            SplinePath spaceshipPath = new SplinePath(new[]
            {
                new SplineSlice<Spline>(backToOrbitPath, new SplineRange(0, 2), localToWorldMatrix),
                new SplineSlice<Spline>(currentPlanetSplines, new SplineRange(nearestKnotIndex, currentPlanetSplines.Count + 1), localToWorldMatrix),
            });

            return spaceshipPath;
        }
        else
        {
            int currentOrbitRange = 0;
            if (isGoingInnerSolarSystem)
            {
                currentOrbitRange = nearestKnotIndex > 4 ? 8 - nearestKnotIndex + 5 : 5 - nearestKnotIndex;
            }
            else
            {
                currentOrbitRange = nearestKnotIndex > 4 ? 9 - nearestKnotIndex : 4 - nearestKnotIndex + 5;
            }

            SplineSlice<Spline> currentOrbitSlice = new SplineSlice<Spline>(currentPlanetSplines, new SplineRange(nearestKnotIndex, currentOrbitRange), localToWorldMatrix);
            List<SplineSlice<Spline>> passingPlanetPaths = PassingPlanet(currentOrbit, targetOrbit, isGoingInnerSolarSystem);


            List<SplineSlice<Spline>> pathSlices = new List<SplineSlice<Spline>>();

            pathSlices.Add(new SplineSlice<Spline>(backToOrbitPath, new SplineRange(0, 2), localToWorldMatrix));
            pathSlices.Add(currentOrbitSlice);
            pathSlices.AddRange(passingPlanetPaths);

            SplinePath spaceshipPath = new SplinePath(pathSlices.ToArray());

            return spaceshipPath;
        }
    }
}
