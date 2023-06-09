namespace Player
{
    using Room; 
    
    class Player
    {
        public Room CurrentRoom { get; set; }
        public bool Dead { get; set; } = false;
        public bool Thrown { get; set; } = false;
    }
}