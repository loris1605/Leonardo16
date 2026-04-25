namespace SysNet
{
    public class Create<T> where T : class
    {

        public static readonly Func<T> Instance =
            System.Linq.Expressions.Expression.
                        Lambda<Func<T>>(System.Linq.Expressions.Expression.New(typeof(T))).Compile();

    }
}
