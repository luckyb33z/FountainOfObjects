namespace Player
{
    using Room; 

    class Player
    {
        readonly int defaultArrowCount = 5;

        public Room CurrentRoom { get; set; }
        public bool Dead { get; set; } = false;
        public bool Thrown { get; set; } = false;
        private int _arrows;
        public int Arrows { get {return _arrows;} private set {_arrows = value; } }

        public Player()
        {
            Arrows = defaultArrowCount;
        }

        public Player(int arrowCount)
        {
            Arrows = arrowCount;
        }

        public bool CanShoot()
        {
            return Arrows > 0;
        }

        public void ShootArrow()
        {
            Arrows -= 1;
        }
    }
}