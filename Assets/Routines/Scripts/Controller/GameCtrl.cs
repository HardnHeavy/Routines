using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.Jireugi.U3DExtension;
using com.Jireugi.U3DExtension.UI;

using GGJ2016.Routines.Model;
using GGJ2016.Routines.View;

namespace GGJ2016.Routines.Controller {
	public class GameCtrl : MonoBehaviour {

		private static GameCtrl _singleton = null;
		public static GameCtrl Get() {
			return _singleton;
		}// Get

		#region Match management

		#region Match Parameters
		public int ParamPlayerCount = 4;
		public int ParamCardsPerPlayer = 9;
		public int ParamBoardRows = 3;
		public int ParamBoardColumns = 3;
		public int ParamCardsMinValue = 1;
		public int ParamCardsMaxValue = 10;
		public int ParamCardsSetsPerColor = 4;
		public bool ParamCardColorForbiddenRed = false;
		public bool ParamCardColorForbiddenGreen = false;
		public bool ParamCardColorForbiddenBlue = false;
		public bool ParamCardColorForbiddenYellow = false;
		#endregion Match Parameters

		protected Match _match = null;

		protected bool _matchRunning = false;

		protected bool _startNextRound = false;

		#region Rules
		protected Card _lastPlayedCard = null;
		protected Card _lastBoardCard = null;
		protected int _lastBoardColumn = -1;
		protected int _lastBoardRow = -1;
		#endregion Rules

		#endregion Match management



		#region View data

		public GameObject PrefabCard = null;



		// This is the parent object for all cards (and perhaps other items?) on the board.
		public GameObject ParentBoard = null;

		// Defines the rotation to be applied for a card that is placed on the board.
		public Vector3 CardOnBoardRotation = Vector3.zero;

		// Defines the offset between cards placed on the board.
		public Vector3 CardOnBoardOffset = Vector3.one;

		protected CardView[][] _cardViewsBoard = null;



		public GameObject CardCursor = null;



		// These are the parent objects for all cards (and perhaps other items?) of each player.
		public GameObject[] ParentPlayerHands = null;

		// Defines the rotation to be applied for a card that is in a player's hand.
		public Vector3 CardPlayerHandRotation = Vector3.zero;

		// Defines the offset between cards of a player hand.
		public Vector3 CardPlayerHandOffset = Vector3.one;

		public int CardsMaxPerRow = 9;

		public List<CardView>[] _cardViewsPerPlayer = null;



		// GameObjects that define the position and rotation to be applied to the camera when it looks into the hand of a player.
		public GameObject[] CamPosPlayers = null;

		public GameObject CamPosBoard = null;

		public float TimeCamFlightNextPlayer = 1.0f;
		public float TimeCamFlightBoard = 0.25f;
		protected Vector3 _camFlightStartPos = Vector3.zero;
		protected Vector3 _camFlightStartRot = Vector3.zero;
		protected Vector3 _camFlightTargetPos = Vector3.zero;
		protected Vector3 _camFlightTargetRot = Vector3.zero;
		protected float _camFlightTimeTotal = 0.0f;
		protected float _camFlightTimeLeft = 0.0f;
		protected bool _camFlightActive = false;

		protected GameObject _camPosCurrent = null;
		protected GameObject _camPosStashed = null;

		protected bool _camPosBoardActive = false;

		#region UI

		public GameObject BtnMainOptionsVisible = null;
		protected bool _mainOptionsVisible = false;
		public GameObject GameLogo = null;
		public GameObject BtnInstructions = null;
		public GameObject BtnBeginNewMatch = null;
		public GameObject BtnAbout = null;
		public UIExtHasText TxtLargeText = null;

		public GameObject BtnChangeRoutine = null;
		public GameObject BtnGuessRoutine = null;
		public GameObject BtnDrawCard = null;
		protected bool _showMyRoutine = false;
		public GameObject BtnMyRoutine = null;
		protected bool _showAllRoutines = false;
		public GameObject BtnAllRoutines = null;
		public UIExtHasText TxtSingleRoutine = null;
		public UIExtHasText TxtStatus = null;
		public UIExtHasText TxtInstructDrawCard = null;

		protected bool _guessRoutinePlayerVisible = false;
		protected bool _guessRoutineSelectVisible = false;
		protected int _guessPlayerId = -1;
		protected Routine _guessRoutine = null;
		public UIExtHasText TxtInstructGuessRoutine = null;
		public GameObject BtnGuess1 = null;
		public GameObject BtnGuess2 = null;
		public GameObject BtnGuess3 = null;
		protected int _displayedRoutine = 0;
		public GameObject BtnSelectRoutine = null;
		public GameObject BtnPrevRoutine = null;
		public GameObject BtnNextRoutine = null;

		#endregion UI

		#endregion View data



		#region input
		protected KeyDebouncer _keys = null;

		protected float _maxDistInteraction = 10.0f;

		protected GameObject _interacttarget = null;
		protected Highlightable _highlightable = null;
		protected GameObject _selectedCard = null;
		protected Card _selectedCardModel = null;
		protected GameObject _selectedTarget = null;

		// Reference to the plane in front of the camera to hinder the player 
		// of accidently selecting cards when some input to a UI gadget is required.
		public GameObject InputWall = null;
		#endregion input



		#region Debug
		// TODO
		#endregion Debug





		protected void Awake() {
			if (_singleton != null) {
				GameObject.Destroy(this.gameObject);
				return;
			}// fi
			_singleton = this;

			//DbgOut.LogEnable = false;

			_camPosCurrent = CamPosPlayers [0];
		}// Awake



