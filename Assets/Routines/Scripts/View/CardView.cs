using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GGJ2016.Routines.Model;

namespace GGJ2016.Routines.Controller {
	public class CardView : MonoBehaviour {

		public Material[] MatColors = null;
		public Material[] MatNumbers = null;

		public GameObject Number = null;

		protected Card _cardModel = null;
		public Card CardModel {
			get { return _cardModel; }
			set {
				_cardModel = value; 
				// Force the CardView and CardModel to be in sync.
				this.SetCardColor(_cardModel.Color);
				this.SetCardNumber (_cardModel.Value);
			}// set
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
