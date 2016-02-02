using UnityEngine;
using System.Collections;


namespace GGJ2016.Routines.View {
	public class Highlightable : MonoBehaviour {


		public GameObject[] HighlightObjects = null;
		public Material HighlightMaterial = null;
		protected bool _isHighlighted = false;



		public void Highlight(){
			if (HighlightObjects != null && !_isHighlighted){
				foreach(GameObject obj in HighlightObjects){
					Renderer rend = obj.GetComponent<Renderer>();
					if (rend != null){
						Material[] mats = new Material[rend.materials.Length+1];
						for (int i=0; i<rend.materials.Length; i++){
							mats[i] = rend.materials[i];
						}// for
						mats[rend.materials.Length] = HighlightMaterial;
						rend.materials = mats;
					}// fi
				}// foreach
				_isHighlighted = true;
			}// fi
		}// Highlight



		public void Unhighlight(){
			if (_isHighlighted){
				foreach(GameObject obj in HighlightObjects){
					Renderer rend = obj.GetComponent<Renderer>();
					Material[] mats = new Material[rend.materials.Length-1];
					for (int i=0; i<rend.materials.Length-1; i++){
						mats[i] = rend.materials[i];
					}// for
					rend.materials = mats;
				}// foreach
				_isHighlighted = false;
			}// fi
		}// Unhighlight

	}// class
}// namespace