		protected void Start() {

			_keys = this.gameObject.GetComponent<KeyDebouncer> ();
			_keys.AddSetup (KeyCode.Return, 0.1f);

			CreateInfrastructure();

			//MatchView.SetModel(_match, LevelController.Level);

			//BeginMatch ();

			_mainOptionsVisible = false;
			OnUIMainOptions ();
			BtnMainOptionsVisible.SetActive (false);
			SetVisiblePlayerControls (false);
			_guessRoutinePlayerVisible = true;
			OnUITryGuessRoutine ();

		}// Start

		
		protected void Update () {
			DoCameraFlight();
		}// Update



		protected void FixedUpdate(){
			if (_startNextRound) {
				_startNextRound = false;
				PrepareTurn ();
			}// fi
		}// FixedUpdate



		protected void LateUpdate() {
			if (Input.GetKeyDown(KeyCode.Space) && _keys.TryPress(KeyCode.Space)) {
				BeginMatch ();
			}// fi

			// Show board cards.
			if (_selectedCard == null && Input.GetKey (KeyCode.Return)){// && _keys.TryPress (KeyCode.Return)) {
				if (!_camPosBoardActive) {
					_camPosBoardActive = true;
					_camPosStashed = _camPosCurrent;
					ApplyCamPosition (CamPosBoard, TimeCamFlightBoard);
				}// fi
			} else {
				if (_camPosBoardActive) {
					ApplyCamPosition (_camPosStashed, TimeCamFlightBoard);
					_camPosBoardActive = false;
				}// fi
			}// fi

			// Check if card is targeted.
			if (_matchRunning){
				
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					//Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
				RaycastHit hitinfo;

				//Vector3 hit = Vector3.zero;
				bool interaction = false;
				if (Physics.Raycast(ray, out hitinfo, 1<<LayerMask.NameToLayer("Card"))){

					/*
					if ((hitinfo.collider.gameObject.layer & LayerMask.NameToLayer("Card")) != 0){
						Debug.Log("Hit card=" + hitinfo.collider.gameObject.name);
					} else {
						Debug.Log("Hit noncard with objectname=" + hitinfo.collider.gameObject.name);
					}// fi
					*/
					/*
					//if (Physics.Raycast(ray, out hitinfo, 100.0f)){
					hit = hitinfo.point;
					if (hit.y < MaxDepth)
						hit.y = MaxDepth;
					this.transform.position = hit;
					*/

					if (
						((hitinfo.collider.gameObject.layer & LayerMask.NameToLayer("Card")) != 0)
					){

						// Unhighlight previously highlighted object first.
						if (_interacttarget != null && _interacttarget != hitinfo.collider.gameObject) {
							if (_highlightable != null){
								_highlightable.Unhighlight();
								_highlightable = null;
							}// fi
							_interacttarget = null;
						}// fi

						// Ensure that player can only select cards of own deck.
						CardView cv = hitinfo.collider.gameObject.GetComponent<CardView> ();
						if (cv == null && hitinfo.collider.gameObject.name.CompareTo ("Number") == 0) {
							cv = hitinfo.collider.gameObject.transform.parent.gameObject.GetComponent<CardView> ();
						}// fi
						//if (cv == null) {
						//	DbgOut.LogError ("Failed to get CardView from object=" + hitinfo.collider.gameObject.name);
						//} else if (cv.CardModel == null){
						//	DbgOut.LogError ("CardView has null CardModel; object=" + hitinfo.collider.gameObject.name);
						//}// fi

						if ((cv != null && cv.CardModel != null)
							&& ((_selectedCard == null && cv.CardModel.Position == _match.PlayerAction)
								|| (_selectedCard != null && _selectedTarget == null && cv.CardModel.Position == Card.POSITION_BOARD
									&& DoCheckCardFits(_selectedCardModel, cv)))){

							_interacttarget = hitinfo.collider.gameObject;
							CardView cv2 = _interacttarget.GetComponent<CardView> ();
							if (cv2 == null && _interacttarget.name.CompareTo ("Number") == 0)
								_interacttarget = _interacttarget.transform.parent.gameObject;

							_highlightable = _interacttarget.GetComponent<Highlightable>();
							if (_highlightable != null && _highlightable.enabled){
								interaction = true;
								_highlightable.Highlight();
							}// fi
						}// fi

					}// fi
				}// fi
				if (!interaction) {
					if (_highlightable != null) {
						_highlightable.Unhighlight ();
						_highlightable = null;
					}// fi

					_interacttarget = null;
				} else {

					if (Input.GetMouseButtonDown(0)){

						
						// Check if player selects card.
						if (_selectedCard == null) {
							_selectedCard = _interacttarget;
							CardView cvSelected = _selectedCard.GetComponent<CardView> ();
							_selectedCardModel = cvSelected.CardModel;

							DoSelectCard ();

							_camPosStashed = _camPosCurrent;
							ApplyCamPosition (CamPosBoard, TimeCamFlightBoard);

						} else if (_selectedTarget == null){
							
							_selectedTarget = _interacttarget;

							DoPlayCard ();
						}// fi
					}// fi

				}// fi

				if (Input.GetMouseButtonDown (1)) {
					if (_selectedCard != null) {
						
						DoUnselectCard ();

						_selectedCard = null;

						ApplyCamPosition (_camPosStashed, TimeCamFlightBoard);
					}// fi
				}// fi

			}// fi


			#region debug
			//if (Input.GetKeyDown(KeyCode.X) && _keys.TryPress(KeyCode.X)) {
			//	EndTurn();
			//}// fi
			#endregion debug
		}// LateUpdate



