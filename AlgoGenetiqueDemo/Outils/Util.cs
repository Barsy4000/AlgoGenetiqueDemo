using System.Collections.Generic;
using System.Linq;

namespace AlgoGenetiqueDemo.Outils
{
    /// <summary>
    /// Méthodes utilitaires du projet.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Calcule toutes les permutations d'une liste.
        /// </summary>
        /// <typeparam name="T">Type d'éléments de la liste.</typeparam>
        /// <param name="list">Liste à partir de laquelle il faut calculer les permutations.</param>
        /// <param name="length">Taille de la liste.</param>
        /// <returns>Retourne l'ensemble des permutations.</returns>
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new T[] { t });
            }

            return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
