using UnityEngine;
using UnityEditor;
using System.Collections;
[ExecuteInEditMode]
[CustomEditor(typeof(MeshBrush))] //Check out the docs for extending the editor in Unity. You can do lots of awesome stuff with this!
public class MeshBrushEditor : Editor
{
	//Let's declare the variables we are going to need in this script...
	private MeshBrush _mp; //This is the super private MeshPaint instance. It's the original script object we are editing(overriding) with this custom inspector script here.
	bool help = false; //"Help" foldout boolean. I define it in here instead of in MeshPaint.cs because I want it to automatically close whenever the user deselects the GameObject, since it kinda takes away a lot of space in the inspector view.
    bool canPaint = true;
    double Time2Die;
    double _t;

    private KeyCode _paintKey;
    private KeyCode _incRadius;
    private KeyCode _decRadius;

    GUIContent toolTipColor, toolTipRadius, toolTipFreq, toolTipOffset, toolTipInset, toolTipNR, toolTipUniformly, toolTipUniformlyRange, //Tooltips for the various gui elements.
        toolTipWithinRange, toolTipRot, toolTipV4, toolTipReset, toolTipAddScale, toolTipFlagS, toolTipCombine, toolTipDelete; 

	#region OnLoadScript(Awake)
	void Awake(){
        _mp = (MeshBrush)target; //Reference to the script we are overriding.
        
		//The GUI elements of the custom inspector, with their corresponding tooltips:

		toolTipColor = new GUIContent("Color:","Color of the circle brush.");

		toolTipRadius = new GUIContent("Radius [m]:","Radius of the circle brush.");

        toolTipFreq = new GUIContent("Delay [s]:", "If you press and hold down the paint button, this value will define the delay (in seconds) between paint strokes; thus, the higher you set this value, the slower you'll be painting meshes.");

		toolTipOffset = new GUIContent("Offset amount [cm]:","Offsets all the painted meshes along their local Y-axis (away from their underlying surface).\nThis is useful if your meshes are stuck inside your GameObject's geometry, or floating above it.\nGenerally, if you place your pivot points carefully, you won't need this.");

        toolTipInset = new GUIContent("Scattering [%]:","Percentage of how much the meshes are scattered away from the center of the circle brush.\n\n(Default is 60%)");
        
        toolTipNR = new GUIContent("Nr. of meshes:","Number of meshes you are going to paint inside the circle brush area at once.");

		toolTipUniformly = new GUIContent("Scale uniformly","Applies the scale uniformly along all three XYZ axes.");

		toolTipUniformlyRange = new GUIContent("Scale within this random range [Min/Max(XYZ)]:","Randomly scales the painted meshes between these two minimum/maximum scale values.\nX stands for the minimum scale and Y for the maximum scale applied.");

		toolTipWithinRange = new GUIContent("Scale within range","Randomly scales the meshes based on custom defined random range parameters.");

		toolTipRot = new GUIContent("Random Y rotation amount [%]:","Applies a random rotation around the local Y-axis of the painted meshes.");

		toolTipV4 = new GUIContent("[Min/Max Width (X/Y); Min/Max Height (Z/W)]","Randomly scales meshes based on custom defined random ranges.\nThe X and Y values stand for the minimum and maximum width (it picks a random value between them); " +
			"the Z and W values are for the minimum and maximum height.");

		toolTipReset = new GUIContent("Reset all randomizers","Resets all the randomize parameters back to zero.");

		toolTipAddScale = new GUIContent("Apply additive scale","Applies a constant, fixed amount of 'additive' scale after the meshes have been placed.");

		toolTipFlagS = new GUIContent("Flag all painted\nmeshes as static","Flags all the meshes you've painted so far as static in the editor.\nCheck out the Unity documentation about drawcall batching if you don't know what this is good for.");

        toolTipCombine = new GUIContent("Combine painted meshes", "Once you're done painting meshes, you can click here to combine them. This will combine all the meshes you've painted into one single mesh (one per material).\n\nVery useful for performance optimization.\nCannot be undone.");

		toolTipDelete = new GUIContent("Delete all painted meshes","Are you sure? This will delete all the meshes you've painted onto this GameObject's surface so far (except already combined meshes).");
	}
	#endregion

	#region MenuItem function
    [MenuItem("GameObject/Paint meshes on selected GameObject")] //Define a custom menu entry in the Unity toolbar above (this way we don't have to go through the add component menu every time).
    public static void AddMeshPaint() //This function gets called every time we click the above defined menu entry (since it is being defined exactly below the [MenuItem()] statement).
    {
		if (Selection.activeGameObject != null) //Check if there is a GameObject selected.
        {

			if(Selection.activeGameObject.GetComponent<Collider>() != null) //Check if the selected GameObject has a collider on it (without it, where would we paint our meshes on?) :-|
                Selection.activeGameObject.AddComponent<MeshBrush>();
			else {
				if(EditorUtility.DisplayDialog("GameObject has no collider component","The GameObject on which you want to paint meshes doesn't have a collider...\nOn top of what did you expect to paint meshes? :)\n\n" +
					"Do you want me to put a collider on it for you (it'll be a mesh collider)?","Yes please!","No thanks")){
					Selection.activeGameObject.AddComponent<MeshCollider>();
                    Selection.activeGameObject.AddComponent<MeshBrush>();
				}
				else return;
			}
        }
		else EditorUtility.DisplayDialog("No GameObject selected","No GameObject selected man... that's not cool bro D: what did you expect? To paint your meshes onto nothingness? :DDDDD","Uhmmm...");
    }
	#endregion