		protected void CreateInfrastructure() {

			Vector3 offsetBoard = Vector3.zero;
			offsetBoard.x = -0.5f * (CardOnBoardOffset.x * (ParamBoardColumns-1));
			offsetBoard.z = -0.5f * (CardOnBoardOffset.z * (ParamBoardRows-1));
			_cardViewsBoard = new CardView[ParamBoardRows][];
			for (int row = 0; row < ParamBoardRows; row++){
				_cardViewsBoard [row] = new CardView[ParamBoardColumns];
				for (int col = 0; col < ParamBoardColumns; col++) {
					CardView cv = SpawnCard (
						(CardColor)System.Enum.ToObject (typeof(CardColor), MathHelper.Rand.Next(System.Enum.GetNames (typeof(CardColor)).Length-1)+1),
						MathHelper.Rand.Next(ParamCardsMaxValue - ParamCardsMinValue) +1 +ParamCardsMinValue
						);
					cv.transform.position =
						ParentBoard.transform.position 
						+ offsetBoard
						+ new Vector3(col * CardOnBoardOffset.x, 0.0f, row * CardOnBoardOffset.z);
					cv.transform.rotation = Quaternion.Euler (CardOnBoardRotation);
					cv.transform.SetParent(ParentBoard.transform, true);

					cv.gameObject.name = "CardBoard_" + col + "_" + row;

					_cardViewsBoard [row] [col] = cv;
					_cardViewsBoard [row] [col].Column = col;
					_cardViewsBoard [row] [col].Row = row;
				}// for
			}// for

			_cardViewsPerPlayer = new List<CardView>[ParamPlayerCount];
			for (int p = 0; p < ParamPlayerCount; p++) {
				_cardViewsPerPlayer[p] = new List<CardView>();
				for (int pc = 0; pc < ParamCardsPerPlayer; pc++) {

					AddCardView(p,
						(CardColor)System.Enum.ToObject (typeof(CardColor), MathHelper.Rand.Next(System.Enum.GetNames (typeof(CardColor)).Length-1)+1),
						MathHelper.Rand.Next(ParamCardsMaxValue - ParamCardsMinValue) +1 +ParamCardsMinValue);
					/*
					CardView cv = SpawnCard (
						(CardColor)System.Enum.ToObject (typeof(CardColor), MathHelper.Rand.Next(System.Enum.GetNames (typeof(CardColor)).Length-1)+1),
						MathHelper.Rand.Next(ParamCardsMaxValue - ParamCardsMinValue) +1 +ParamCardsMinValue
					);

					int row = Mathf.FloorToInt (((float)pc) / ((float)CardsMaxPerRow));
					cv.transform.position =
						//ParentPlayerHands[p].transform.position +
						new Vector3((pc % CardsMaxPerRow)* CardPlayerHandOffset.x, CardPlayerHandOffset.y, row * CardPlayerHandOffset.z);
					cv.transform.rotation = Quaternion.Euler (CardPlayerHandRotation);// + ParentPlayerHands[p].transform.rotation.eulerAngles);
					cv.transform.SetParent(ParentPlayerHands[p].transform, false);

					cv.gameObject.name = "CardPlayer_" + p + "_" + pc;

					_cardViewsPerPlayer [p].Add(cv);
					*/
				}// for
			}// for

			//AudioHelper.AssignSfx(this.gameObject, ref _asApplause, SfxApplause);
		}// CreateInfrastructure



		protected CardView AddCardView(int player, Card card){
			CardView cv = AddCardView (player, card.Color, card.Value);
			cv.CardModel = card;
			cv.CardModel.Position = player;
			return cv;
		}// AddCard



		protected CardView AddCardView(int player, CardColor col, int value){
			CardView cv = null;

			// Try to find a free one.
			for (int i = 0; cv == null && i < _cardViewsPerPlayer [player].Count; i++) {
				if (!_cardViewsPerPlayer [player] [i].gameObject.activeSelf)
					cv = _cardViewsPerPlayer [player] [i];
			}// for

			if (cv != null) {
				cv.SetCardColor (col);
				cv.SetCardNumber (value);
				cv.gameObject.SetActive (true);
			} else {
				cv = SpawnCard (
					col,
					value
				);

				int count = _cardViewsPerPlayer [player].Count;
				int row = Mathf.FloorToInt (((float)count) / ((float)CardsMaxPerRow));
				cv.transform.position =
					//ParentPlayerHands[p].transform.position +
					new Vector3 ((count % CardsMaxPerRow) * CardPlayerHandOffset.x, CardPlayerHandOffset.y, row * CardPlayerHandOffset.z);
				cv.transform.rotation = Quaternion.Euler (CardPlayerHandRotation);// + ParentPlayerHands[p].transform.rotation.eulerAngles);
				cv.transform.SetParent (ParentPlayerHands [player].transform, false);

				cv.gameObject.name = "CardPlayer_" + player + "_" + count;

				_cardViewsPerPlayer [player].Add (cv);
			}// fi

			return cv;
		}// AddCardView



		protected void CreateRoutines(){
			int id = 0;
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on yellow or red cards.", ValidateRoutine_YellowRed));
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on yellow or blue cards.", ValidateRoutine_YellowBlue));
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on yellow or green cards.", ValidateRoutine_YellowGreen));
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on red or green cards.", ValidateRoutine_RedGreen));
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on red or blue cards.", ValidateRoutine_RedBlue));
			_match.AddNewRoutine (new Routine (id++, "You can only place your cards on green or blue cards.", ValidateRoutine_GreenBlue));

			_match.AddNewRoutine (new Routine (id++, "You can only place an even card on an even card and an uneven on an uneven card.", ValidateRoutine_SameOddEven));
			_match.AddNewRoutine (new Routine (id++, "You can only place an even card on an uneven card and an uneven on an even card.", ValidateRoutine_MixedOddEven));

			_match.AddNewRoutine (new Routine (id++, "You need to place your card besides the last played card.", ValidateRoutine_BesidesLast));
			_match.AddNewRoutine (new Routine (id++, "You are not allowed to place your card besides the last played card.", ValidateRoutine_NotBesidesLast));

		}// CreateRoutines



