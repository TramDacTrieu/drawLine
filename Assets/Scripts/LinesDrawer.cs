using UnityEngine;

public class LinesDrawer : MonoBehaviour {

    public GameObject linePrefab;
    public LayerMask cantDrawOverLayer;

    [Space(30f)]
    public Gradient lineColor;
    public float linePointsMinDistance;
    public float lineWidth;

    Line currentLine;

    Camera cam;
    Vector2 startDrawingPos;
    LayerMask cast;

    int cantDrawOverLayerIndex;

    void Start() {
        cam = Camera.main;

        cast = LayerMask.GetMask("CantDrawOver");
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
    }

    void Update() {
        if (Input.GetMouseButtonDown(0))
            BeginDraw();

        if (currentLine != null)
            Draw();

        if (Input.GetMouseButtonUp(0))
            EndDraw();
    }

    void BeginDraw() {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        startDrawingPos = mousePosition;
        currentLine = Instantiate(linePrefab, mousePosition, Quaternion.identity).GetComponent<Line>();

        //Set line properties
        currentLine.UsePhysics(false);
        currentLine.SetLineColor(lineColor);
        currentLine.SetPointsMinDistance(linePointsMinDistance);
        currentLine.SetLineWidth(lineWidth);
    }

    void Draw() {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cast);
        mousePosition -= startDrawingPos;

        if (hit)
            EndDraw();
        else
            currentLine.AddPoint(mousePosition);
    }

    void EndDraw() {
        if (currentLine != null) {
            if (currentLine.pointsCount < 2) {
                Destroy(currentLine.gameObject);
            } else {
                currentLine.gameObject.layer = cantDrawOverLayerIndex;
                currentLine.UsePhysics(true);
                currentLine = null;
            }
        }
    }
}
