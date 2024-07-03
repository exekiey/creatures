using UnityEngine;

class SplineInterpolator
{

    struct Spline
    {

        public bool isInverted;

        public float a;
        public float b;
        public float c;

        public Vector2 p0;
        public Vector2 p1;

        public Spline(float a, float b, float c, Vector2 p0, Vector2 p1)
        {

            this.a = a;
            this.b = b;
            this.c = c;

            this.p0 = p0;
            this.p1 = p1;

            isInverted = p0.x > p1.x;

        }

        override public string ToString()
        {

            return $"{a} {b} {c}";

        }
        public string ToString2()
        {

            return $"{(int)a} {(int)b} {(int)c}";

        }

        float QuadraticFormula(float x)
        {

            float y = a * x * x + b * x + c;

            return y;


        }

        public Vector2[] GetInterpolation(int precision)
        {


            Vector2[] subpoints = new Vector2[precision];

            if (p0.x > p1.x)
            {

                float step = p0.x - p1.x;
                step = Mathf.Abs(step);

                int counter = 0;

                for (float x = p0.x; x < p1.x; x += step)
                {
                    float y = QuadraticFormula(x);

                    subpoints[counter] = new Vector2(x, y);
                    counter++;

                }

                return subpoints;
            }
            else
            {


                float step = p0.y - p1.y;

                step = Mathf.Abs(step);

                int counter = 0;

                for (float y = p0.y; y < p1.y; y += step)
                {
                    float x = QuadraticFormula(y);

                    subpoints[counter] = new Vector2(x, y);
                    counter++;

                }

                return subpoints;


            }



        }

    }

    Vector2[] subPoints;
    Spline[] splines;
    int precision;
    int numberOfPoints;

    int numberOfSubpoints;

    public int NumberOfSubpoints { get => numberOfSubpoints; }

    public SplineInterpolator(int precision, int numberOfPoints)
    {
        this.precision = precision;
        this.numberOfPoints = numberOfPoints;

        numberOfSubpoints = (numberOfPoints - 1) * precision;

        subPoints = new Vector2[numberOfSubpoints];
        splines = new Spline[numberOfPoints - 1];
    }

    float QuadraticFormula(float x, float a, float b, float c)
    {

        float y = a * x * x + b * x + c;

        return y;


    }

    public Vector2[] Interpolate(Vector2[] points)
    {

        GenerateSplines(points);

        for (int i = 0; i < splines.Length; i++)
        {

            Spline currentSpline = splines[i];

            float startX = currentSpline.p0.x;

            float distance = currentSpline.p1.x - currentSpline.p0.x;

            float step = distance / precision;

            for (int j = 0; j < precision; j++)
            {

                float x = startX + j * step;

                float y = QuadraticFormula(x, currentSpline.a, currentSpline.b, currentSpline.c);

                Vector2 currentSubpoint = new Vector2(x, y);

                subPoints[(i * precision) + j] = currentSubpoint;
            }

        }

        subPoints[subPoints.Length - 1] = splines[splines.Length - 1].p1;



        return subPoints;

    }

    private void GenerateSplines(Vector2[] points)
    {
        /*
        float slope = (points[1].y - points[0].y) / (points[1].x - points[0].x);
        float origin = -slope * points[0].x + points[0].y;

        Spline firstSpline = new Spline(0, slope, origin, points[0], points[1]);*/

        Spline firstSpline = LineSpline(points[0], points[1]);

        splines[0] = firstSpline;

        for (int i = 1; i < splines.Length; i++)
        {

            splines[i] = GetNextSpline(splines[i - 1], points[i], points[i + 1]);
        }

    }


    private Spline LineSpline(Vector2 leftPoint, Vector2 rightPoint)
    {

        float slope = (rightPoint.y - leftPoint.y) / (rightPoint.x - leftPoint.x);
        float origin = -slope * leftPoint.x + leftPoint.y;

        Spline lineSpline = new Spline(0, slope, origin, leftPoint, rightPoint);

        return lineSpline;

    }

    private Spline GetNextSpline(Spline previousSpline, Vector2 leftPoint, Vector2 rightPoint)
    {

        if (leftPoint.x + 0.1f > rightPoint.x && !previousSpline.isInverted)
        {
            return LineSpline(leftPoint, rightPoint);


        }


        Matrix4x4 interpolationMatrix = new Matrix4x4
        (

            new Vector3(2 * leftPoint.x, 1, 0),
            new Vector3(rightPoint.x * rightPoint.x, rightPoint.x, 1),
            new Vector3(leftPoint.x * leftPoint.x, leftPoint.x, 1),
            new Vector4(0, 0, 0, 1)

        );

        interpolationMatrix = interpolationMatrix.transpose;

        Matrix4x4 inverseMatrix = interpolationMatrix.inverse;

        float previousSlope = 2 * previousSpline.a * leftPoint.x + previousSpline.b;

        if (previousSpline.isInverted)
        {

            previousSlope = -previousSlope;

        }

        Vector3 resultVector = new Vector4(previousSlope, rightPoint.y, leftPoint.y);


        Vector3 solution = inverseMatrix.MultiplyVector(resultVector);

        Spline res = new Spline(solution.x, solution.y, solution.z, leftPoint, rightPoint);

        return res;
    }

}

[RequireComponent(typeof(Tentacle))]
[RequireComponent(typeof(CustomLineRenderer))]

public class PointInterpolator : MonoBehaviour
{
    Tentacle tentacle;
    SplineInterpolator pointInterpolator;
    CustomLineRenderer lineRenderer;
    [SerializeField] int precision;

    Vector2[] subpoints;


    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
        lineRenderer = GetComponent<CustomLineRenderer>();



        Debug.Log(tentacle.name);
    }

    private void Start()
    {
        pointInterpolator = new SplineInterpolator(10, tentacle.NumberOfPoints);
        lineRenderer.NumberOfPoints = pointInterpolator.NumberOfSubpoints;
    }


    private void Update()
    {

        Vector2[] points = new Vector2[tentacle.NumberOfPoints];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = tentacle.Points[i].currentPosition;
        }

        subpoints = pointInterpolator.Interpolate(points);

        lineRenderer.SetPoints(subpoints);


    }

}