		#region match management
		public void BeginMatch(){

			DbgOut.Log ("Beginning new match.");

			if (_matchRunning)
				AbortMatch ();

			List<CardColor> forbiddenColors = new List<CardColor> ();
			forbiddenColors.Add (CardColor.None);
			if (ParamCardColorForbiddenRed)
				forbiddenColors.Add (CardColor.Red);
			if (ParamCardColorForbiddenGreen)
				forbiddenColors.Add (CardColor.Green);
			if (ParamCardColorForbiddenBlue)
				forbiddenColors.Add (CardColor.Blue);
			if (ParamCardColorForbiddenYellow)
				forbiddenColors.Add (CardColor.Yellow);

			_match = new Match(
				ParamPlayerCount, // playerCount
				ParamCardsPerPlayer, // cardsPerPlayer
				ParamBoardRows, // boardrows
				ParamBoardColumns, // boardcolumns
				ParamCardsMinValue, // cardsMinValue
				ParamCardsMaxValue, // cardsMaxValue
				ParamCardsSetsPerColor, // cardsSetsPerColor
				forbiddenColors // forbiddenCardColors
			);

			CreateRoutines ();

			FillBoard ();

			DrawPlayerHands ();

			// The very first played card is the center one.
			_lastBoardColumn = 1;
			_lastBoardRow = 1;

			_matchRunning = true;

			//PrepareTurn ();

			// Prepare UI.
			_mainOptionsVisible = true;
			OnUIMainOptions ();
			BtnMainOptionsVisible.SetActive (true);
			_guessRoutinePlayerVisible = true;
			OnUITryGuessRoutine ();
			SetVisiblePlayerControls (true);
			InputWall.SetActive (false);

			SetUIStatusText ("It's your turn, player " + (_match.PlayerAction+1) + ". Select a card!");

			_startNextRound = true;

		}// BeginMatch


		// Prepares a player's turn.
		protected void PrepareTurn(){
			DbgOut.Log ("Now it's the turn of player=" + (_match.PlayerAction+1));

			_camPosCurrent = CamPosPlayers [_match.PlayerAction];
			ApplyCamPosition (_camPosCurrent, TimeCamFlightNextPlayer);

			_selectedCard = null;
			_selectedTarget = null;

			_match.PlayerMustDraw = !CheckPlayerCanPlay ();
			_match.PlayerDidGuess = false;

			// Setup the UI.
			HideAdditionalUI ();
			SetVisiblePlayerControls (true);
		}// PrepareTurn



		// Commits / ends a player's turn.
		protected void EndTurn(){

			int lastplayer = _match.PlayerAction;

			// Determine the next player.
			int idxNextPlayer = _match.PlayerAction + 1;
			if (idxNextPlayer >= _match.PlayerCount)
				idxNextPlayer = 0;
			_match.PlayerAction = idxNextPlayer;

			SetUIStatusText ("Player " + (lastplayer+1) + " placed a " + _lastPlayedCard.Color
				+ " " + _lastPlayedCard.Value + " on a " + _lastBoardCard.Color + " " + _lastBoardCard.Value + " (at " + (_lastBoardColumn+1) + "/" + (_lastBoardRow+1) + ")"
				+ ". Now it's your turn, player " + (_match.PlayerAction+1) + ". Select a card!");

			//PrepareTurn ();
			_startNextRound = true;

			// TODO
		}// EndTurn



		protected void EndMatch(){
			_matchRunning = false;

			SetUIStatusText ("The match is over. The winner is player " + (_match.Winner+1) + "!");
			// TODO
		}// EndMatch



		protected void AbortMatch(){
			_matchRunning = false;
			_startNextRound = false;

			for (int p=0; p < _match.PlayerCount; p++){
				Player player = _match.GetPlayer (p);
				foreach (Card card in player.Cards) {
					_match.PlayDeck.ReturnCard (card);
				}// foreach
				player.Cards.Clear();
			}// for

			_match.ReturnAllRoutines ();

			// TODO
		}// AbortMatch


		#endregion match management




		#region view management

		protected CardView SpawnCard(CardColor color, int value){

			GameObject card = GameObject.Instantiate (PrefabCard);
			CardView view = card.GetComponent<CardView> ();
			view.SetCardColor (color);
			view.SetCardNumber (value);

			return view;
		}// SpawnCard



		protected CardView SpawnCard(Card cardModel){

			GameObject card = GameObject.Instantiate (PrefabCard);
			CardView view = card.GetComponent<CardView> ();
			view.CardModel = cardModel;
		
			return view;
		}// SpawnCard



		protected void FillBoard(){
			for (int row = 0; row < _match.PlayBoard.Rows; row++){
				for (int col = 0; col < _match.PlayBoard.Columns; col++) {
					Card card = _match.PlayDeck.DrawCard ();
					_cardViewsBoard [row] [col].CardModel = card;
					card.Position = Card.POSITION_BOARD;
					//DbgOut.Log ("Board["+row+"]["+col+"]=" + card.ToString());
				}// for
			}// for
		}// FillBoard



