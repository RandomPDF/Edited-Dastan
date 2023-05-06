//Skeleton Program code for the AQA A Level Paper 1 Summer 2023 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

namespace Dastan
{
	class Program
	{
		static void Main(string[] args)
		{
			Dastan ThisGame = new Dastan(6, 6, 4);
			ThisGame.PlayGame();
			Console.WriteLine("Goodbye!");
			Console.ReadLine();
		}
	}

	class Dastan
	{
		protected List<Square> Board;
		protected int NoOfRows, NoOfColumns, MoveOptionOfferPosition;
		protected List<Player> Players = new List<Player>();
		protected List<string> MoveOptionOffer = new List<string>();
		protected Player CurrentPlayer;
		protected Random RGen = new Random();

		public Dastan(int R, int C, int NoOfPieces)
		{
			CreateCustomPlayers();
			CreateMoveOptions();
			NoOfRows = R;
			NoOfColumns = C;
			MoveOptionOfferPosition = 0;
			CreateMoveOptionOffer();
			CreateBoard();
			CreatePieces(NoOfPieces);
			CurrentPlayer = Players[0];
		}

		private void CreateCustomPlayers()
		{
			Console.WriteLine("What is the name of player 1?");
			string name1 = Console.ReadLine();
			Players.Add(new Player(name1, 1));

			string name2;
			do
			{
				Console.WriteLine("What is the name of player 2?");
				name2 = Console.ReadLine();
			}
			while (name1 == name2);

			Players.Add(new Player(name2, -1));
		}

		private void CalculateSahmMove(int StartSquareReference)
		{
			bool inBounds = true;
			int CurrentSquareReference = StartSquareReference;
			do
			{
				CurrentSquareReference += 10 * CurrentPlayer.GetDirection();

				if (CheckSquareInBounds(CurrentSquareReference) &&
					!Board[GetIndexOfSquare(CurrentSquareReference)].ContainsKotla())
				{
					UpdatePlayerScore(CalculatePieceCapturePoints(CurrentSquareReference));
					Board[GetIndexOfSquare(CurrentSquareReference)].RemovePiece();
				}
				else inBounds = false;
			}
			while (inBounds);
		}

		private void DisplayBoard()
		{
			Console.Write(Environment.NewLine + "   ");
			for (int Column = 1; Column <= NoOfColumns; Column++) Console.Write(Column.ToString() + "  ");
			Console.Write(Environment.NewLine + "  ");
			for (int Count = 1; Count <= NoOfColumns; Count++) Console.Write("---");
			Console.WriteLine("-");
			for (int Row = 1; Row <= NoOfRows; Row++)
			{
				Console.Write(Row.ToString() + " ");
				for (int Column = 1; Column <= NoOfColumns; Column++)
				{
					int Index = GetIndexOfSquare(Row * 10 + Column);
					Console.Write("|" + Board[Index].GetSymbol());
					Piece PieceInSquare = Board[Index].GetPieceInSquare();
					if (PieceInSquare == null)
					{
						Console.Write(" ");
					}
					else
					{
						Console.Write(PieceInSquare.GetSymbol());
					}
				}
				Console.WriteLine("|");
			}
			Console.Write("  -");
			for (int Column = 1; Column <= NoOfColumns; Column++) Console.Write("---");
			Console.WriteLine();
			Console.WriteLine();
		}

		private void DisplayState()
		{
			DisplayBoard();
			Console.WriteLine("You have " + CurrentPlayer.GetChoiceOptionsLeft() + " more offers to grab.");

			if (CurrentPlayer.GetChoiceOptionsLeft() > 0)
				Console.WriteLine("Move option offer: " + MoveOptionOffer[MoveOptionOfferPosition]);

			Console.WriteLine();
			Console.WriteLine(CurrentPlayer.GetPlayerStateAsString());
			Console.WriteLine("Turn: " + CurrentPlayer.GetName());
			Console.WriteLine();
		}

		private int GetValidInt()
		{
			int output = int.MinValue;
			do
			{
				if (!int.TryParse(Console.ReadLine(), out output))
				{
					Console.WriteLine("The input is an invalid data type. Should be an integer.");
				}
			}
			while (output == int.MinValue);

			return output;
		}

		private int GetIndexOfSquare(int SquareReference)
		{
			int Row = SquareReference / 10;
			int Col = SquareReference % 10;
			return (Row - 1) * NoOfColumns + (Col - 1);
		}

