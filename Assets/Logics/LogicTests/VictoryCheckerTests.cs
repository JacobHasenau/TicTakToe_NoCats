using NUnit.Framework;
using System.Linq;

namespace Assets.Logics.LogicTests
{
    [TestFixture]
    public class Given_A_VictoryChecker
    {
        private ushort _completionNumber;
        private Shape _victoryShape;
        private TicTacToeBoard _board;
        private VictoryChecker _victoryChecker;

        public Given_A_VictoryChecker(int completionNumber, Shape victoryShape)
        {
            _completionNumber = (ushort)completionNumber;
            _victoryShape = victoryShape;
        }

        [SetUp]
        public void CreateVictoryChecker()
        {
            _board = new TicTacToeBoard(_completionNumber + (uint)1, _completionNumber + (uint)1);
            _victoryChecker = new VictoryChecker(_board);
        }

        [TestFixture(3, Shape.Cross)]
        [TestFixture(3, Shape.Circle)]
        [TestFixture(4, Shape.Cross)]
        [TestFixture(4, Shape.Circle)]
        [TestFixture(5, Shape.Cross)]
        [TestFixture(5, Shape.Circle)]
        [TestFixture(10, Shape.Cross)]
        [TestFixture(10, Shape.Circle)]
        public class And_PlayerWonOnMove_Called : Given_A_VictoryChecker
        {
            public And_PlayerWonOnMove_Called(int completionNumber, Shape victoryShape) : base(completionNumber, victoryShape) {}

            [Test]
            public void And_PlayerWonOnMove_Called_For_Up_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, (_completionNumber - (uint)1) - i, 0);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_UpRight_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, (_completionNumber - (uint)1) - i, i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_Right_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, 0, i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_DownRight_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, i, i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_Down_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, i, 0);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_DownLeft_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, i, (_completionNumber - (uint)1) - i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_Left_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, 0, (_completionNumber - (uint)1) - i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_UpLeft_Victory_Then_Returns_True()
            {
                var playerMoves = new PlayerMove[_completionNumber];

                for (uint i = 0; i < _completionNumber; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, (_completionNumber - (uint)1) - i, (_completionNumber - (uint)1) - i);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsTrue(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }

            [Test]
            public void And_PlayerWonOnMove_Called_For_Not_Victory_Then_Returns_False()
            {
                var playerMoves = new PlayerMove[_completionNumber - 1];

                for (uint i = 0; i < _completionNumber - 1; i++)
                {
                    playerMoves[i] = new PlayerMove(_victoryShape, i, 0);
                }

                _board.InitializeBoard(playerMoves);

                Assert.IsFalse(_victoryChecker.PlayerWonOnMove(playerMoves.First(), _completionNumber).VictoryAchived);
            }
        }
    }
}