		protected void DrawPlayerHands(){
			for (int p = 0; p < ParamPlayerCount; p++) {

				// Note: the initial cards were actually drawn when the player object got created.
				for (int pc = 0; pc < _match.GetPlayer (p).Cards.Count; pc++) {
					Card card = _match.GetPlayer (p).Cards [pc]; //_match.PlayDeck.DrawCard ();
					_cardViewsPerPlayer [p] [pc].CardModel = card;
					card.Position = p;

					_cardViewsPerPlayer [p] [pc].gameObject.SetActive (true);
					//DbgOut.Log ("Player=" + p + " card[" + pc + "]=" + card.ToString());
				}// for

				// Also draw a Routine for the player.
				_match.GetPlayer(p).Routine = _match.DrawRoutine();
				DbgOut.Log ("Rule of player=" + (p+1) + ": " + _match.GetPlayer(p).Routine.Text);

			}// for
		}// DrawPlayerHands



		protected void ApplyCamPosition(GameObject camPosition, float time){
			_camFlightTimeLeft = time;
			_camFlightStartPos = Camera.main.transform.position;
			_camFlightStartRot = Camera.main.transform.rotation.eulerAngles;
			_camFlightTargetPos = camPosition.transform.position;
			_camFlightTargetRot = camPosition.transform.rotation.eulerAngles;
			_camFlightTimeTotal = time;
			_camFlightTimeLeft = time;

			_camFlightActive = true;

			_camPosCurrent = camPosition;
		}// ApplyCamPosition



		protected void DoCameraFlight(){
			if (!_camFlightActive)
				return;

			_camFlightTimeLeft -= Time.deltaTime;
			if (_camFlightTimeLeft <= 0.0f) {
				_camFlightTimeLeft = 0.0f;
				_camFlightActive = false;
			}// fi
			float share = (_camFlightTimeTotal - _camFlightTimeLeft) / _camFlightTimeTotal;

			Camera.main.transform.position = Vector3.Lerp(_camFlightStartPos, _camFlightTargetPos, share);
			Camera.main.transform.rotation = Quaternion.Euler( Vector3.Lerp(_camFlightStartRot, _camFlightTargetRot, share));
		}// DoCameraFlight


		protected void ApplyPlayerHandPresentation(){
			// TODO
		}// ApplyPlayerHandPresentation



		#region UI

		public void OnUIMainOptions(){
			_mainOptionsVisible = !_mainOptionsVisible;
			GameLogo.SetActive(_mainOptionsVisible);
			BtnInstructions.SetActive(_mainOptionsVisible);
			BtnBeginNewMatch.SetActive(_mainOptionsVisible);
			BtnAbout.SetActive(_mainOptionsVisible);
			TxtLargeText.gameObject.SetActive(false);
		}// OnUIMainOptions



		protected void SetVisiblePlayerControls(bool state){
			
			BtnChangeRoutine.SetActive(state
				&& _match != null && !_match.GetPlayer(_match.PlayerAction).DidChangeRoutine
				);

			BtnGuessRoutine.SetActive(state && !_match.PlayerDidGuess);
			BtnMyRoutine.SetActive(state);
			BtnAllRoutines.SetActive(state);
			TxtStatus.gameObject.SetActive(state);

			BtnDrawCard.SetActive(state && _match != null && _match.PlayerMustDraw);
			TxtInstructDrawCard.gameObject.SetActive(state && _match != null && _match.PlayerMustDraw);
			if (state && _match!=null && _match.PlayerMustDraw)
				InputWall.SetActive (true);

			TxtInstructGuessRoutine.gameObject.SetActive(state && _guessRoutinePlayerVisible);
			BtnGuess1.SetActive(state && _guessRoutinePlayerVisible);
			BtnGuess2.SetActive(state && _guessRoutinePlayerVisible);

			BtnGuess3.SetActive(state && _guessRoutinePlayerVisible);

			TxtSingleRoutine.gameObject.SetActive(state && _guessRoutinePlayerVisible);

		}// SetVisiblePlayerControls



		public void OnUIDrawCard(){
			AddCardView (_match.PlayerAction, _match.PlayDeck.DrawCard ());
		}// OnUIDrawCard



		public void OnUIChangeRoutine(){
			if (!_match.GetPlayer (_match.PlayerAction).DidChangeRoutine) {

				Routine newRoutine = _match.DrawRoutine ();
				_match.ReturnRoutine (_match.GetPlayer (_match.PlayerAction).Routine);
				_match.GetPlayer (_match.PlayerAction).Routine = newRoutine;

				_match.GetPlayer (_match.PlayerAction).DidChangeRoutine = true;

				BtnChangeRoutine.SetActive (false);

				SetUIStatusText ("Your Routine has been changed.");

				_match.PlayerMustDraw = !CheckPlayerCanPlay ();

				SetVisiblePlayerControls (true);

			}// fi
		}// OnPlayerChangeRoutine



		public void OnUITryGuessRoutine(){
			bool guessRoutinePlayerVisible = _guessRoutinePlayerVisible;
			HideAdditionalUI ();

			_guessRoutinePlayerVisible = !guessRoutinePlayerVisible;
			_guessRoutineSelectVisible = false;

			InputWall.SetActive (_guessRoutinePlayerVisible);

			TxtInstructGuessRoutine.gameObject.SetActive(_guessRoutinePlayerVisible);
			BtnGuess1.SetActive(_guessRoutinePlayerVisible);
			BtnGuess2.SetActive(_guessRoutinePlayerVisible);
			BtnGuess3.SetActive(_guessRoutinePlayerVisible);
			if (_guessRoutinePlayerVisible) {
				int player = 0;
				UIExtHasText text = BtnGuess1.GetComponent<UIExtHasText> ();
				if (player == _match.PlayerAction) player++;
				text.Text.text = "" + (player+1);
				player++;
				text = BtnGuess2.GetComponent<UIExtHasText> ();
				if (player == _match.PlayerAction) player++;
				text.Text.text = "" + (player+1);
				player++;
				text = BtnGuess3.GetComponent<UIExtHasText> ();
				if (player == _match.PlayerAction) player++;
				text.Text.text = "" + (player+1);
			}// fi

			TxtSingleRoutine.gameObject.SetActive(false);
			BtnSelectRoutine.SetActive(false);
			BtnPrevRoutine.SetActive(false);
			BtnNextRoutine.SetActive(false);

		}// OnUITryGuessRoutine



