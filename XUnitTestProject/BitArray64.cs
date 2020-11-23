namespace quick_code_test
{
    public struct BitArray64
    {
        public ulong Bits;
        public bool this[int index]
        {
            get
            {
                ulong mask = 1ul << index;
                return (Bits & mask) == mask;
            }
            set
            {
                ulong mask = 1ul << index;
                if (value)
                {
                    Bits |= mask;
                }
                else
                {
                    Bits &= ~mask;
                }
            }
        }
    }
}