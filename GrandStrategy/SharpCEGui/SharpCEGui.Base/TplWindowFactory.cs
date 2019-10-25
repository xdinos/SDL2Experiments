using System;
using System.Linq.Expressions;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Template based WindowFactory that can be used to automatically generate a
    /// WindowFactory given a Window based class.
    /// 
    /// The advantage of this over the previous macro based methods is that there is
    /// no longer any need to have any supporting code or structure in order to add
    /// new Window types to the system, rather you can just do:
    /// \code
    /// CEGUI::WindowFactoryManager::addFactory<TplWindowFactory<MyWidget> >();
    /// \endcode
    /// </summary>
    /// <typeparam name="T">
    /// Specifies the Window based class that the factory will create and
    /// destroy instances of.
    /// </typeparam>
    public class TplWindowFactory<T> : WindowFactory where T : Window
    {
        public TplWindowFactory()
            : base((string)typeof(T).GetField("WidgetTypeName").GetValue(null))
        {
            var objectConstructor = typeof (T).GetConstructor(new[] {typeof (string), typeof (string)});
            if (objectConstructor==null)
                throw new InvalidOperationException("Constructor not found.");

            var parameterType = Expression.Parameter(typeof(string), "type");
            var parameterName = Expression.Parameter(typeof(string), "name");
            
            _creator = Expression.Lambda<Func<string, string, Window>>(
                Expression.Convert(
                    Expression.New(objectConstructor, parameterType, parameterName),
                    typeof (Window)),
                parameterType, parameterName)
                .Compile();
        }

        public override Window CreateWindow(string name)
        {
            return _creator(GetTypeName(), name);
        }

        public override void DestroyWindow(Window window)
        {
            window.Dispose();
        }

        private readonly Func<string, string, Window> _creator;
    }
}