using AutoFixture;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class Given_A_TurnManager
    {
        private ushort _completionNumber;
        private TurnManager _manager;
        private TicTacToeBoard _board;

        public Given_A_TurnManager(int completionNumber)
        {
            _completionNumber = (ushort)completionNumber;
        }

        [SetUp]
        public void CreateManager()
        {
            _board = new TicTacToeBoard(_completionNumber + (uint)1, _completionNumber + (uint)1);
            _manager = new TurnManager(_board, _completionNumber, Enum.GetValues(typeof(Shape)).Cast<Shape>().ToList());
        }

        [TestFixture(3)]
        [TestFixture(4)]
        [TestFixture(5)]
        [TestFixture(10)]
        private class And_MovesLeftOnBoard_Called : Given_A_TurnManager
        {
            private IFixture _fixture;

            public And_MovesLeftOnBoard_Called(int completionNumber) : base(completionNumber) { }

            [SetUp]
            public void CreateFixture()
            {
                _fixture = new Fixture();
                _fixture.Customizations.Add(new PlayerMoveUniqueBuilder(_completionNumber + (uint)1, _completionNumber + (uint)1));
            }

            [Test]
            public void And_Board_Is_Full_Then_Returns_False()
            {
                _board.InitializeBoard(_fixture.CreateMany<PlayerMove>((int)(_completionNumber + (uint)1) * (int)(_completionNumber + (uint)1)));

                Assert.IsFalse(_manager.MovesLeftOnBoard());
            }

            [Test]
            public void And_Board_Is_Not_Full_Then_Returns_True()
            {
                _board.InitializeBoard(_fixture.CreateMany<PlayerMove>((((_completionNumber + 1) * (_completionNumber + 1)) - 1)));

                Assert.IsTrue(_manager.MovesLeftOnBoard());
            }
        }

        [TestFixture(3, Shape.Cross)]
        [TestFixture(3, Shape.Circle)]
        [TestFixture(4, Shape.Cross)]
        [TestFixture(4, Shape.Circle)]
        [TestFixture(5, Shape.Cross)]
        [TestFixture(5, Shape.Circle)]
        [TestFixture(10, Shape.Cross)]
        [TestFixture(10, Shape.Circle)]
        private class And_PlayerMakesTurn_Called : Given_A_TurnManager
        {
            private Shape _moveShape;
            private IFixture _fixture;
            
            public And_PlayerMakesTurn_Called(int completionNumber, Shape moveShape) : base(completionNumber)
            {
                _moveShape = moveShape;
            }

            [SetUp]
            public void CreateFixture()
            {
                _fixture = new Fixture();
                _fixture.Customizations.Add(new PlayerMoveUniqueBuilder(_completionNumber + (uint)1, _completionNumber + (uint)1));
            }

            [Test]
            public void And_Valid_PlayerMove_Does_Not_MaxOut_Board_And_Does_Not_Win_Then_Board_Accepts_Move()
            {
                _fixture.Inject(_moveShape);
                var initalMoves = _fixture.CreateMany<PlayerMove>(_completionNumber - 2);
                var newMove = _fixture.Create<PlayerMove>();

                _board.InitializeBoard(initalMoves);

                _manager.PlayerMakesTurn(newMove);
                Assert.AreEqual(newMove.PlayerShape, _board.GetShapeAtPosition(newMove.PosY, newMove.PosX));
            }

            [Test]
            public void And_Invalid_PlayerMove_Then_Throws_Error()
            {
                _fixture.Inject(_moveShape);
                var initalMoves = _fixture.CreateMany<PlayerMove>(_completionNumber - 2);
                var newMove = initalMoves.Last();

                _board.InitializeBoard(initalMoves);

                Assert.Throws<Exception>(() => _manager.PlayerMakesTurn(newMove));
            }

            //TODO: Make win condition not a thrown error
            [Test]
            public void And_Valid_PlayerMove_Wins_Then_Fires_Win_Event()
            {
                _fixture.Inject(_moveShape);
                var initalMoves = _fixture.CreateMany<PlayerMove>(_completionNumber - 1);
                var newMove = _fixture.Create<PlayerMove>();

                _board.InitializeBoard(initalMoves);
                
                var wasCalled = false;
                _manager.SubscribeToOnPlayerWin((sender, winningInfo) => wasCalled = true);
                _manager.PlayerMakesTurn(newMove);

                Assert.IsTrue(wasCalled);
            }

            [Test]
            public void And_Valid_PlayerMove_MaxsOut_Board_And_Does_Not_Win_Then_Board_Doubles_In_Size()
            {
                var expectedBoardSize = ((_completionNumber + 1) * 2) - 1;
                _fixture.Inject(Enum.GetValues(typeof(Shape)).Cast<Shape>().First(shape => shape != _moveShape));
                var initalMoves = _fixture.CreateMany<PlayerMove>((((_completionNumber + 1) * (_completionNumber + 1)) - 1));
                _fixture.Inject(_moveShape);
                var newMove = _fixture.Create<PlayerMove>();

                _board.InitializeBoard(initalMoves);

                _manager.PlayerMakesTurn(newMove);
                Assert.IsTrue(_board.YSize == expectedBoardSize && _board.XSize == expectedBoardSize);
            }

            [Test]
            public void And_Valid_PlayerMove_Does_Not_MaxOut_Board_And_Does_Not_Win_Then_Board_Does_Not_Double_In_Size()
            {
                var expectedBoardSize = (_completionNumber + 1);
                _fixture.Inject(Enum.GetValues(typeof(Shape)).Cast<Shape>().First(shape => shape != _moveShape));
                var initalMoves = _fixture.CreateMany<PlayerMove>((((_completionNumber + 1) * (_completionNumber + 1)) - 2));
                _fixture.Inject(_moveShape);
                var newMove = _fixture.Create<PlayerMove>();

                _board.InitializeBoard(initalMoves);

                _manager.PlayerMakesTurn(newMove);
                Assert.IsTrue(_board.YSize == expectedBoardSize && _board.XSize == expectedBoardSize);
            }
        }
    }
}
