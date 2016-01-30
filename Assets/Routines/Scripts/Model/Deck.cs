using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.Jireugi.U3DExtension;

namespace GGJ2016.Routines.Model {

	public enum CardColor {
		None,
		Red,
		Green,
		Blue,
		Yellow
	};

	public class Deck {
		
		protected int _minValue = 1;
		public int MinValue {
			get { return _minValue; }
			//set { _minValue = value; }
		}// property

		protected int _maxValue = 10;
		public int MaxValue {
			get { return _maxValue; }
			//set { _maxValue = value; }
		}// property

		protected int _setsPerColor = 4;
		public int SetsPerColor {
			get { return _setsPerColor; }
			//set { _setsPerColor = value; }
		}// property


		protected List<CardColor> _forbiddenColors = null;
		public List<CardColor> ForbiddenColors {
			get { return _forbiddenColors; }
			//set { _forbiddenColors = value; }
		}// property

		protected List<Card> _cardsInReserve = null;
		protected List<Card> _cardsInPlay = null;



		public Deck(
				int cardMinVal,
				int cardMaxVal,
				int setsPerColor,
				List<CardColor> forbiddenColors
		) {
			_minValue = cardMinVal;
			_maxValue = cardMaxVal;

			_setsPerColor = SetsPerColor;

			_forbiddenColors = forbiddenColors;

			_cardsInReserve = new List<Card> ();
			_cardsInPlay = new List<Card> ();

			GenerateDeck ();

			DbgOut.Log ("New Deck created.");

		}// Card



		protected void GenerateDeck(){
			for (int col = 0; col < (System.Enum.GetNames (typeof(CardColor)).Length); col++) {

				CardColor color = (CardColor)System.Enum.ToObject (typeof(CardColor), col);
				if (_forbiddenColors.Contains (color))
					continue;

				for (int setCount = 0; setCount < _setsPerColor; setCount++){
					for (int val = _minValue; val <= _maxValue; val++) {
						_cardsInReserve.Add(new Card(color, val));
					}// for
				}// for
					
			}// for
		}// GenerateDeck



		public Card DrawCard(){
			Card card = null;

			if (_cardsInReserve.Count > 0) {
				card = _cardsInReserve [MathHelper.Rand.Next (_cardsInReserve.Count)];
				_cardsInReserve.Remove (card);
				_cardsInPlay.Add (card);
			}// fi

			return card;
		}// DrawCard



		public void ReturnCard(Card card){
			_cardsInReserve.Add (card);
			_cardsInPlay.Remove (card);
		}// ReturnCard



		public void ReturnAllCards(){
			_cardsInReserve.AddRange (_cardsInPlay);
			_cardsInPlay.Clear ();
		}// ReturnAllCards


	}// class

}// namespace