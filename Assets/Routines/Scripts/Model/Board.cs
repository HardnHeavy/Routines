using UnityEngine;
using System.Collections;

namespace GGJ2016.Routines.Model {



	public class Board {

		protected Card[][] _cards = null;
		//public Card[][] Cards {
		//	get { return _cards; }
			//set { _cards = value; }
		//}// property

		protected int _rows = 3;
		public int Rows {
			get { return _rows; }
			//set { _rows = value; }
		}// property

		protected int _columns = 3;
		public int Columns {
			get { return _columns; }
			//set { _columns = value; }
		}// property



		public Board(int rows, int columns) {
			_rows = rows;
			_columns = columns;

			_cards = new Card[rows][];
			for (int row=0; row<rows; row++){
				_cards [row] = new Card[columns];
				//for (int col=0; col<columns; col++){
				//	_cards [row] [col] = null;
				//}// for
			}// for

		}// Card



		public Card GetCardAt(int row, int column){
			return _cards [Mathf.Clamp (row, 0, _rows)] [Mathf.Clamp (column, 0, _columns)];
		}// GetCardAt



		public void SetCardAt(int row, int column, Card card){
			_cards [Mathf.Clamp (row, 0, _rows)] [Mathf.Clamp (column, 0, _columns)] = card;
		}// SetCardAt

	}// class

}// namespace