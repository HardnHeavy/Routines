using UnityEngine;
using System.Collections;

namespace GGJ2016.Routines.Model {



	public class Card {

		protected CardColor _color = CardColor.None;
		public CardColor Color {
			get { return _color; }
			//set { _color = value; }
		}// property

		protected int _value = -1;
		public int Value {
			get { return _value; }
			//set { _value = value; }
		}// property



		public Card(CardColor color, int value) {
			_color = color;
			_value = value;
		}// Card

	}// class

}// namespace