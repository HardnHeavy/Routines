using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.Jireugi.U3DExtension;

namespace GGJ2016.Routines.Model {
	public class Player {

		protected List<Card> _cards = null;
		public List<Card> Cards {
			get { return _cards; }
			//set { _cards = value; }
		}// property

		protected Routine _routine = null;
		public Routine Routine {
			get { return _routine; }
			set { _routine = value; }
		}// property

		protected Deck _deck = null;

		protected int _index = -1;
		public int Index {
			get { return _index; }
			set { _index = value; }
		}// property

		protected bool _didChangeRoutine = false;
		public bool DidChangeRoutine {
			get { return _didChangeRoutine; }
			set { _didChangeRoutine = value; }
		}// property



		public Player(int index, Deck deck, int cardsInitially){
			_index = index;

			_deck = deck;

			_cards = new List<Card>();
			for (int i = 0; i < cardsInitially; i++) {
				Card card = _deck.DrawCard ();
				card.Position = _index;
				_cards.Add (card);
			}// for

			_didChangeRoutine = false;

		}// Player



	
	}// class

}// namespace
