using UnityEngine;
using System.Collections;

namespace GGJ2016.Routines.Model {



	public class Card {

		public static int POSITION_RESERVE = -2;
		public static int POSITION_BOARD = -1;

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

		// Defines where the card currently is:
		// in the reserve heap (POSITION_RESERVE),
		// on the board (POSITION_BOARD)
		// or among the hand of a player (= player index)
		protected int _position = POSITION_RESERVE;
		public int Position {
			get { return _position; }
			set { _position = value; }
		}// property



		public Card(CardColor color, int value, int position) {
			_color = color;
			_value = value;
			_position = position;
		}// Card


		public override string ToString (){
			return "Card_" + _color + "_" + _value + "@" + _position;
		}// ToString

	}// class

}// namespace