	#region OnInspectorGUI
    public override void OnInspectorGUI() //Works like OnGUI, except that it updates only the inspector view.
    {
        //Using the keyword target instead of gameObject
		//
        //"target" refers to the current GameObject we are inspecting in the inspector... 
        //...it works just like "gameObject" in MonoBehaviour scripts, it just has a different name since we aren't directly attaching this script here to any object at all. Thus we can access all the public variables of the original script by typing target.<<name_of_variable_here>>.
		//In this case I already created an instance of the script we are editing (MeshPaint) above and assigned it the value "target" for performance reasons 
		//(we just initialize it once in the Awake() function instead of every OnInspectorGUI cycle).

		help = EditorGUILayout.Foldout(help,"Help"); //Foldout menu for the help section, see below for further information
		if(help){
            EditorGUILayout.HelpBox("Paint meshes onto your GameObject's surface.\n_______\n\nKeyBoard shortcuts:\n\nPaint meshes: press or hold " + _mp.paint + "\nIncrease radius: press or hold "+_mp.increaseRadius+"\nDecrease radius: press or hold " + _mp.decreaseRadius +"\n_______\n\n"+
				"Assign one or more prefab objects to the 'Set of meshes to paint' array below and press 'P' while hovering your mouse above your GameObject to start painting meshes." +
			   	"\n\nMake sure that the local Y-axis of each prefab mesh is the one pointing away from the surface on which you are painting (to avoid weird rotation errors).\n\n" +
                "Check the documentation text file that comes with MeshBrush to find out more about the individual parameters (but most of them should be quite self explainatory, or supplied with a tooltip text label after hovering your mouse over them for a couple of seconds).\n_______\n\n"+
			   	"You can press 'Flag/Unflag all painted meshes as static' to mark/unmark as static all the meshes you've painted so far.\nFlagging painted meshes as static will improve performance overhead thanks to Unity's built-in static batching, " +
			   	"as long as the meshes obviously don't move (and as long as they share the same material).\nSo don't flag meshes as static if you have fancy looking animations on your prefab meshes (like, for instance, swaying animations for vegetation or similar properties that make the mesh move, rotate or scale in any way).\n_______\n\n" +
			   	"Once you're done painting, you can also combine your meshes with the 'Combine painted meshes button'. Check out the documentation for further information.\n\n" +
			   	"If you are painting grass or other kinds of small vegetation, I recommend using the '2-Sided Vegetation' shader that comes with the MeshBrush package. It's the " +
			   	"built-in Unity transparent cutout diffuse shader, just without backface culling, so that you get 2-sided materials.\nYou can obviously also use your own custom shaders if you want.\n_______\n\nFeel free to add multiple MeshBrush script instances to one GameObject for multiple mesh painting sets, with defineable parameters for each of them;\n"+
                "MeshBrush will then randomly cycle through all of your MeshBrush instances and paint your meshes within the corresponding circle brush based on the corresponding parameters for that set.", MessageType.None);
		}
		EditorGUILayout.Space();//This leaves a little space between inspector properties... It makes the custom inspector a little lighter and less painful to look at. Believe me: you're gonna need this extra-air to breathe inside the inspector!

        DrawDefaultInspector();
        //EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal(); //Editor version of the GUILayout.BeginHorizontal().
            _mp.autoStatic = EditorGUILayout.Toggle(_mp.autoStatic,GUILayout.Width(15f));
			EditorGUILayout.LabelField("Automatically flag meshes as static",GUILayout.Width(210f),GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal(); 
        EditorGUILayout.Space();

        _mp.b_customKeys = EditorGUILayout.Foldout(_mp.b_customKeys, "Customize Keyboard Shortcuts");
        if (_mp.b_customKeys)
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Paint");
                _mp.paint = (KeyCode)EditorGUILayout.EnumPopup(_mp.paint);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Increase radius");
                _mp.increaseRadius = (KeyCode)EditorGUILayout.EnumPopup(_mp.increaseRadius);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Decrease radius");
                _mp.decreaseRadius = (KeyCode)EditorGUILayout.EnumPopup(_mp.decreaseRadius);
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.Space();
            if (GUILayout.Button("Reset to default keys"))
            {
                _mp.paint = KeyCode.P;
                _mp.increaseRadius = KeyCode.O;
                _mp.decreaseRadius = KeyCode.I;
            }
            EditorGUILayout.Space();
        } EditorGUILayout.Space();

        _mp.paint = (_mp.paint == KeyCode.None) ? KeyCode.P : _mp.paint; //(Avoid having unassigned keys in MeshBrush; reset to default value in case the user tries to set the button to "None")

        _mp.increaseRadius = (_mp.increaseRadius == KeyCode.None) ? KeyCode.O : _mp.increaseRadius;
        _mp.decreaseRadius = (_mp.decreaseRadius == KeyCode.None) ? KeyCode.I : _mp.decreaseRadius;

		_mp.hColor = EditorGUILayout.ColorField(toolTipColor, _mp.hColor); //Color picker for our circle brush.

        EditorGUILayout.BeginHorizontal();
            _mp.hRadius = EditorGUILayout.FloatField(toolTipRadius, _mp.hRadius, GUILayout.Width(175f),GUILayout.ExpandWidth(true));
            _mp.meshCount = EditorGUILayout.IntField(toolTipNR, Mathf.Clamp(_mp.meshCount, 1, 100), GUILayout.Width(175f), GUILayout.ExpandWidth(true)); //Clamp the meshcount so it never goes below 1 or above 100.
        EditorGUILayout.EndHorizontal();

		if(_mp.hRadius < 0.01f) _mp.hRadius = 0.01f; //Avoid negative or null radii.
		EditorGUILayout.Space();

        _mp.delay = EditorGUILayout.Slider(toolTipFreq, _mp.delay, 0.05f, 1.0f); //Slider for the delay between paint strokes.
        EditorGUILayout.Space(); EditorGUILayout.Space();
		
		EditorGUILayout.LabelField(toolTipOffset);
		_mp.meshOffset = EditorGUILayout.Slider(_mp.meshOffset, -50.0f, 50.0f); //Slider for the offset amount.

        if (_mp.meshCount > 1)
        {
            EditorGUILayout.LabelField(toolTipInset);
            _mp.inset = EditorGUILayout.Slider(_mp.inset, 0, 100.0f); //Slider for the scattering.
        }
		EditorGUILayout.Space();
		
		_mp.b_rScale = EditorGUILayout.Foldout(_mp.b_rScale,"Randomize"); //This makes the little awesome arrow for the foldout menu in the inspector view appear...
		EditorGUILayout.Space();

		if(_mp.b_rScale) //...and this below here makes it actually fold stuff in and out (the menu is closed if the arrow points to the right and thus rScale is false).
		{ 
			EditorGUILayout.BeginHorizontal();
			_mp.uniformScale = EditorGUILayout.Toggle("",_mp.uniformScale,GUILayout.Width(15f));
			EditorGUILayout.LabelField(toolTipUniformly,GUILayout.Width(100f));
			_mp.rWithinRange = EditorGUILayout.Toggle("",_mp.rWithinRange,GUILayout.Width(15f));
			EditorGUILayout.LabelField(toolTipWithinRange);
			EditorGUILayout.EndHorizontal();
			
			if(_mp.uniformScale == true){
				if(_mp.rWithinRange == false)
					_mp.rScale = EditorGUILayout.Slider("Random scale:", _mp.rScale, 0, 5f);
				else {
					EditorGUILayout.Space();

					EditorGUILayout.LabelField(toolTipUniformlyRange);
					_mp.rUniformRange = EditorGUILayout.Vector2Field("",_mp.rUniformRange);
				}
			}
			else{
				if(_mp.rWithinRange == false){
					EditorGUILayout.Space();

					_mp.rScaleW = EditorGUILayout.Slider("Random width (X/Z)", _mp.rScaleW, 0, 3f);
					_mp.rScaleH = EditorGUILayout.Slider("Random height (Y)", _mp.rScaleH, 0, 3f);
				}
				else {
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Randomly scale within these ranges:");
					EditorGUILayout.LabelField(toolTipV4);
					_mp.rNonUniformRange = EditorGUILayout.Vector4Field("",_mp.rNonUniformRange);
					EditorGUILayout.Space();
				}
			}
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(toolTipRot);
			_mp.rRot = EditorGUILayout.Slider(_mp.rRot, 0.0f, 100.0f); //Create the slider for the percentage of random rotation around the Y axis applied to our painted meshes.
			EditorGUILayout.Space();
		} 
		
		_mp.b_cScale = EditorGUILayout.Foldout(_mp.b_cScale,toolTipAddScale); //Foldout for the additive scale.
		EditorGUILayout.Space();

		if(_mp.b_cScale)
		{
			_mp.constUniformScale = EditorGUILayout.Toggle(toolTipUniformly,_mp.constUniformScale);
			if(_mp.constUniformScale == true)
				_mp.cScale = EditorGUILayout.FloatField("Add to scale",_mp.cScale);
			else{
				_mp.cScaleXYZ = EditorGUILayout.Vector3Field("Add to scale",_mp.cScaleXYZ);
			}
            if (_mp.cScale < -0.9f) _mp.cScale = -0.9f;
            if (_mp.cScaleXYZ.x < -0.9f) _mp.cScaleXYZ.x = -0.9f;
            if (_mp.cScaleXYZ.y < -0.9f) _mp.cScaleXYZ.y = -0.9f;
            if (_mp.cScaleXYZ.z < -0.9f) _mp.cScaleXYZ.z = -0.9f;
			EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset additive scale", GUILayout.Height(30f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
            {
                _mp.cScale = 0;
                _mp.cScaleXYZ = Vector3.zero;
            }
            if (GUILayout.Button(toolTipReset, GUILayout.Height(30f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                _mp.ResetRandomizers();
            EditorGUILayout.EndHorizontal();
		}

        _mp.b_opt = EditorGUILayout.Foldout(_mp.b_opt, "Optimize");
        if (_mp.b_opt)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(toolTipFlagS, GUILayout.Height(50f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)) && _mp.GetComponentInChildren<MeshBrushParent>() == true) //Create 2 buttons for quickly flagging/unflagging all painted meshes as static...
                _mp.GetComponentInChildren<MeshBrushParent>().FlagMeshesAsStatic();
            if (GUILayout.Button("Unflag all painted\nmeshes as static", GUILayout.Height(50f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)) && _mp.GetComponentInChildren<MeshBrushParent>() == true)
                _mp.GetComponentInChildren<MeshBrushParent>().UnflagMeshesAsStatic();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(toolTipCombine, GUILayout.Height(50f), GUILayout.Width(150f), GUILayout.ExpandWidth(true))) //... one for combining them...
            {
                if (_mp.GetComponentInChildren<MeshBrushParent>() != null)
                {
                    _mp.GetComponentInChildren<MeshBrushParent>().CombinePaintedMeshes();
                    DestroyImmediate(_mp.GetComponentInChildren(typeof(MeshBrushParent)));
                }
            }

            if (GUILayout.Button(toolTipDelete, GUILayout.Height(50f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)) && _mp.GetComponentInChildren<MeshBrushParent>() == true) //...and one to delete all the meshes we painted on this GameObject so far.
                Undo.DestroyObjectImmediate(_mp.GetComponentInChildren(typeof(MeshBrushParent)).gameObject);
            EditorGUILayout.EndHorizontal();
        }
            
		EditorGUILayout.Space();

    }
	#endregion