		public void OnUITryGuessRoutine(int option){

			_guessRoutineSelectVisible = true;
			 
			_guessPlayerId = -1;
			UIExtHasText text = null;
			switch (option) {
			case 1:
				text = BtnGuess1.GetComponent<UIExtHasText> ();
				break;
			case 2:
				text = BtnGuess2.GetComponent<UIExtHasText> ();
				break;
			default:
				text = BtnGuess3.GetComponent<UIExtHasText> ();
				break;
			}// switch
			_guessPlayerId = int.Parse (text.Text.text) - 1;
			_guessRoutine = _match.GetPlayer (_guessPlayerId).Routine;

			_displayedRoutine = 0;

			TxtSingleRoutine.Text.text = _match.AllRoutines[_displayedRoutine].Text;
			TxtSingleRoutine.gameObject.SetActive(true);

			BtnSelectRoutine.SetActive(true);
			BtnPrevRoutine.SetActive(true);
			BtnNextRoutine.SetActive(true);
		}// OnUITryGuessRoutine(int)



		public void OnUINextRoutine(){
			_displayedRoutine++;
			if (_displayedRoutine >= _match.AllRoutines.Count) {
				_displayedRoutine = 0;
			}// fi
			TxtSingleRoutine.Text.text = _match.AllRoutines[_displayedRoutine].Text;
		}// OnUINextRoutine



		public void OnUIPrevRoutine(){
			_displayedRoutine--;
			if (_displayedRoutine <= 0) {
				_displayedRoutine = _match.AllRoutines.Count - 1;
			}// fi
			TxtSingleRoutine.Text.text = _match.AllRoutines[_displayedRoutine].Text;
		}// OnUIPrevRoutine



		public void OnUISelectRoutine(){

			_match.PlayerDidGuess = true;

			// Hide guessing UI.
			OnUITryGuessRoutine ();

			if (_match.AllRoutines[_displayedRoutine].Id == _guessRoutine.Id) {
				AddCardView(_guessPlayerId, _match.PlayDeck.DrawCard());

				SetUIStatusText ("You guessed correct! Player " + (_guessPlayerId+1) + " had to draw another card. (Routine of player " + (_guessPlayerId+1) + " changed.)");
			} else {
				AddCardView (_match.PlayerAction, _match.PlayDeck.DrawCard ());

				SetUIStatusText ("You guessed wrong! You had to draw another card. (Routine of player " + (_guessPlayerId+1) + " changed.)");
			}// fi

			// The Routine of the guessed player will change in any outcome.
			Routine newRoutine = _match.DrawRoutine ();
			_match.ReturnRoutine (_match.GetPlayer (_guessPlayerId).Routine);
			_match.GetPlayer (_guessPlayerId).Routine = newRoutine;

			// Reset view to have guess button vanish.
			SetVisiblePlayerControls (true);
		}// OnUISelectRoutine



		public void OnUIListRoutines(){
			bool showAllRoutines = _showAllRoutines;

			HideAdditionalUI ();

			_showAllRoutines = !showAllRoutines;

			InputWall.SetActive (_showAllRoutines);

			_displayedRoutine = 0;

			TxtSingleRoutine.Text.text = _match.AllRoutines[_displayedRoutine].Text;
			TxtSingleRoutine.gameObject.SetActive(_showAllRoutines);

			BtnPrevRoutine.SetActive(_showAllRoutines);
			BtnNextRoutine.SetActive(_showAllRoutines);

		}// OnUIListRoutines



		public void OnUIDisplayMyRoutine(){
			bool showMyRoutine = _showMyRoutine;

			HideAdditionalUI ();

			_showMyRoutine = !showMyRoutine;

			InputWall.SetActive (_showMyRoutine);

			// Debugging.
			/*
			if (_match.GetPlayer (_match.PlayerAction).Routine == null)
				DbgOut.LogError ("Routine of player=" + _match.PlayerAction + " is null.");
			if (TxtSingleRoutine == null)
				DbgOut.LogError ("TxtSingleRoutine is null.");
			else if (TxtSingleRoutine.Text == null)
				DbgOut.LogError ("TxtSingleRoutine.Text is null.");
			*/

			TxtSingleRoutine.Text.text = _match.GetPlayer(_match.PlayerAction).Routine.Text;
			TxtSingleRoutine.gameObject.SetActive(_showMyRoutine);

		}// OnUIListRoutines



		protected void HideAdditionalUI(){
			_guessRoutinePlayerVisible = false;
			_guessRoutineSelectVisible = false;

			_showAllRoutines = false;

			_showMyRoutine = false;

			InputWall.SetActive (false);

			TxtInstructGuessRoutine.gameObject.SetActive(false);
			BtnGuess1.SetActive(false);
			BtnGuess2.SetActive(false);
			BtnGuess3.SetActive(false);

			TxtSingleRoutine.gameObject.SetActive(false);
			BtnSelectRoutine.SetActive(false);
			BtnPrevRoutine.SetActive(false);
			BtnNextRoutine.SetActive(false);

		}// HideAdditionalUI



