using System;
using System.Collections.Generic;
using System.Text;

namespace FTPServer.Domain.Extensions
{
    public static class ArrayExtend
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