		private bool CheckSquareInBounds(int SquareReference)
		{
			int Row = SquareReference / 10;
			int Col = SquareReference % 10;
			if (Row < 1 || Row > NoOfRows) return false;
			else if (Col < 1 || Col > NoOfColumns) return false;
			else return true;
		}

		private bool CheckSquareIsValid(int SquareReference, bool StartSquare)
		{
			if (!CheckSquareInBounds(SquareReference))
			{
				Console.WriteLine("The input square isn't in bounds.");
				return false;
			}

			Piece PieceInSquare = Board[GetIndexOfSquare(SquareReference)].GetPieceInSquare();
			if (PieceInSquare == null)
			{
				if (StartSquare) return false;
				return true;
			}
			else if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()))
			{
				if (StartSquare) return true;
				return false;
			}
			else
			{
				if (StartSquare) return false;
				return true;
			}
		}

		private bool CheckIfGameOver()
		{
			bool Player1HasMirza = false;
			bool Player2HasMirza = false;
			foreach (var S in Board)
			{
				Piece PieceInSquare = S.GetPieceInSquare();
				if (PieceInSquare != null)
				{
					if (S.ContainsKotla() && PieceInSquare.GetTypeOfPiece() == "mirza" && !PieceInSquare.GetBelongsTo().SameAs(S.GetBelongsTo()))
					{
						return true;
					}
					else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[0]))
					{
						Player1HasMirza = true;
					}
					else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[1]))
					{
						Player2HasMirza = true;
					}
				}
			}
			return !(Player1HasMirza && Player2HasMirza);
		}

		private int GetSquareReference(string Description)
		{
			int SelectedSquare;

			Console.Write("Enter the square " + Description + " (row number followed by column number): ");
			SelectedSquare = GetValidInt();
			return SelectedSquare;
		}

		private void UseMoveOptionOffer()
		{
			int ReplaceChoice;
			int input = int.MinValue;

			do
			{
				Console.Write("Choose the move option from your queue to replace (1 to 7): ");
				input = GetValidInt();
				if (input < 1 || input > 7) Console.WriteLine("This move option isn't between 1 and 7.");
			}
			while (input < 1 || input > 7);

			ReplaceChoice = input;
			CurrentPlayer.UpdateMoveOptionQueueWithOffer(ReplaceChoice - 1, CreateMoveOption(MoveOptionOffer[MoveOptionOfferPosition], CurrentPlayer.GetDirection()));
			CurrentPlayer.ChangeScore(-(10 - (ReplaceChoice * 2)));
			MoveOptionOfferPosition = RGen.Next(0, 5);
			CurrentPlayer.DecreaseChoiceOptionsLeft();
		}

		private void UseSpyOption()
		{
			foreach (Player player in Players)
			{
				//find opponent player
				if (!player.SameAs(CurrentPlayer))
				{
					Console.WriteLine("Here is the opponents move option queue: " + player.GetJustQueueAsString());
					CurrentPlayer.ChangeScore(-5);
				}
			}
		}

		private void SacraficePiece()
		{
			bool SquareIsValid = false;
			int SacraficeReference = 0;

			do
			{
				SacraficeReference = GetSquareReference("containing the piece to sacrafice");
				SquareIsValid = CheckSquareIsValid(SacraficeReference, true);
			}
			while (!SquareIsValid);

			if (Board[GetIndexOfSquare(SacraficeReference)].ContainsKotla()) return;

			if (CurrentPlayer.SameAs(Players[0])) Board[GetIndexOfSquare(SacraficeReference)] = new Kotla(CurrentPlayer, "K");
			else if (CurrentPlayer.SameAs(Players[1])) Board[GetIndexOfSquare(SacraficeReference)] = new Kotla(CurrentPlayer, "k");
		}

		private void ModifyQueueOptions()
		{
			Console.WriteLine("You can:");
			Console.WriteLine("1. Reverse the current player queue");
			Console.WriteLine("2. Swap the current players queue with the opponents queue");
			Console.WriteLine("3. Swap the first and last elements in the current players queue");
			Console.WriteLine("4. Move one of the move options to the front of the current player queue");
			Console.WriteLine("5. Nothing (make normal move)\n");
			Console.WriteLine("What would you like to do?");

			int Choice = -1;
			do
			{
				Choice = GetValidInt();
			}
			while (Choice < 0 || Choice > 6);

			if (Choice != 5) CurrentPlayer.ChangeScore(-3);

			switch (Choice)
			{
				case 1:
					CurrentPlayer.ReversePlayerQueue();
					break;

				case 2:
					Player opponent;
					if (CurrentPlayer.SameAs(Players[0])) opponent = Players[1];
					else opponent = Players[2];

					CurrentPlayer.ReplaceQueue(opponent.GetMoveOptionQueue());
					break;

				case 3:
					CurrentPlayer.SwapFirstAndLast();
					break;

				case 4:
					Console.Write("What is the item you want to move to the front of the queue: ");
					CurrentPlayer.MoveItemToFront(GetValidInt());
					break;
			}

			Console.WriteLine($"The current queue is: {CurrentPlayer.GetJustQueueAsString()}");
		}

		private int GetPointsForOccupancyByPlayer(Player CurrentPlayer)
		{
			int ScoreAdjustment = 0;
			foreach (var S in Board) ScoreAdjustment += (S.GetPointsForOccupancy(CurrentPlayer));
			return ScoreAdjustment;
		}

		private void UpdatePlayerScore(int PointsForPieceCapture)
		{
			CurrentPlayer.ChangeScore(GetPointsForOccupancyByPlayer(CurrentPlayer) + PointsForPieceCapture);
		}

		private int CalculatePieceCapturePoints(int FinishSquareReference)
		{
			if (Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare() != null)
			{
				return Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare().GetPointsIfCaptured();
			}
			return 0;
		}

		private bool AwardWafr()
		{
			//generate random number 0 to 3
			if (RGen.Next(0, 4) == 0) return true;
			return false;
		}

		public void PlayGame()
		{
			bool GameOver = false;
			while (!GameOver)
			{
				DisplayState();

				bool SquareIsValid = false;
				int Choice;

				bool useWafr = !CurrentPlayer.GetWafrAwarded() && AwardWafr();
				if (useWafr)
				{
					Console.WriteLine("You have been awarded a Wafr, you can select any move from your queue for free this turn");

					do
					{
						Console.Write("Choose any move option to use from queue: ");
						Choice = GetValidInt();
					}
					while (Choice > 7);

				}
				else
				{
					do
					{
						if (CurrentPlayer.GetChoiceOptionsLeft() > 0)
						{
							Console.Write("Choose move option to use from queue (1 to 3) or 8 to spy on the opponents queue 9 to take the offer or 10 to sacrifice a piece or 11 to change the queue: ");
						}
						else Console.Write("Choose move option to use from queue (1 to 3) or 8 to spy on the opponents queue 10 to sacrifice a piece or 11 to change the queue: ");

						Choice = GetValidInt();

						if (Choice == 8)
						{
							UseSpyOption();
							DisplayState();
						}
						else if (Choice == 9 && CurrentPlayer.GetChoiceOptionsLeft() > 0)
						{
							UseMoveOptionOffer();
							DisplayState();
						}
						else if (Choice == 10)
						{
							SacraficePiece();
						}
						else if (Choice == 11)
						{
							ModifyQueueOptions();
						}
					}
					while ((Choice < 1 || Choice > 3) && Choice != 10);
				}

				if (CurrentPlayer.ChoiceIsSahm(Choice))
				{
					int _StartSquareReference = 0;
					while (!SquareIsValid)
					{
						_StartSquareReference = GetSquareReference("to start from.");
						SquareIsValid = CheckSquareIsValid(_StartSquareReference, true);
					}

					CalculateSahmMove(_StartSquareReference);
					CurrentPlayer.SetSahmUsed();
				}
				else if (Choice != 10)
				{
					int StartSquareReference = 0;
					while (!SquareIsValid)
					{
						StartSquareReference = GetSquareReference("containing the piece to move");
						SquareIsValid = CheckSquareIsValid(StartSquareReference, true);
					}

					int FinishSquareReference = 0;
					SquareIsValid = false;
					while (!SquareIsValid)
					{
						FinishSquareReference = GetSquareReference("to move to");
						SquareIsValid = CheckSquareIsValid(FinishSquareReference, false);
					}
					int previousScore = -1;
					bool MoveLegal = CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, FinishSquareReference);
					if (MoveLegal)
					{
						previousScore = CurrentPlayer.GetScore();

						int PointsForPieceCapture = CalculatePieceCapturePoints(FinishSquareReference);
						if (!useWafr) CurrentPlayer.ChangeScore(-(Choice + (2 * (Choice - 1))));
						else CurrentPlayer.SetWafrAwarded();
						CurrentPlayer.UpdateQueueAfterMove(Choice);
						UpdateBoard(StartSquareReference, FinishSquareReference);
						UpdatePlayerScore(PointsForPieceCapture);
						Console.WriteLine("New score: " + CurrentPlayer.GetScore() + Environment.NewLine);

						DisplayBoard();
						Console.Write("Would you like to undo this move (y/n): ");

						if (Console.ReadLine() == "y")
						{
							CurrentPlayer.ChangeScore(previousScore - CurrentPlayer.GetScore() - 5);
							CurrentPlayer.ResetQueueBackAfterUndo(Choice);
							UpdateBoard(FinishSquareReference, StartSquareReference);

							if (CurrentPlayer.SameAs(Players[0])) CurrentPlayer = Players[1];
							else CurrentPlayer = Players[0];
						}
					}
				}

				if (CurrentPlayer.SameAs(Players[0])) CurrentPlayer = Players[1];
				else CurrentPlayer = Players[0];

				GameOver = CheckIfGameOver();
			}

			DisplayState();
			DisplayFinalResult();
		}

		private void UpdateBoard(int StartSquareReference, int FinishSquareReference)
		{
			Board[GetIndexOfSquare(FinishSquareReference)].SetPiece(Board[GetIndexOfSquare(StartSquareReference)].RemovePiece());
		}

		private void DisplayFinalResult()
		{
			if (Players[0].GetScore() == Players[1].GetScore()) Console.WriteLine("Draw!");
			else if (Players[0].GetScore() > Players[1].GetScore())
			{
				Console.WriteLine(Players[0].GetName() + " is the winner!");
			}
			else
			{
				Console.WriteLine(Players[1].GetName() + " is the winner!");
			}
		}

		private void CreateBoard()
		{
			Square S;
			Board = new List<Square>();
			for (int Row = 1; Row <= NoOfRows; Row++)
			{
				for (int Column = 1; Column <= NoOfColumns; Column++)
				{
					if (Row == 1 && Column == NoOfColumns / 2) S = new Kotla(Players[0], "K");
					else if (Row == NoOfRows && Column == NoOfColumns / 2 + 1) S = new Kotla(Players[1], "k");
					else S = new Square();
					Board.Add(S);
				}
			}
		}

		private void CreatePieces(int NoOfPieces)
		{
			Piece CurrentPiece;
			for (int Count = 1; Count <= NoOfPieces; Count++)
			{
				CurrentPiece = new Piece("piece", Players[0], 1, "!");
				Board[GetIndexOfSquare(2 * 10 + Count + 1)].SetPiece(CurrentPiece);
			}
			CurrentPiece = new Piece("mirza", Players[0], 5, "1");
			Board[GetIndexOfSquare(10 + NoOfColumns / 2)].SetPiece(CurrentPiece);
			for (int Count = 1; Count <= NoOfPieces; Count++)
			{
				CurrentPiece = new Piece("piece", Players[1], 1, "\"");
				Board[GetIndexOfSquare((NoOfRows - 1) * 10 + Count + 1)].SetPiece(CurrentPiece);
			}
			CurrentPiece = new Piece("mirza", Players[1], 5, "2");
			Board[GetIndexOfSquare(NoOfRows * 10 + (NoOfColumns / 2 + 1))].SetPiece(CurrentPiece);
		}

		private void CreateMoveOptionOffer()
		{
			MoveOptionOffer.Add("sahm");
			MoveOptionOffer.Add("jazair");
			MoveOptionOffer.Add("chowkidar");
			MoveOptionOffer.Add("cuirassier");
			MoveOptionOffer.Add("ryott");
			MoveOptionOffer.Add("faujdar");
			MoveOptionOffer.Add("faris");
			MoveOptionOffer.Add("sarukh");
		}

		private MoveOption CreateSahmMoveOption()
		{
			MoveOption NewMoveOption = new MoveOption("sahm");
			Move NewMove = new Move(0, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateSarukhMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("sarukh");
			Move NewMove = new Move(2 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateFarisMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("faris");
			Move NewMove = new Move(1 * Direction, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(2 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-2 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(2 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-2 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateRyottMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("ryott");
			Move NewMove = new Move(0, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateFaujdarMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("faujdar");
			Move NewMove = new Move(0, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateJazairMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("jazair");
			Move NewMove = new Move(2 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(2 * Direction, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(2 * Direction, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateCuirassierMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("cuirassier");
			Move NewMove = new Move(1 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(2 * Direction, 0);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateChowkidarMoveOption(int Direction)
		{
			MoveOption NewMoveOption = new MoveOption("chowkidar");
			Move NewMove = new Move(1 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(1 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, 1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(-1 * Direction, -1 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, 2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			NewMove = new Move(0, -2 * Direction);
			NewMoveOption.AddToPossibleMoves(NewMove);
			return NewMoveOption;
		}

		private MoveOption CreateMoveOption(string Name, int Direction)
		{
			if (Name == "chowkidar") return CreateChowkidarMoveOption(Direction);
			else if (Name == "ryott") return CreateRyottMoveOption(Direction);
			else if (Name == "faujdar") return CreateFaujdarMoveOption(Direction);
			else if (Name == "jazair") return CreateJazairMoveOption(Direction);
			else if (Name == "faris") return CreateFarisMoveOption(Direction);
			else if (Name == "sarukh") return CreateSarukhMoveOption(Direction);
			else if (Name == "sahm") return CreateSahmMoveOption();

			return CreateCuirassierMoveOption(Direction);
		}

		private void CreateMoveOptions()
		{
			Players[0].AddToMoveOptionQueue(CreateMoveOption("ryott", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("chowkidar", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("sarukh", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("faris", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("cuirassier", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("faujdar", 1));
			Players[0].AddToMoveOptionQueue(CreateMoveOption("jazair", 1));

			Players[1].AddToMoveOptionQueue(CreateMoveOption("ryott", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("chowkidar", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("sarukh", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("faris", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("jazair", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("faujdar", -1));
			Players[1].AddToMoveOptionQueue(CreateMoveOption("cuirassier", -1));
		}
	}

	class Piece
	{
		protected string TypeOfPiece, Symbol;
		protected int PointsIfCaptured;
		protected Player BelongsTo;

		public Piece(string T, Player B, int P, string S)
		{
			TypeOfPiece = T;
			BelongsTo = B;
			PointsIfCaptured = P;
			Symbol = S;
		}

		public string GetSymbol() { return Symbol; }

		public string GetTypeOfPiece() { return TypeOfPiece; }

		public Player GetBelongsTo() { return BelongsTo; }

		public int GetPointsIfCaptured() { return PointsIfCaptured; }
	}

	class Square
	{
		protected string Symbol;
		protected Piece PieceInSquare;
		protected Player BelongsTo;

		public Square()
		{
			PieceInSquare = null;
			BelongsTo = null;
			Symbol = " ";
		}

		public virtual void SetPiece(Piece P) { PieceInSquare = P; }

		public virtual Piece RemovePiece()
		{
			Piece PieceToReturn = PieceInSquare;
			PieceInSquare = null;
			return PieceToReturn;
		}

		public virtual Piece GetPieceInSquare() { return PieceInSquare; }

		public virtual string GetSymbol() { return Symbol; }

		public virtual int GetPointsForOccupancy(Player CurrentPlayer) { return 0; }

		public virtual Player GetBelongsTo() { return BelongsTo; }

		public virtual bool ContainsKotla()
		{
			if (Symbol == "K" || Symbol == "k") return true;
			return false;
		}
	}

	class Kotla : Square
	{
		public Kotla(Player P, string S) : base()
		{
			BelongsTo = P;
			Symbol = S;
		}

		public override int GetPointsForOccupancy(Player CurrentPlayer)
		{
			if (PieceInSquare == null) return 0;
			else if (BelongsTo.SameAs(CurrentPlayer))
			{
				if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
				{
					return 5;
				}

				return 0;
			}
			else
			{
				if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
				{
					return 1;
				}

				return 0;
			}
		}
	}

	class MoveOption
	{
		protected string Name;
		protected List<Move> PossibleMoves;

		public MoveOption(string N)
		{
			Name = N;
			PossibleMoves = new List<Move>();
		}

		public void AddToPossibleMoves(Move M) { PossibleMoves.Add(M); }

		public string GetName() { return Name; }

		public bool CheckIfThereIsAMoveToSquare(int StartSquareReference, int FinishSquareReference)
		{
			int StartRow = StartSquareReference / 10;
			int StartColumn = StartSquareReference % 10;
			int FinishRow = FinishSquareReference / 10;
			int FinishColumn = FinishSquareReference % 10;
			foreach (var M in PossibleMoves)
			{
				if (StartRow + M.GetRowChange() == FinishRow && StartColumn + M.GetColumnChange() == FinishColumn)
				{
					return true;
				}
			}
			return false;
		}
	}

	class Move
	{
		protected int RowChange, ColumnChange;

		public Move(int R, int C)
		{
			RowChange = R;
			ColumnChange = C;
		}

		public int GetRowChange() { return RowChange; }

		public int GetColumnChange() { return ColumnChange; }
	}

	class MoveOptionQueue
	{
		private List<MoveOption> Queue = new List<MoveOption>();

		public string GetQueueAsString()
		{
			string QueueAsString = "";
			int Count = 1;
			foreach (var M in Queue)
			{
				QueueAsString += Count.ToString() + ". " + M.GetName() + "   ";
				Count += 1;
			}
			return QueueAsString;
		}

		public void Add(MoveOption NewMoveOption) { Queue.Add(NewMoveOption); }

		public void Replace(int Position, MoveOption NewMoveOption) { Queue[Position] = NewMoveOption; }

		public void MoveItemToBack(int Position)
		{
			MoveOption Temp = Queue[Position];
			Queue.RemoveAt(Position);
			Queue.Add(Temp);
		}

		public MoveOption GetMoveOptionInPosition(int Pos) { return Queue[Pos]; }

		public int GetQueueSize() { return Queue.Count; }

		public void ResetQueueBack(int Position)
		{
			MoveOption Temp = Queue[Queue.Count - 1];
			Queue.RemoveAt(Queue.Count - 1);
			Queue.Insert(Position - 1, Temp);
		}

		public void ReverseQueue() { Queue.Reverse(); }

		public void SwapFirstAndLast()
		{
			MoveOption Temp = Queue[Queue.Count - 1];
			Replace(Queue.Count - 1, Queue[0]);
			Replace(0, Temp);
		}

		public void MoveItemToFront(int position)
		{
			Queue.Insert(0, Queue[position - 1]);
			Queue.RemoveAt(position);
		}
	}

	class Player
	{
		private string Name;
		private int Direction, Score;
		private bool WafrAwarded;
		private bool SahmUsed = false;
		private int ChoiceOptionsLeft = 3;
		private MoveOptionQueue Queue = new MoveOptionQueue();

		public Player(string N, int D)
		{
			Score = 100;
			Name = N;
			Direction = D;
		}

		public bool SameAs(Player APlayer)
		{
			if (APlayer == null) return false;
			else if (APlayer.GetName() == Name) return true;
			return false;
		}

		public string GetPlayerStateAsString()
		{
			return Name + Environment.NewLine + "Score: " + Score.ToString() + Environment.NewLine + "Move option queue: " + Queue.GetQueueAsString() + Environment.NewLine;
		}

		public void AddToMoveOptionQueue(MoveOption NewMoveOption) { Queue.Add(NewMoveOption); }

		public void UpdateQueueAfterMove(int Position) { Queue.MoveItemToBack(Position - 1); }

		public string GetJustQueueAsString() { return Queue.GetQueueAsString(); }

		public void UpdateMoveOptionQueueWithOffer(int Position, MoveOption NewMoveOption)
		{
			Queue.Replace(Position, NewMoveOption);
		}

		public void ResetQueueBackAfterUndo(int Position) { Queue.ResetQueueBack(Position); }

		public int GetScore() { return Score; }

		public string GetName() { return Name; }

		public int GetDirection() { return Direction; }

		public bool GetWafrAwarded() { return WafrAwarded; }

		public bool GetSahmStatus() { return SahmUsed; }

		public int GetChoiceOptionsLeft() { return ChoiceOptionsLeft; }

		public void DecreaseChoiceOptionsLeft() { ChoiceOptionsLeft--; }

		public void SetWafrAwarded() { WafrAwarded = true; }

		public void SetSahmUsed() { SahmUsed = true; }

		public void ChangeScore(int Amount) { Score += Amount; }

		public bool CheckPlayerMove(int Pos, int StartSquareReference, int FinishSquareReference)
		{
			MoveOption Temp = Queue.GetMoveOptionInPosition(Pos - 1);
			return Temp.CheckIfThereIsAMoveToSquare(StartSquareReference, FinishSquareReference);
		}

		public bool ChoiceIsSahm(int Choice)
		{
			if (Choice <= Queue.GetQueueSize() && Choice > 0 &&
				Queue.GetMoveOptionInPosition(Choice - 1).GetName() == "sahm") return true;

			return false;
		}

		public void ReversePlayerQueue() { Queue.ReverseQueue(); }

		public void SwapFirstAndLast() { Queue.SwapFirstAndLast(); }

		public void MoveItemToFront(int position) { Queue.MoveItemToFront(position); }

		public void ReplaceQueue(MoveOptionQueue queue) { Queue = queue; }

		public MoveOptionQueue GetMoveOptionQueue() { return Queue; }
	}
}