		protected void SetUIStatusText(string text){
			TxtStatus.Text.text = text;
		}// SetUIStatusText



		public void OnUIAbout(){
			bool hide = TxtLargeText.gameObject.activeSelf;
			HideAdditionalUI ();
			TxtLargeText.gameObject.SetActive (!hide);
			if (!hide) {
				TxtLargeText.Text.text = "<size=25>(C) 2016 Markus Wellmann and Harry Trautmann\n\n"
					+ "Lead Game Design: Markus Wellmann\n\n"
					+ "Additional Game Design and Digital Simulation: Harry Trautmann (http://metacozm.com)\n\n"
					+ "Special Thanks to Chantal, Sebastian and Anselm for helping in the initial phase!!!\n\n"
					+ "This game was developed during the Global Game Jam 2016 in Stuttgart/Germany.\n"
					+ "Find more info here: http://globalgamejam.org/2016/games/routines</size>";
			}// fi
		}// OnUIAbout



		public void OnUIInstructions(){
			bool hide = TxtLargeText.gameObject.activeSelf;
			HideAdditionalUI ();
			TxtLargeText.gameObject.SetActive (!hide);
			if (!hide) {
				TxtLargeText.Text.text = "<size=25>"
					+ "<b>Routines is a card game about guessing the routine of your opponent and trying to get rid of your own cards"
					+ "in a way so nobody guesses yours.</b>\n\n"
					+ "Material:\n"
					+ "1) Playing cards with 4 colors and numbers reaching from 1-10."
					+ "There are two of each card. All in all there are 80 Cards.\n"
					+ "2) 10 special Routines cards (see Routines Cards below).\n\n"
					+ "Setup:\n"
					+ "1) Every player gets nine random playing cards and one random routine card.\n"
					+ "2) A grid of 3 x 3 playing cards is placed on the table.\n\n"
					+ "Main Rules:\n"
					+ "1) Each turn a player is allowed to lay down one card on another card of the grid. "
					+ "You can only place a card on another if the number is higher or the same. "
					+ "There is an exception, if your card has the same color the number does not matter.\n"
					+ "2) Additionally to these main rules each routine card adds another rule that only applies to the player who holds it. "
					+ "The other players do not know which routine card it is.\n"
					+ "3) If a player can’t play a card she has to draw a card from the pile. But the drawn card can instantly be played it if it fits.\n"
					+ "4) Once each turn a player can try to guess the routine of another player. "
					+ "If the guess is correct then the player with the guessed routine has to draw another card. "
					+ "If the guess is wrong, the guesser himself has to draw another card. "
					+ "In any case the player whose routine card got guessed, will then draw a new routine card. The old one gets put back into the pile of routine cards.\n"
					+ "5) Every player can change his routine card once in the game, on his turn before playing a card. "
					+ "The discarded routine card is then put back into the pile of routine cards.\n\n"
					+ "Start of the Game:\n"
					+ "1) Initially the centered card on the table is considered the 'last card played'."
					+ "2) The player turns go in a clockwise manner.\n\n"
					+ "End of the game:\n"
					+ "   The player who first get rid of all his cards wins.\n\n"
					+ "List of all routine cards:\n"
					+ "1. You can only place your cards on yellow or red cards.\n"
					+ "2. You can only place your cards on yellow or blue cards.\n"
					+ "3. You can only place your cards on yellow or green cards.\n"
					+ "4. You can only place your cards on red or green cards.\n"
					+ "5. You can only place your cards on red or blue cards.\n"
					+ "6. You can only place your cards on green or blue cards.\n"
					+ "7. You can only place an even card on an even card and an uneven card on an uneven card.\n"
					+ "8. You can only place an even card on an uneven card and an uneven card on an even card.\n"
					+ "9. You need to place your card besides the last played card."
					+ "(Examples: 'The player before you placed his card in the very center of the grid. Then you can place your card everywhere except in the very center.' "
					+ "'The player places his card top left. You now can place your card on only 3 places around the top left.')\n"
					+ "10. You are not allowed to place your card besides the last played card. (Note: This is the exact opposite of routine 9.)"
					+ "</size>";
			}// fi
		}// OnUIInstructions

		#endregion UI

		#endregion view management



		#region rules

		protected bool CheckPlayerCanPlay(){
			foreach (Card card in _match.GetPlayer(_match.PlayerAction).Cards) {
				for (int col = 0; col < ParamBoardColumns; col++) {
					for (int row = 0; row < ParamBoardRows; row++) {
						if (DoCheckCardFits(card, _cardViewsBoard[row][col]))
							return true;
					}// for
				}// for
			}// foreach
			return false;
		}// CheckPlayerCanPlay



		protected bool DoCheckCardFits(Card selected, CardView cv){
			return
				ValidateRoutine_Common(selected, cv.CardModel, cv.Column, cv.Row)
				&& (_match.GetPlayer(_match.PlayerAction).Routine.Validator(selected, cv.CardModel, cv.Column, cv.Row))
				;
		}// DoCheckCardFits



		protected void DoSelectCard(){
			CardView cvSelect = _selectedCard.GetComponent<CardView> ();
			CardView cvCursor = CardCursor.GetComponent<CardView> ();

			cvCursor.SetCardColor (cvSelect.CardModel.Color);
			cvCursor.SetCardNumber (cvSelect.CardModel.Value);

			CardCursor.SetActive (true);
			_selectedCard.SetActive (false);

			SetVisiblePlayerControls (false);
			//TxtStatus.gameObject.SetActive (true);
		}// DoSelectCard

