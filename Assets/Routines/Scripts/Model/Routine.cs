using UnityEngine;
using System.Collections;

namespace GGJ2016.Routines.Model {

	public delegate bool IsPlacementOk(Card played, Card onboard, int column, int row); 


	public class Routine {

		protected string _text = string.Empty;
		public string Text {
			get { return _text; }
			//set { _text = value; }
		}// property

		// Some id for easier comparison of equality against another Routine without having to rely on reference/instance comparison.
		protected int _id = -1;
		public int Id {
			get { return _id; }
			//set { _id = value; }
		}// property

		protected IsPlacementOk _validator = null;
		public IsPlacementOk Validator {
			get { return _validator; }
			//set { _validator = value; }
		}// property



		public Routine(int id, string text, IsPlacementOk validator) {
			_id = id;
			_text = text;
			_validator = validator;
		}// Card


		public override string ToString (){
			return "Routine_" + _id + "_" + _text;
		}// ToString

	}// class

}// namespace