using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwych.Aston;

public static class EnumerableExtensions
{
    public static TSource Second<TSource>(this IEnumerable<TSource> self) => self.Skip(1).First();

    public static TSource SecondOrDefault<TSource>(this IEnumerable<TSource> self) => self.Skip(1).FirstOrDefault();

    public static TSource Third<TSource>(this IEnumerable<TSource> self) => self.Skip(2).First();

    public static TSource ThirdOrDefault<TSource>(this IEnumerable<TSource> self) => self.Skip(2).FirstOrDefault();

    public static TSource Fouth<TSource>(this IEnumerable<TSource> self) => self.Skip(3).First();

    public static TSource FouthOrDefault<TSource>(this IEnumerable<TSource> self) => self.Skip(3).FirstOrDefault();
}