		protected void DoUnselectCard(){
			CardCursor.SetActive (false);
			if (_selectedCard != null)
				_selectedCard.SetActive (true);

			SetVisiblePlayerControls (true);
		}// DoUnselectCard

		protected void DoPlayCard(){

			DbgOut.Log ("Playing card.");

			CardView cvplayed = _selectedCard.GetComponent<CardView>();
			CardView cvtarget = _selectedTarget.GetComponent<CardView>();

			// Hide the temporary selection view.
			DoUnselectCard ();

			// Remember the last played card.
			_lastPlayedCard = cvplayed.CardModel;
			_lastBoardCard = cvtarget.CardModel;
			_lastBoardColumn = cvtarget.Column;
			_lastBoardRow = cvtarget.Row;

			// Return card from board to reserve stack.
			DbgOut.Log ("Returning card to reserve; card=" + cvtarget.CardModel.ToString ());
			_match.PlayDeck.ReturnCard (cvtarget.CardModel);

			// Place it on the board.
			cvtarget.CardModel = cvplayed.CardModel;
			cvtarget.CardModel.Position = Card.POSITION_BOARD;
			DbgOut.Log ("Placing card on board; removing from player hand; card=" + cvplayed.CardModel.ToString ());

			// Remove card from player hand.
			Player player = _match.GetPlayer (_match.PlayerAction);
			player.Cards.Remove (cvplayed.CardModel);
			cvplayed.CardModel = null;
			cvplayed.gameObject.SetActive (false);
			//Renderer rend = cvplayed.gameObject.GetComponent<Renderer> ();
			//rend.enabled = false;

			if (CheckMatchEnd ()) {
				EndMatch ();
			} else {
				EndTurn ();
			}// fi
		}// DoPlayCard



		// Set winning condition.
		protected bool CheckMatchEnd(){
			for (int i = 0; i < _match.PlayerCount; i++) {
				Player player = _match.GetPlayer (i);
				if (player.Cards.Count == 0) {
					_match.Winner = i;

					DbgOut.Log ("Detected match end. Winner is player=" + i);

					return true;
				}// fi
			}// for
			return false;
		}// CheckMatchEnd



		#region Routine validators

		protected bool ValidateRoutine_Common(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(played.Color)==0)
				|| (onboard.Color.CompareTo(played.Color)!=0 && (onboard.Value <= played.Value));
		}// ValidateRoutine_Common

		protected bool ValidateRoutine_YellowRed(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Yellow)==0)
				|| (onboard.Color.CompareTo(CardColor.Red)==0);
		}// ValidateRoutine_YellowRed

		protected bool ValidateRoutine_YellowBlue(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Yellow)==0)
				|| (onboard.Color.CompareTo(CardColor.Blue)==0);
		}// ValidateRoutine_YellowBlue

		protected bool ValidateRoutine_YellowGreen(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Yellow)==0)
				|| (onboard.Color.CompareTo(CardColor.Green)==0);
		}// ValidateRoutine_YellowGreen

		protected bool ValidateRoutine_RedGreen(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Red)==0)
				|| (onboard.Color.CompareTo(CardColor.Green)==0);
		}// ValidateRoutine_RedGreen

		protected bool ValidateRoutine_RedBlue(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Red)==0)
				|| (onboard.Color.CompareTo(CardColor.Blue)==0);
		}// ValidateRoutine_RedBlue

		protected bool ValidateRoutine_GreenBlue(Card played, Card onboard, int column, int row){
			return (onboard.Color.CompareTo(CardColor.Green)==0)
				|| (onboard.Color.CompareTo(CardColor.Blue)==0);
		}// ValidateRoutine_GreenBlue

		protected bool ValidateRoutine_SameOddEven(Card played, Card onboard, int column, int row){
			return (((played.Value + onboard.Value) % 2) == 0);
		}// ValidateRoutine_SameOddEven

		protected bool ValidateRoutine_MixedOddEven(Card played, Card onboard, int column, int row){
			return !ValidateRoutine_SameOddEven(played, onboard, column, row);
		}// ValidateRoutine_MixedOddEven

		protected bool ValidateRoutine_BesidesLast(Card played, Card onboard, int column, int row){
			int diffX = Mathf.Abs (_lastBoardColumn - column);
			int diffY = Mathf.Abs (_lastBoardRow - row);
			return (
				(diffX==1 && diffY<2)
				|| (diffX==0 && diffY == 1)
			);
		}// ValidateRoutine_BesidesLast

		protected bool ValidateRoutine_NotBesidesLast(Card played, Card onboard, int column, int row){
			return !ValidateRoutine_BesidesLast (played, onboard, column, row);
		}// ValidateRoutine_NotBesidesLast

		#endregion Routine validators
		#endregion rules


		#region debug

		protected void DbgOutMatchState(){
			DbgOut.Log ("Cards held:"
				+ " p0=" + _match.GetPlayer(0).Cards.Count
				+ " p1=" + _match.GetPlayer(1).Cards.Count
				+ " p2=" + _match.GetPlayer(2).Cards.Count
				+ " p3=" + _match.GetPlayer(3).Cards.Count
			);
			for (int p = 0; p < _match.PlayerCount; p++) {
				DbgOut.Log ("Player=" + p + " has hand:");
				for (int pc = 0; pc < _match.GetPlayer (p).Cards.Count; pc++)
					DbgOut.Log (_match.GetPlayer (p).Cards [pc].ToString ());
			}// for
		}// DbgOutMatchState

		#endregion debug

			

	}// class
}// namespace

