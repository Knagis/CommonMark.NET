namespace CommonMark
{
#if v2_0 || v3_5
    class Lazy<T>
    {
        private readonly Func<T> valueFactory;
        private readonly object _lock = new object();
        private T value;

        public Lazy(Func<T> valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public T Value
        {
            get
            {
                if (value == null)
                {
                    lock (_lock)
                    {
                        if (value == null)
                        {
                            value = valueFactory();
                        }
                    }
                }
                return value;
            }
        }
    }
#endif
}
