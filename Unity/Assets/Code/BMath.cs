using UnityEngine;
using System.Collections;

public class BMath : MonoBehaviour {

	public static Vector3 PointLineIntersect(Transform lineStart, Transform lineEnd, Transform thePoint){
		
		Vector2 lineStartV;
		Vector2 lineEndV;
		Vector2 thePointV; 
		
		lineStartV.x = lineStart.position.x; //getting these into 2d space.
		lineStartV.y = lineStart.position.z;
		lineEndV.x = lineEnd.position.x;
		lineEndV.y = lineEnd.position.z;
		thePointV.x = thePoint.position.x;
		thePointV.y = thePoint.position.z;
		
		float xMax; //getting the bounds of the line segment. For clamping purposes.
		float xMin; 
		float yMax; 
		float yMin; 
		if (lineStartV.x >= lineEndV.x) {
			xMax = lineStartV.x;
			xMin = lineEndV.x; 
		}
		else {
			xMax = lineEndV.x;
			xMin = lineStartV.x; 
		}
		if (lineStartV.y >= lineEndV.y) {
			yMax = lineStartV.y;
			yMin = lineEndV.y; 
		}
		else {
			yMax = lineEndV.y;
			yMin = lineStartV.y; 
		}
		
		float slope = (lineStartV.y - lineEndV.y)/(lineStartV.x - lineEndV.x) ; //convert the points into 'standard form'
		float theC = (slope * lineStartV.x * -1) + lineStartV.y;
		float theB = 1; 
		float theA = slope * -1; 
		
		if (theA < 0) {  //pretty sure this does nothing, remenants from debugging. 
			theA *= -1; 
			theB *= -1; 
			theC *= -1; 
		}
		float intersectX =  (((theB*(theB*thePointV.x - theA*thePointV.y)) - theA *theC) / (theA*theA + theB * theB)); //the actual math for finding hte point on the line
		float intersectZ = (((theA*(-1*theB*thePointV.x + theA*thePointV.y)) - theB * theC) / (theA*theA + theB * theB));
		intersectX = Mathf.Clamp (intersectX, xMin, xMax); //clamp that bitch to the line segmen
		intersectZ = Mathf.Clamp (intersectZ, yMin, yMax); 
		
		return new Vector3 (intersectX, 0, intersectZ); 
	}

	public static Vector3 DoubleLineIntersect(Transform line1Start, Transform line1End, Transform line2Start, Transform line2End){
		Vector2 _p1; //declare the vector 2's that will be used 
		Vector2 _p2;
		Vector2 _p3;
		Vector2 _p4; 
		
		_p1.x = line1Start.position.x;  //get the vector 2's set up
		_p1.y = line1Start.position.z; 
		_p2.x = line1End.position.x;
		_p2.y = line1End.position.z; 
		_p3.x = line2Start.position.x;
		_p3.y = line2Start.position.z; 
		_p4.x = line2End.position.x;
		_p4.y = line2End.position.z; 
		
		float intersectX = ((_p1.x * _p2.y - _p1.y * _p2.x) * (_p3.x - _p4.x) - (_p1.x - _p2.x) * (_p3.x * _p4.y - _p3.y * _p4.x)) /  //do the thing
			((_p1.x - _p2.x) * (_p3.y - _p4.y) - (_p1.y - _p2.y) * (_p3.x - _p4.x));
		float intersectZ = ((_p1.x*_p2.y - _p1.y*_p2.x)*(_p3.y - _p4.y) - (_p1.y - _p2.y)*(_p3.x*_p4.y - _p3.y*_p4.x)) /
			((_p1.x - _p2.x) * (_p3.y - _p4.y) - (_p1.y - _p2.y) * (_p3.x - _p4.x));
		
		return new Vector3 (intersectX, 0, intersectZ);  //spit it back out as a vector 3. 
	}
}
