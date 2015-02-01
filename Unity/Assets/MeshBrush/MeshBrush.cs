using UnityEngine;
using System.Collections;

public class MeshBrush : MonoBehaviour
{

	//Actual editor script is inside the Assets/Editor folder...
	//Here I just define some public variables that I need for the inspector gui.

    public GameObject[] setOfMeshesToPaint = new GameObject[1]; //This is the array of meshes to paint.

    [HideInInspector]
    public KeyCode paint = KeyCode.P;
    [HideInInspector]
    public KeyCode increaseRadius = KeyCode.O;
    [HideInInspector]
    public KeyCode decreaseRadius = KeyCode.I;

	[HideInInspector]public float hRadius = 0.3f; //The radius of the helper handle. 

	[HideInInspector]public Color hColor = Color.white; //Sets the helper handle color.

	[HideInInspector]public int meshCount = 1; //Number of meshes to paint.
    [HideInInspector]public float delay = 0.25f; //Delay between paint strokes if you hold down your paint button.
	[HideInInspector]public float meshOffset = 0.0f; //A float variable for the vertical offset of the mesh we are going to paint. You probably won't ever need this if you place the pivot of your meshes nicely, but you never know.

    [HideInInspector]public float inset = 60f; //Percentage of scattering.
	[HideInInspector]public bool autoStatic = true;
	[HideInInspector]public bool uniformScale = true;
	[HideInInspector]public bool constUniformScale = true;
	[HideInInspector]public bool rWithinRange = false; //Within range toggle bool.

    [HideInInspector]public bool b_customKeys = false; //Boolean for the customize keyboard shortcuts foldout.
	[HideInInspector]public bool b_rScale = true; //Boolean value for the randomize foldout menu in the inspector.
	[HideInInspector]public bool b_cScale = true; //Boolean for the 'Apply additive scale' foldout menu.
    [HideInInspector]public bool b_opt = true; //Boolean for the 'Optimization' foldout menu.

	[HideInInspector]public float rScaleW = 0.0f; //Random and constant scale multipliers.
	[HideInInspector]public float rScaleH = 0.0f;
	[HideInInspector]public float rScale = 0.0f;
	[HideInInspector]public Vector2 rUniformRange = Vector2.zero; //Variables for our customized random ranges.....
	[HideInInspector]public Vector4 rNonUniformRange = Vector4.zero;

	[HideInInspector]public float cScale = 0.0f; //Float variable for the uniform additive scale.
	[HideInInspector]public Vector3 cScaleXYZ = Vector3.zero; //Vector3 variable for the non-uniform additive scale.

	[HideInInspector]public float rRot = 0.0f; //Random rotation float value.

    

	public void ResetRandomizers()
	{
		rScale = 0;
		rScaleW = 0;
		rScaleH = 0;
		rRot = 0;
		rUniformRange = Vector2.zero;
		rNonUniformRange = Vector4.zero;
	}
}

/*
 * 
 * Raphael Beck, 2014
 * 
 */