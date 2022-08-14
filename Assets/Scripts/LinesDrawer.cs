using UnityEngine;

public class LinesDrawer : MonoBehaviour {

	public GameObject linePrefab;
	public LayerMask cantDrawOverLayer;
	int cantDrawOverLayerIndex;

	[Space ( 30f )]
	public Gradient lineColor;
	public float linePointsMinDistance;
	public float lineWidth;
	public GameObject test;

	Line currentLine;

	Camera cam;
	Vector2 startDrawingPos;
	LayerMask cast;
 
	void Start ( ) {
		cam = Camera.main;
		//cantDrawOverLayerIndex = cantDrawOverLayer.value;

		cast = LayerMask.GetMask("CantDrawOver");
        cantDrawOverLayerIndex = LayerMask.NameToLayer("CantDrawOver");
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
			BeginDraw ();

		if ( currentLine != null )
            Draw();

        if (Input.GetMouseButtonUp(0))
			EndDraw ();
	}

	// Begin Draw ----------------------------------------------
	void BeginDraw () {

		Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
		//currentLine = Instantiate(linePrefab, this.transform).GetComponent<Line>();
		startDrawingPos = mousePosition;

		currentLine = Instantiate(linePrefab, mousePosition, Quaternion.identity).GetComponent<Line>();

		//Set line properties
		currentLine.UsePhysics(false);
        currentLine.SetLineColor(lineColor);
        currentLine.SetPointsMinDistance(linePointsMinDistance);
		currentLine.SetLineWidth (lineWidth);

		//currentLine.gameObject.layer = cantDrawOverLayerIndex;

				
	}
	// Draw ----------------------------------------------------
	void Draw ( ) {
		Vector2 mousePosition = cam.ScreenToWorldPoint ( Input.mousePosition );
		//Debug.Log(mousePosition);
		test.transform.position = mousePosition;

		

		//Check if mousePos hits any collider with layer "CantDrawOver", if true cut the line by calling EndDraw( )
		RaycastHit2D hit = Physics2D.CircleCast(mousePosition, lineWidth / 3f, Vector2.zero, 1f, cast);

		mousePosition -= startDrawingPos;

		if (hit)
			EndDraw ();
		else
			currentLine.AddPoint(mousePosition);
	}
	// End Draw ------------------------------------------------
	void EndDraw ( ) {
		if ( currentLine != null ) {
			if ( currentLine.pointsCount < 2 ) {
				//If line has one point
				Destroy ( currentLine.gameObject );
			} else {
				//Add the line to "CantDrawOver" layer
				currentLine.gameObject.layer = cantDrawOverLayerIndex;

				//Activate Physics on the line
				currentLine.UsePhysics (true);

				currentLine = null;
			}
		}
	}
}