	#region OnSceneGUI

	void OnSceneGUI() //http://docs.unity3d.com/Documentation/ScriptReference/Editor.OnSceneGUI.html
	{
		Handles.color = _mp.hColor;
        Time2Die = EditorApplication.timeSinceStartup;

        canPaint = (_t > Time2Die) ? false : true;

		if(Selection.gameObjects.Length == 1 && Selection.activeGameObject.gameObject == _mp.gameObject) //Only cast rays if we have our object selected (for the sake of performance).
		{
			Ray scRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition); //Ray through 2D mouse position on scene view.
			RaycastHit[] scHits;
	        scHits = Physics.RaycastAll(scRay);
			int r = 0;
			while(r < scHits.Length) //Check if we are hitting any collider at all...
			{
				if(scHits[r].collider == Selection.activeGameObject.collider) //...and if the collider we hit is actually the collider of our inspected object.
				{
					SceneView.RepaintAll(); //Constantly update scene view at this point (to avoid the circle handle jumping around as we click in and out of the scene view).
					Handles.DrawWireDisc(scHits[r].point, scHits[r].normal, _mp.hRadius); //Thanks to the RepaintAll() function above, the circle handle that we draw here gets updated constantly inside our scene view.
                    if (canPaint)
                    {
                        switch (Event.current.type)
                        {
                            case EventType.KeyDown:
                            {
                                if (Event.current.keyCode == _mp.paint)
                                {
                                    _t = Time2Die + _mp.delay;
                                    Paint(r, scHits);
                                }
                                break;
                            }
                        }
                    }
				}
				r++;
			}
		}

