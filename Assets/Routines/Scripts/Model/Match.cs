using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.Jireugi.U3DExtension;

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

		protected List<Routine> _allRoutines = null;
		public List<Routine> AllRoutines {
			get { return _allRoutines; }
			//set { _allRoutines = value; }
		}// property

		protected List<Routine> _routinesInReserve = null;
		protected List<Routine> _routinesInPlay = null;

		#endregion infrastructure



		#region match state

		// Index of the player that is to take action.
		protected int _playerAction = -1;
		public int PlayerAction {
			get { return _playerAction; }
			set { _playerAction = value; }
		}// property

		// true: player needs to draw a card; false: player does not need to draw a card.
		protected bool _playerMustDraw = false;
		public bool PlayerMustDraw {
			get { return _playerMustDraw; }
			set { _playerMustDraw = value; }
		}// property

		// true: player tried to guess a routine during the current round; false: the player did not in this round.
		protected bool _playerDidGuess = false;
		public bool PlayerDidGuess {
			get { return _playerDidGuess; }
			set { _playerDidGuess = value; }
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
				_players [i] = new Player (i, _deck, cardsPerPlayer);
			}// for

			_playerAction = MathHelper.Rand.Next (playerCount);

			_winner = -1;

			_board = new Board (boardrows, boardcolumns);

			_playerMustDraw = false;
			_playerDidGuess = false;

			_allRoutines = new List<Routine> ();
			_routinesInReserve = new List<Routine> ();
			_routinesInPlay = new List<Routine> ();

		}// Match



		public Player GetPlayer(int index){
			if (index < 0 || index >= _playercount)
				return null;
			return _players [index];
		}// GetPlayer



		public void AddNewRoutine(Routine routine){
			_routinesInReserve.Add (routine);
			_allRoutines.Add (routine);
		}// AddNewRoutine



		public Routine DrawRoutine(){
			Routine routine = null;

			if (_routinesInReserve.Count > 0) {
				routine = _routinesInReserve [MathHelper.Rand.Next (_routinesInReserve.Count)];
				_routinesInReserve.Remove (routine);
				_routinesInPlay.Add (routine);
			}// fi

			return routine;
		}// DrawRoutine



		public void ReturnRoutine(Routine routine){
			_routinesInReserve.Add (routine);
			_routinesInPlay.Remove (routine);
		}// ReturnRoutine



		public void ReturnAllRoutines(){
			foreach (Routine routine in _routinesInPlay) {
				_routinesInReserve.Add (routine);
			}// foreach
			_routinesInPlay.Clear ();
		}// ReturnAllRoutines


	}// class

}// namespace
