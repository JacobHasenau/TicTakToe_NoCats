using AutoFixture;
using AutoFixture.Kernel;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tests
{
    [TestFixture(3, 3, 3)]
    [TestFixture(5, 5, 4)]
    [TestFixture(9, 9, 5)]
    [TestFixture(17, 17, 6)]
    [TestFixture(33, 33, 7)]
    [TestFixture(3, 5, 3)]
    [TestFixture(5, 3, 3)]
    [TestFixture(5, 9, 4)]
    [TestFixture(9, 5, 4)]
    public class TicTacToeBoardTests
    {
        private uint _xSize, _ySize;
        private ushort _completionNumber;
        private TicTacToeBoard _board;

        public TicTacToeBoardTests(int ySize, int xSize, int completionNumber)
        {
            _xSize = (uint)xSize;
            _ySize = (uint)ySize;
            _completionNumber = (ushort)completionNumber;
        }

        [SetUp]
        public void CreateBoard()
        {
            _board = new TicTacToeBoard(_ySize, _xSize);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void Then_XSize_Set_Correctly()
        {
            Assert.AreEqual(_xSize, _board.XSize);
        }

        [Test]
        public void Then_YSize_Set_Correctly()
        {
            Assert.AreEqual(_ySize, _board.YSize);
        }

        [Test]
        [TestCase(0, 0, Shape.Circle)]
        [TestCase(0, 0, Shape.Cross)]
        [TestCase(2, 0, Shape.Circle)]
        [TestCase(2, 0, Shape.Cross)]
        [TestCase(0, 2, Shape.Cross)]
        [TestCase(0, 2, Shape.Circle)]
        [TestCase(1, 0, Shape.Circle)]
        [TestCase(1, 0, Shape.Cross)]
        [TestCase(0, 1, Shape.Cross)]
        [TestCase(0, 1, Shape.Circle)]
        [TestCase(1, 1, Shape.Circle)]
        [TestCase(1, 1, Shape.Cross)]
        public void And_AcceptPlayerMove_Called_On_Empty_Cell_Correctly_Sets_Shape(int posYMod, int posXMod, Shape shape)
        {
            var posY = Math.Floor((_ySize - 1) * (posYMod == 0 ? 0 : (float)1/posYMod));
            var posX = Math.Floor((_xSize - 1) * (posXMod == 0 ? 0 : (float)1/posXMod));

            var playerMove = new PlayerMove(shape, (uint)posY, (uint)posX);
            Debug.Log($"Player move {shape} at PosY: {posY}, PosX: {posX}");
            _board.AcceptPlayerMove(playerMove);

            Assert.AreEqual(shape, _board.GetShapeAtPosition((uint)posY, (uint)posX));
        }

        [Test]
        [TestCase(Shape.Circle, Shape.Circle)]
        [TestCase(Shape.Circle, Shape.Cross)]
        [TestCase(Shape.Cross, Shape.Circle)]
        [TestCase(Shape.Cross, Shape.Cross)]
        public void And_AcceptPlayerMove_Called_On_Filled_Cell_Then_Throws_Exception(Shape existingShape, Shape newShape)
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new UintMaxSetBuilder(Math.Min(_xSize, _ySize)));

            fixture.Inject(existingShape);
            var firstMove = fixture.Create<PlayerMove>();
            _board.AcceptPlayerMove(firstMove);

            var secondMove = new PlayerMove(newShape, firstMove.PosY, firstMove.PosX);
            Assert.IsFalse(_board.AcceptPlayerMove(secondMove));
        }

        [Test]
        public void And_InitializeBoard_Called_Then_Sets_Correct_Values()
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new PlayerMoveUniqueBuilder(_ySize, _xSize));
            var playerMoves = fixture.CreateMany<PlayerMove>((int)_ySize * (int)_xSize).ToArray();

            _board.InitializeBoard(playerMoves);

            var shapesAreCorrect = true;
            for(uint posY = 0; posY < _ySize; posY++)
            {
                for (uint posX = 0; posX < _xSize; posX++)
                {
                    shapesAreCorrect = shapesAreCorrect
                        && _board.GetShapeAtPosition(posY, posX)
                        == playerMoves.Single(x => x.PosX == posX && x.PosY == posY).PlayerShape;
                    if (!shapesAreCorrect)
                        break;
                }
            }

            Assert.IsTrue(shapesAreCorrect);
        }
        
        [Test]
        [TestCase(Shape.Circle, 0)]
        [TestCase(Shape.Circle, 1)]
        [TestCase(Shape.Circle, 2)]
        [TestCase(Shape.Circle, 3)]
        [TestCase(Shape.Cross, 0)]
        [TestCase(Shape.Cross, 1)]
        [TestCase(Shape.Cross, 2)]
        [TestCase(Shape.Cross, 3)]
        public void And_GetShape_Called_Then_Fetches_Correct_Value(Shape shape, int expectedShapeCount)
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new PlayerMoveUniqueBuilder(_ySize, _xSize));
            fixture.Inject(shape);
            var playerMoves = fixture.CreateMany<PlayerMove>(expectedShapeCount).ToArray();

            var nonPassedInShape = Enum.GetValues(typeof(Shape)).Cast<Shape>().First(x => x != shape);
            fixture.Inject(nonPassedInShape);
            var extraShapes = fixture.CreateMany<PlayerMove>(((int)_ySize * (int)_xSize) - expectedShapeCount).ToList();
            var combinedMoves = playerMoves.Union(extraShapes).ToList();

            _board.InitializeBoard(combinedMoves);

            Assert.AreEqual(expectedShapeCount, _board.GetShapeCount(shape));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_Up_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, (_completionNumber - (uint)1) - i, 0);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_UpRight_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, (_completionNumber - (uint)1) - i, i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_Right_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, 0, i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_DownRight_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, i, i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_Down_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, i, 0);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_DownLeft_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, i, (_completionNumber - (uint)1) - i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_Left_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, 0, (_completionNumber - (uint)1) - i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_UpLeft_Victory_Then_Returns_True(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber];

            for (uint i = 0; i < _completionNumber; i++)
            {
                playerMoves[i] = new PlayerMove(shape, (_completionNumber - (uint)1) - i, (_completionNumber - (uint)1) - i);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsTrue(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        [TestCase(Shape.Circle)]
        [TestCase(Shape.Cross)]
        public void And_PlayerWonOnMove_Called_For_Not_Victory_Then_Returns_False(Shape shape)
        {
            var playerMoves = new PlayerMove[_completionNumber - 1];
            
            for(uint i = 0; i < _completionNumber - 1; i++)
            {
                playerMoves[i] = new PlayerMove(shape, i, 0);
            }

            _board.InitializeBoard(playerMoves);

            Assert.IsFalse(_board.PlayerWonOnMove(playerMoves.First(), _completionNumber));
        }

        [Test]
        public void And_DoubleBoardSize_Called_Then_Pieces_Properly_Mapped()
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new PlayerMoveUniqueBuilder(_ySize, _xSize));
            var playerMoves = fixture.CreateMany<PlayerMove>((int)_ySize * (int)_xSize).ToArray();

            _board.InitializeBoard(playerMoves);
            _board.DoubleBoardSize();

            var shapesAreCorrect = true;
            for (uint posY = 0; posY < _ySize; posY++)
            {
                for (uint posX = 0; posX < _xSize; posX++)
                {
                    shapesAreCorrect = shapesAreCorrect
                        && _board.GetShapeAtPosition(posY * 2, posX * 2)
                        == playerMoves.Single(x => x.PosX == posX && x.PosY == posY).PlayerShape;
                    if (!shapesAreCorrect)
                        break;
                }
            }

            Assert.IsTrue(shapesAreCorrect);
        }
    }

    public class UintMaxSetBuilder : ISpecimenBuilder
    {
        private uint _maxValue;

        public UintMaxSetBuilder(uint maxValue)
        {
            _maxValue = maxValue;
            Random.InitState(DateTime.Now.GetHashCode());
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(uint))
                return (uint)Math.Floor(Random.Range(0, _maxValue));

            return new NoSpecimen();
        }
    }

    public class PlayerMoveUniqueBuilder : ISpecimenBuilder
    {
        private uint _maxXValue, _maxYValue, currXValue, currYValue;

        public PlayerMoveUniqueBuilder(uint boardYSize, uint boardXSize)
        {
            _maxYValue = boardYSize - 1;
            _maxXValue = boardXSize - 1;
            currYValue = 0;
            currXValue = 0;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(PlayerMove))
            {
                if (currXValue > _maxXValue)
                {
                    currXValue = 0;
                    currYValue++;
                }

                var playerMove =
                    new PlayerMove(context.Create<Shape>(), currYValue, currXValue++);

                return playerMove;
            }

            return new NoSpecimen();
        }
    }
}
