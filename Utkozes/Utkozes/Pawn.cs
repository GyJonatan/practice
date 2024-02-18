namespace Utkozes
{
    internal class Pawn
    {
        public Position Position { get; set; }
        public Direction Direction { get; set; }
        public bool CanMove { get; set; } = true;
        public Pawn(Position position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }


        public void Move()
        {            
            switch (Direction)
            {
                case Direction.Fel: Position.X--; break;
                case Direction.Le: Position.X++; break;
                case Direction.Jobb: Position.Y++; break;
                case Direction.Bal: Position.Y--; break;
            }
        }

        private Pawn GetCopy()
        {
            // ha nem hozunk létre új Position példányt, hanem ennek az osztálynak a Positon tulajdonságát
            // passzoljuk tovább az új Pawn-nak, akkor igazából csak egy referenciát hozunk rá létre,
            // így ha a kópiánk mozog, az igazi bábunk is mozog.. 
            Position position = new Position()
            {
                X = Position.X,
                Y = Position.Y,
            };
            return new Pawn(position, Direction);
        }

        public Position NexPosition()
        {
            Pawn pawn = GetCopy();
            pawn.Move();
            return pawn.Position;
        }
    }
}