        switch(Event.current.type) //Increase/decrease the radius with the keyboard buttons I and O
        {
            case EventType.KeyDown:
            {
                if (Event.current.keyCode == _mp.increaseRadius)
                {
                    _mp.hRadius += 0.05f;
                }
                else if (Event.current.keyCode == _mp.decreaseRadius && _mp.hRadius > 0)
                    _mp.hRadius -= 0.05f;
                break;
            }
        }
    }
    #endregion

    #region Painting functions
    void Paint(int r, RaycastHit[] scHits)
    {
        if (_mp.setOfMeshesToPaint.Length == 0) //Display an error dialog box if we are trying to paint 'nothing' onto our GameObject.
        {
            EditorUtility.DisplayDialog("No meshes to paint...", "Please add at least one prefab mesh to the array of meshes to paint.", "Okay");
            return;
        }

        else if (_mp.setOfMeshesToPaint.Length == 1)
        {
            if (_mp.GetComponentInChildren<MeshBrushParent>() != null) 	//Check if we already have an empty Mesh "holder" GameObject in our GameObject's hierarchy. 
            //The MeshPaintParent script gets attached automatically to the mesh holder GameObject. Through him, we can access all of his children (ergo our painted meshes). 
            {
                if (_mp.meshCount > 1) //Paint multiple objects of the same type (since the array has a length of 1 here, and the nr. of meshes value is greater than 1.
                {
                    if (_mp.setOfMeshesToPaint[0] == null) //Check if we are trying to paint nothing onto our GameObject (anti-stupidity statement).
                    {
                        EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                        return;
                    }
                    PaintMultipleMeshes(scHits[r]);
                }
                else
                {
                    if (_mp.setOfMeshesToPaint[0] == null)
                    {
                        EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                        return;
                    }
                    GameObject _i = Instantiate(_mp.setOfMeshesToPaint[0], scHits[r].point, Quaternion.LookRotation(scHits[r].normal)) as GameObject; //And bamm: we painted our mesh! ;) But oh no D: It still has its Z axis aligned with the normal, which is obviously not what we want...
                    _i.transform.up = _i.transform.forward; //...But don't worry... here we align the local Y axis with the normal vector of our Raycast hit, this way the Y axis is the one pointing away from the surface on which we painted (since Unity is a Y-up engine) ;)
                    _i.transform.parent = _mp.GetComponentInChildren<MeshBrushParent>().transform; //Afterwards we set the instantiated object as a parent of the MeshPaintParent holder GameObject.
                    _i.name = _mp.setOfMeshesToPaint[0].name;	//Set the painted object's name to the one of the GameObject you assigned to the single array field.

                    if (_mp.autoStatic == true) _i.isStatic = true;

                    //Perform the randomizing operations and apply them to out painted meshes:
                    if (!_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                            ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
                    }
                    else if (!_mp.rWithinRange && _mp.uniformScale)
                    {
                        if (_mp.rScale > 0)
                            ApplyRandomScale(_i, _mp.rScale);
                    }
                    else if (_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rNonUniformRange != Vector4.zero)
                            ApplyRandomScale(_i, _mp.rNonUniformRange);
                    }
                    else
                    {
                        if (_mp.rUniformRange != Vector2.zero)
                            ApplyRandomScale(_i, _mp.rUniformRange);
                    }

                    //Constant, additive scale (adds up to the total scale after everything else):
                    if (!_mp.constUniformScale)
                    {
                        if (_mp.cScaleXYZ != Vector3.zero)
                            AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
                    }
                    else
                    {
                        if (_mp.cScale != 0)
                            AddConstantScale(_i, _mp.cScale);
                    }

                    //The next two if-statements apply the random rotation and the offset to the meshes.
                    if (_mp.rRot > 0)
                        ApplyRandomRotation(_i, _mp.rRot);
                    if (_mp.meshOffset != 0)
                        ApplyMeshOffset(_i, _mp.meshOffset);

                    Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
                }

            }
            else 	//Otherwise, if we are painting the first mesh and therefore don't have a holder object in our hierarchy yet, create one with a MeshBrushParent component on it and start painting normally.
            {    	//The holder parent avoids horrible organization of the separately painted meshes all around in our hierarchy view (instead, we group them all together as a child of the holder). 

                if (_mp.meshCount > 1) //If we are painting more than one object at once, do this:
                {
                    if (_mp.setOfMeshesToPaint[0] == null)
                    {
                        EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                        return;
                    }
                    GameObject parentObj = new GameObject("Painted meshes"); //This creates and sets up the above mentioned holder object.
                    parentObj.AddComponent<MeshBrushParent>();
                    parentObj.transform.rotation = _mp.transform.rotation;
                    parentObj.transform.parent = _mp.transform;
                    parentObj.transform.localPosition = new Vector3(0, 0, 0);

                    PaintMultipleMeshes(scHits[r], parentObj);
                }

                else
                {
                    if (_mp.setOfMeshesToPaint[0] == null)
                    {
                        EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                        return;
                    }
                    GameObject parentObj = new GameObject("Painted meshes"); //This creates and sets up the above mentioned holder object.
                    parentObj.AddComponent<MeshBrushParent>();
                    parentObj.transform.rotation = _mp.transform.rotation;
                    parentObj.transform.parent = _mp.transform;
                    parentObj.transform.localPosition = new Vector3(0, 0, 0);

                    GameObject _i = Instantiate(_mp.setOfMeshesToPaint[0], scHits[r].point, Quaternion.LookRotation(scHits[r].normal)) as GameObject;
                    _i.transform.up = _i.transform.forward;
                    _i.transform.parent = parentObj.transform; //Set the instantiated object as a parent of the "Painted meshes" object holder.
                    _i.name = _mp.setOfMeshesToPaint[0].name;	//Set the painted object's name to the one of the asset you assigned as the mesh to paint. 

                    if (_mp.autoStatic) _i.isStatic = true;

                    //The various states of the toggles:
                    if (!_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                            ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
                    }
                    else if (!_mp.rWithinRange && _mp.uniformScale)
                    {
                        if (_mp.rScale > 0)
                            ApplyRandomScale(_i, _mp.rScale);
                    }
                    else if (_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rNonUniformRange != Vector4.zero)
                            ApplyRandomScale(_i, _mp.rNonUniformRange);
                    }
                    else
                    {
                        if (_mp.rUniformRange != Vector2.zero)
                            ApplyRandomScale(_i, _mp.rUniformRange);
                    }

                    //Constant, additive scale (adds up to the total scale after everything else:
                    if (!_mp.constUniformScale)
                    { //Same here for the additive scale
                        if (_mp.cScaleXYZ != Vector3.zero)
                            AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
                    }
                    else
                    {
                        if (_mp.cScale != 0)
                            AddConstantScale(_i, _mp.cScale);
                    }

                    if (_mp.rRot > 0)
                        ApplyRandomRotation(_i, _mp.rRot);
                    if (_mp.meshOffset != 0)
                        ApplyMeshOffset(_i, _mp.meshOffset);

                    Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
                }
            }
        }
        else
        { //(if the array has more than 1 slot)

            if (_mp.GetComponentInChildren<MeshBrushParent>() != null) //This huge block of code has the same structure as the one seen above. It checks if there is already the holder, and if not it creates one for you.
            {
                if (_mp.meshCount > 1)
                {
                    for (int u = 0; u < _mp.setOfMeshesToPaint.Length; u++) //Check if every single field of the array has a GameObject assigned (this is necessary to avoid a disturbing error printed in the console).
                    {
                        if (_mp.setOfMeshesToPaint[u] == null)
                        {
                            EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                            return;
                        }
                    }

                    PaintMultipleMeshesWithArray(scHits[r]);
                }
                else
                {
                    for (int u = 0; u < _mp.setOfMeshesToPaint.Length; u++) //Check if every single field of the array has a GameObject assigned (this is necessary to avoid a disturbing error printed in the console).
                    {
                        if (_mp.setOfMeshesToPaint[u] == null)
                        {
                            EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                            return;
                        }
                    }
                    GameObject _i = Instantiate(_mp.setOfMeshesToPaint[Random.Range(0, _mp.setOfMeshesToPaint.Length)], scHits[r].point, Quaternion.LookRotation(scHits[r].normal)) as GameObject;
                    _i.transform.up = _i.transform.forward;
                    _i.transform.parent = _mp.GetComponentInChildren<MeshBrushParent>().transform;
                    //_i.name = _mp.setOfMeshesToPaint[0].name;

                    if (_mp.autoStatic == true) _i.isStatic = true;

                    //The various states of the toggles:
                    if (!_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                            ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
                    }
                    else if (!_mp.rWithinRange && _mp.uniformScale)
                    {
                        if (_mp.rScale > 0)
                            ApplyRandomScale(_i, _mp.rScale);
                    }
                    else if (_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rNonUniformRange != Vector4.zero)
                            ApplyRandomScale(_i, _mp.rNonUniformRange);
                    }
                    else
                    {
                        if (_mp.rUniformRange != Vector2.zero)
                            ApplyRandomScale(_i, _mp.rUniformRange);
                    }

                    //Constant, additive scale (adds up to the total scale after everything else:
                    if (!_mp.constUniformScale)
                    { //Same here for the additive scale
                        if (_mp.cScaleXYZ != Vector3.zero)
                            AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
                    }
                    else
                    {
                        if (_mp.cScale != 0)
                            AddConstantScale(_i, _mp.cScale);
                    }

                    //The next two if-statements apply the random rotation and the offset to the mesh.
                    if (_mp.rRot > 0)
                        ApplyRandomRotation(_i, _mp.rRot);
                    if (_mp.meshOffset != 0)
                        ApplyMeshOffset(_i, _mp.meshOffset);

                    Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
                }
            }
            else
            {
                if (_mp.meshCount > 1)
                {
                    for (int u = 0; u < _mp.setOfMeshesToPaint.Length; u++) //Check if every single field of the array has a GameObject assigned (this is necessary to avoid a disturbing error printed in the console).
                    {
                        if (_mp.setOfMeshesToPaint[u] == null)
                        {
                            EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                            return;
                        }
                    }
                    GameObject parentObj = new GameObject("Painted meshes"); //This creates and sets up the holder object.
                    parentObj.AddComponent<MeshBrushParent>();
                    parentObj.transform.rotation = _mp.transform.rotation;
                    parentObj.transform.parent = _mp.transform;
                    parentObj.transform.localPosition = new Vector3(0, 0, 0);

                    PaintMultipleMeshesWithArray(scHits[r], parentObj);
                }
                else
                {
                    for (int u = 0; u < _mp.setOfMeshesToPaint.Length; u++) //Check if every single field of the array has a GameObject assigned (this is necessary to avoid a disturbing error printed in the console).
                    {
                        if (_mp.setOfMeshesToPaint[u] == null)
                        {
                            EditorUtility.DisplayDialog("Warning!", "One or more fields in the array of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");
                            return;
                        }
                    }
                    GameObject parentObj = new GameObject("Painted meshes"); //This creates and sets up the above mentioned holder object.
                    parentObj.AddComponent<MeshBrushParent>();
                    parentObj.transform.rotation = _mp.transform.rotation;
                    parentObj.transform.parent = _mp.transform;
                    parentObj.transform.localPosition = new Vector3(0, 0, 0);

                    GameObject _i = Instantiate(_mp.setOfMeshesToPaint[0], scHits[r].point, Quaternion.LookRotation(scHits[r].normal)) as GameObject;
                    _i.transform.up = _i.transform.forward;
                    _i.transform.parent = parentObj.transform; //Set the instantiated object as a parent of the "Painted meshes" object holder.
                    _i.name = _mp.setOfMeshesToPaint[0].name;	//Set the painted object's name to the one of the asset you assigned as the mesh to paint. 

                    if (_mp.autoStatic) _i.isStatic = true;

                    //The various states of the toggles:
                    if (!_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                            ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
                    }
                    else if (!_mp.rWithinRange && _mp.uniformScale)
                    {
                        if (_mp.rScale > 0)
                            ApplyRandomScale(_i, _mp.rScale);
                    }
                    else if (_mp.rWithinRange && !_mp.uniformScale)
                    {
                        if (_mp.rNonUniformRange != Vector4.zero)
                            ApplyRandomScale(_i, _mp.rNonUniformRange);
                    }
                    else
                    {
                        if (_mp.rUniformRange != Vector2.zero)
                            ApplyRandomScale(_i, _mp.rUniformRange);
                    }

                    //Constant, additive scale (adds up to the total scale after everything else:
                    if (!_mp.constUniformScale)
                    { //Same here for the additive scale
                        if (_mp.cScaleXYZ != Vector3.zero)
                            AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
                    }
                    else
                    {
                        if (_mp.cScale != 0)
                            AddConstantScale(_i, _mp.cScale);
                    }

                    if (_mp.rRot > 0)
                        ApplyRandomRotation(_i, _mp.rRot);
                    if (_mp.meshOffset != 0)
                        ApplyMeshOffset(_i, _mp.meshOffset);

                    Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
                }
            }
        }
    }

    void PaintMultipleMeshes(RaycastHit scHit, GameObject parentObj)
    {
        for (int C = 0; C < _mp.meshCount; C++) //Hahahahah sorry I just had to ;D
        {
            GameObject _i = Instantiate(_mp.setOfMeshesToPaint[0], scHit.point, Quaternion.LookRotation(scHit.normal)) as GameObject;
            _i.transform.up = _i.transform.forward; //Here we align the local Y axis with the normal vector of our Raycast hit, this way the Y axis is the one pointing away from the surface on which we painted (since Unity is a Y-up engine) ;)
            _i.transform.parent = parentObj.transform; //Afterwards we set the instantiated object as a parent of the holder GameObject.
            _i.name = _mp.setOfMeshesToPaint[0].name;	//Set the painted object's name to the one of the asset you assigned as the "mesh to paint". 

            //And here comes the fun part: 
            //Let's make the meshes scatter around randomly inside the area of the circle brush :->
            
            float insetThreshold = (_mp.hRadius / 100 * _mp.inset);
            Vector3 insetV = new Vector3(Random.Range(-Random.insideUnitCircle.x * insetThreshold, Random.insideUnitCircle.x * insetThreshold), 0, Random.Range(-Random.insideUnitCircle.y * insetThreshold, Random.insideUnitCircle.y * insetThreshold));
            _i.transform.Translate(insetV);

            for (int i = 0; i < _mp.meshCount; i++) //Performs a raycast for each mesh created to reposition it correctly (adapting it to the shape of our GameObject).
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(_i.transform.position, -scHit.normal);
                for (int e = 0; e < hits.Length; e++)
                {
                    if (hits[e].collider == _mp.GetComponent<Collider>())
                    {
                        _i.transform.position = hits[e].point;
                        _i.transform.rotation = Quaternion.LookRotation(hits[e].normal);
                        _i.transform.up = _i.transform.forward;
                        ApplyMeshOffset(_i, _mp.meshOffset);
                    }
                }
            }

            if (_mp.autoStatic == true) _i.isStatic = true;

            if (!_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                    ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
            }
            else if (!_mp.rWithinRange && _mp.uniformScale)
            {
                if (_mp.rScale > 0)
                    ApplyRandomScale(_i, _mp.rScale);
            }
            else if (_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rNonUniformRange != Vector4.zero)
                    ApplyRandomScale(_i, _mp.rNonUniformRange);
            }
            else
            {
                if (_mp.rUniformRange != Vector2.zero)
                    ApplyRandomScale(_i, _mp.rUniformRange);
            }

            //Constant, additive scale (adds up to the total scale after everything else:
            if (!_mp.constUniformScale)
            { //Same here for the additive scale
                if (_mp.cScaleXYZ != Vector3.zero)
                    AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
            }
            else
            {
                if (_mp.cScale != 0)
                    AddConstantScale(_i, _mp.cScale);
            }

            //The next two if-statements apply the random rotation and the offset to the mesh.
            if (_mp.rRot > 0)
                ApplyRandomRotation(_i, _mp.rRot);
            if (_mp.meshOffset != 0)
                ApplyMeshOffset(_i, _mp.meshOffset);

            Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
        }
    }

    void PaintMultipleMeshes(RaycastHit scHit)
    {
        for (int C = 0; C < _mp.meshCount; C++) 
        {
            GameObject _i = Instantiate(_mp.setOfMeshesToPaint[0], scHit.point, Quaternion.LookRotation(scHit.normal)) as GameObject; 
            _i.transform.up = _i.transform.forward; 
            _i.transform.parent = _mp.GetComponentInChildren<MeshBrushParent>().transform;
            _i.name = _mp.setOfMeshesToPaint[0].name;	

            //And here comes the fun part: 
            //Let's make the meshes scatter randomly inside the area of the circle brush...
            float insetThreshold = (_mp.hRadius / 100 * _mp.inset);
            Vector3 insetV = new Vector3(Random.Range(-Random.insideUnitCircle.x * insetThreshold, Random.insideUnitCircle.x * insetThreshold), 0, Random.Range(-Random.insideUnitCircle.y * insetThreshold, Random.insideUnitCircle.y * insetThreshold));
            _i.transform.Translate(insetV);

            for (int i = 0; i < _mp.meshCount; i++)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(_i.transform.position, -scHit.normal);
                for (int e = 0; e < hits.Length; e++)
                {
                    if (hits[e].collider == _mp.GetComponent<Collider>())
                    {
                        _i.transform.position = hits[e].point;
                        _i.transform.rotation = Quaternion.LookRotation(hits[e].normal);
                        _i.transform.up = _i.transform.forward;
                        ApplyMeshOffset(_i, _mp.meshOffset);
                    }
                }
            }

            if (_mp.autoStatic == true) _i.isStatic = true;

            
            if (!_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                    ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
            }
            else if (!_mp.rWithinRange && _mp.uniformScale)
            {
                if (_mp.rScale > 0)
                    ApplyRandomScale(_i, _mp.rScale);
            }
            else if (_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rNonUniformRange != Vector4.zero)
                    ApplyRandomScale(_i, _mp.rNonUniformRange);
            }
            else
            {
                if (_mp.rUniformRange != Vector2.zero)
                    ApplyRandomScale(_i, _mp.rUniformRange);
            }

            //Constant, additive scale (adds up to the total scale after everything else:
            if (!_mp.constUniformScale)
            { //Same here for the additive scale
                if (_mp.cScaleXYZ != Vector3.zero)
                    AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
            }
            else
            {
                if (_mp.cScale != 0)
                    AddConstantScale(_i, _mp.cScale);
            }

            //The next two if-statements apply the random rotation and the offset to the mesh.
            if (_mp.rRot > 0)
                ApplyRandomRotation(_i, _mp.rRot);
            if (_mp.meshOffset != 0)
                ApplyMeshOffset(_i, _mp.meshOffset);

            Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
        }
    }

    void PaintMultipleMeshesWithArray(RaycastHit scHit, GameObject parentObj)
    {
        for (int C = 0; C < _mp.meshCount; C++)
        {
            int r = Random.Range(0, _mp.setOfMeshesToPaint.Length);
            GameObject _i = Instantiate(_mp.setOfMeshesToPaint[r], scHit.point, Quaternion.LookRotation(scHit.normal)) as GameObject;
            _i.transform.up = _i.transform.forward; 
            _i.transform.parent = parentObj.transform; 
            _i.name = _mp.setOfMeshesToPaint[r].name;	

            //And here comes the fun part: 
            //Let's make the meshes scatter randomly inside the area of the circle brush...

            float insetThreshold = (_mp.hRadius / 100 * _mp.inset);
            Vector3 insetV = new Vector3(Random.Range(-Random.insideUnitCircle.x * insetThreshold, Random.insideUnitCircle.x * insetThreshold), 0, Random.Range(-Random.insideUnitCircle.y * insetThreshold, Random.insideUnitCircle.y * insetThreshold));
            _i.transform.Translate(insetV);

            for (int i = 0; i < _mp.meshCount; i++)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(_i.transform.position, -scHit.normal);
                for (int e = 0; e < hits.Length; e++)
                {
                    if (hits[e].collider == _mp.GetComponent<Collider>())
                    {
                        _i.transform.position = hits[e].point;
                        _i.transform.rotation = Quaternion.LookRotation(hits[e].normal);
                        _i.transform.up = _i.transform.forward;
                        ApplyMeshOffset(_i, _mp.meshOffset);
                    }
                }
            }
            if (_mp.autoStatic == true) _i.isStatic = true;

            if (!_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                    ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
            }
            else if (!_mp.rWithinRange && _mp.uniformScale)
            {
                if (_mp.rScale > 0)
                    ApplyRandomScale(_i, _mp.rScale);
            }
            else if (_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rNonUniformRange != Vector4.zero)
                    ApplyRandomScale(_i, _mp.rNonUniformRange);
            }
            else
            {
                if (_mp.rUniformRange != Vector2.zero)
                    ApplyRandomScale(_i, _mp.rUniformRange);
            }

            //Constant, additive scale (adds up to the total scale after everything else:
            if (!_mp.constUniformScale)
            { //Same here for the additive scale
                if (_mp.cScaleXYZ != Vector3.zero)
                    AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
            }
            else
            {
                if (_mp.cScale != 0)
                    AddConstantScale(_i, _mp.cScale);
            }

            //The next two if-statements apply the random rotation and the offset to the mesh.
            if (_mp.rRot > 0)
                ApplyRandomRotation(_i, _mp.rRot);
            if (_mp.meshOffset != 0)
                ApplyMeshOffset(_i, _mp.meshOffset);

            Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
        }
    }
   
    void PaintMultipleMeshesWithArray(RaycastHit scHit)
	{
        for (int C = 0; C < _mp.meshCount; C++) 
        {
            int r = Random.Range(0, _mp.setOfMeshesToPaint.Length);
            GameObject _i = Instantiate(_mp.setOfMeshesToPaint[r], scHit.point, Quaternion.LookRotation(scHit.normal)) as GameObject;
            _i.transform.up = _i.transform.forward;
            _i.transform.parent = _mp.GetComponentInChildren<MeshBrushParent>().transform; 
            _i.name = _mp.setOfMeshesToPaint[r].name;	

            //And here comes the fun part: 
            //Let's make the meshes scatter randomly inside the area of the circle brush...
            float insetThreshold = (_mp.hRadius / 100 * _mp.inset);
            Vector3 insetV = new Vector3(Random.Range(-Random.insideUnitCircle.x * insetThreshold, Random.insideUnitCircle.x * insetThreshold), 0, Random.Range(-Random.insideUnitCircle.y * insetThreshold, Random.insideUnitCircle.y * insetThreshold));
            _i.transform.Translate(insetV);

            for (int i = 0; i < _mp.meshCount; i++)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(_i.transform.position, -scHit.normal);
                for (int e = 0; e < hits.Length; e++)
                {
                    if (hits[e].collider == _mp.GetComponent<Collider>())
                    {
                        _i.transform.position = hits[e].point;
                        _i.transform.rotation = Quaternion.LookRotation(hits[e].normal);
                        _i.transform.up = _i.transform.forward;
                        ApplyMeshOffset(_i, _mp.meshOffset);
                    }
                }
            }

            if (_mp.autoStatic == true) _i.isStatic = true;

            
            if (!_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rScaleW > 0 || _mp.rScaleH > 0)
                    ApplyRandomScale(_i, _mp.rScaleW, _mp.rScaleH);
            }
            else if (!_mp.rWithinRange && _mp.uniformScale)
            {
                if (_mp.rScale > 0)
                    ApplyRandomScale(_i, _mp.rScale);
            }
            else if (_mp.rWithinRange && !_mp.uniformScale)
            {
                if (_mp.rNonUniformRange != Vector4.zero)
                    ApplyRandomScale(_i, _mp.rNonUniformRange);
            }
            else
            {
                if (_mp.rUniformRange != Vector2.zero)
                    ApplyRandomScale(_i, _mp.rUniformRange);
            }

            //Constant, additive scale (adds up to the total scale after everything else:
            if (!_mp.constUniformScale)
            { //Same here for the additive scale
                if (_mp.cScaleXYZ != Vector3.zero)
                    AddConstantScale(_i, _mp.cScaleXYZ.x, _mp.cScaleXYZ.y, _mp.cScaleXYZ.z);
            }
            else
            {
                if (_mp.cScale != 0)
                    AddConstantScale(_i, _mp.cScale);
            }

            //The next two if-statements apply the random rotation and the offset to the mesh.
            if (_mp.rRot > 0)
                ApplyRandomRotation(_i, _mp.rRot);
            if (_mp.meshOffset != 0)
                ApplyMeshOffset(_i, _mp.meshOffset);

            Undo.RegisterCreatedObjectUndo(_i, _i.name); //Allow the "undo" operation for the creation of meshes.
        }
	}
    #endregion

    #region Other functions
    void ApplyRandomScale(GameObject sMesh, float W, float H) //Apply some random scale (non-uniformly) to the freshly painted object.
    {
        float rW, rH;
        rW = W*Random.value + 0.15f;
        rH = H*Random.value + 0.15f;
        sMesh.transform.localScale = new Vector3(rW,rH,rW);
    }
    
    void ApplyRandomScale(GameObject sMesh, float U) //Here I overload the ApplyRandomScale function for the uniform random scale.
    {
        float r;
        r = U * Random.value + 0.15f;
        sMesh.transform.localScale = new Vector3(r,r,r);
    }
    
    void ApplyRandomScale(GameObject sMesh, Vector2 range) //Overload for the customized uniform random scale.
    {
        float s = Random.Range(range.x,range.y);
        sMesh.transform.localScale = new Vector3(s,s,s);
	}

	void ApplyRandomScale(GameObject sMesh, Vector4 ranges) //Non-uniform custom random range scale.
	{
		float rW = Random.Range(ranges.x,ranges.y);
		float rH = Random.Range(ranges.z,ranges.w);
		sMesh.transform.localScale = new Vector3(rW,rH,rW);
	}
	
	void AddConstantScale(GameObject sMesh, float X, float Y, float Z){ //Same procedure for the constant scale methods.
		sMesh.transform.localScale += new Vector3(X,Y,Z);
	}
	
	void AddConstantScale(GameObject sMesh, float S){
		sMesh.transform.localScale += new Vector3(S,S,S);
	}

	void ApplyRandomRotation(GameObject rMesh, float rot){ //Apply some random rotation (around local Y axis) to the freshly painted mesh.
        float randomRotation = Random.Range(0f, 3.60f * rot);
		rMesh.transform.Rotate(new Vector3(0,randomRotation,0));
	}

	void ApplyMeshOffset(GameObject oMesh, float offset){ //Y-axis offset
		oMesh.transform.Translate(new Vector3(0,offset/100,0)); //We divide offset by 100 since we want to use centimeters as our offset unit (because 1cm = 0.01m)
    }
    #endregion

}

/*
 * 
 * Raphael Beck, 2014
 * 
 */
