namespace ApiClientLib
{
    /// <summary>
    /// A lightweight wrapper to pass reference types by value,
    /// enabling lazy initialization and shared references across scopes.
    /// </summary>
    internal sealed class Ref<T> where T : class
    {
        public T Value { get; set; }

        public Ref()
        {
        }

        public Ref(T initialValue)
        {
            Value = initialValue;
        }
    }
}