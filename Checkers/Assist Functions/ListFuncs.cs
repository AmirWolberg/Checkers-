using System.Collections.Generic;


namespace Checkers
{
    /// <summary>
    /// Added List Functons Written by me for use in the project 
    /// </summary>
    /// <typeparam name="T"> Generic Type Of List </typeparam>
    public class ListFuncs<T>: List<T>
    {
        /// <summary>
        /// Clones list of type t 
        /// </summary>
        /// <param name="ToClone"> list to clone </param>
        /// <returns> returns clone of list of type t </returns>
        public static List<T> CloneList(List<T> ToClone)
        {
            if (ToClone == null)
                return new List<T>();

            List<T> Clone = new List<T>();
            foreach(T item in ToClone)
            {
                Clone.Add(item);
            }
            return Clone;
        }
    }
}
