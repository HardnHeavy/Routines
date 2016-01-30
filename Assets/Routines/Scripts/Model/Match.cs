using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GGJ2016.Routines.Model {

	public class Match {

		#region infrastructure
		protected int _playercount = 0;
		public int PlayerCount {
			get { return _playercount; }
			//set { _playercount = value; }
		}// property

		protected Player[] _players = null;

		protected Board _board = null;
		public Board PlayBoard {
			get { return _board; }
			//set { _board = value; }
		}// property

		protected Deck _deck = null;
		public Deck PlayDeck {
			get { return _deck; }
			//set { _deck = value; }
		}// property

		#endregion infrastructure



		#region match state

		// Index of the player that is to take action.
		protected int _playerAction = -1;
		public int PlayerAction {
			get { return _playerAction; }
			set { _playerAction = value; }
		}// property

		protected int _winner = -1;
		public int Winner {
			get { return _winner; }
			set { _winner = value; }
		}// property

		#endregion match state






		public Match(
				int playerCount,
				int cardsPerPlayer,
				int boardrows,
				int boardcolumns,
				int cardsMinValue,
				int cardsMaxValue,
				int cardsSetsPerColor,
				List<CardColor> forbiddenCardColors
		){

			_deck = new Deck (cardsMinValue, cardsMaxValue, cardsSetsPerColor, forbiddenCardColors);

			_playercount = playerCount;

			_players = new Player[_playercount];
			for (int i = 0; i < _playercount; i++) {
				_players [i] = new Player (_deck, cardsPerPlayer);
			}// for

			_playerAction = 0;

			_winner = -1;

			_board = new Board (boardrows, boardcolumns);


		}// Match



		public Player GetPlayer(int index){
			if (index < 0 || index >= _playercount)
				return null;
			return _players [index];
		}// GetPlayer


	}// class

}// namespace
