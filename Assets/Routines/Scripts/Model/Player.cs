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

		protected Deck _deck = null;



		public Player(Deck deck, int cardsInitially){
			_deck = deck;

			_cards = new List<Card>();
			for (int i = 0; i < cardsInitially; i++) {
				_cards.Add (_deck.DrawCard());
			}// for

		}// Player



	
	}// class

}// namespace
