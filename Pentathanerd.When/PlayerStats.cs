namespace Pentathanerd.When
{
    internal class PlayerStats
    {
        public string ConnectionId { get; set; }

        public int Hits { get; set; }

        public int Misses { get; set; }

        public int KeysPressedThisTurn { get; set; }

        public ScreenLocation ScreenLocation { get; set; }
    }
}