using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calc {

	// Helper Function
	public static float Approach (float Start, float End, float Shift) {
		if (Start < End)
			return Mathf.Min (Start + Shift, End);
		else
			return Mathf.Max(Start - Shift, End);
	}
}
