using System;
using System.Collections.Concurrent;
using Storage = System.Collections.Concurrent.ConcurrentDictionary<(System.Type, string), System.Lazy<object>>;

namespace Common
{
    public interface ILocator
    {
        void Register<T>(Func<T> factory) where T : class;
        void Register<T>(string name, Func<T> factory) where T : class;
        T Resolve<T>() where T : class;
        T Resolve<T>(string name) where T : class;
    }

    public class Locator : ILocator
    {
        public void Register<T>(Func<T> factory) where T : class
        {
            Register<T>(null);
        }

        public void Register<T>(string name, Func<T> factory) where T:class
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            _storage.AddOrUpdate((typeof(T), name)
                , typeInfo => new Lazy<object>(() => factory() ?? throw new InvalidOperationException($"Type in locator registered with null factory for type: '{typeof(T)}','{name}'"), true)
                , (typeInfo, oldValue) => new Lazy<object>(() => factory() ?? throw new InvalidOperationException($"Type in locator registered with null factory for type: '{typeof(T)}','{name}'"), true));
        }

        public T Resolve<T>() where T : class
        {
            return Resolve<T>(null);
        }

        public T Resolve<T>(string name) where T : class
        {
            return _storage.TryGetValue((typeof(T), name), out var result) 
                ? (result as Lazy<T>).Value
                : throw new InvalidOperationException($"Unregistered type in locator: '{typeof(T)}','{name}'");
        }

        private Storage _storage = new Storage();
    }

    public static class Static
    {
        public static ILocator Locator { get; } = new Locator();
    }
}
