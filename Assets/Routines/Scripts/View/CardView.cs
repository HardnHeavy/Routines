using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GGJ2016.Routines.Model;

namespace GGJ2016.Routines.View {

	public class CardView : MonoBehaviour {

		public Material[] MatColors = null;
		public Material[] MatNumbers = null;

		public GameObject Number = null;

		protected Card _cardModel = null;
		public Card CardModel {
			get { return _cardModel; }
			set {
				_cardModel = value; 
				if (_cardModel != null) {
					// Force the CardView and CardModel to be in sync.
					this.SetCardColor (_cardModel.Color);
					this.SetCardNumber (_cardModel.Value);
				}// fi
			}// set
		}// property

		protected int _column = -1;
		public int Column {
			get { return _column; }
			set { _column = value; }
		}// property

		protected int _row = -1;
		public int Row {
			get { return _row; }
			set { _row = value; }
		}// property


		public void SetCardColor(CardColor color){
			Renderer rend = this.gameObject.GetComponent<Renderer> ();
			rend.material = MatColors [((int)color)-1];
		}// SetCardColor

		public void SetCardNumber(int number){
			Renderer rend = Number.GetComponent<Renderer> ();
			rend.material = MatNumbers [number-1];
		}// SetCardNumber

	}// class
}// namespace
