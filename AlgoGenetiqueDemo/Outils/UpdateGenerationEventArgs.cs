using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoGenetiqueDemo.Outils
{
    /// <summary>
    /// Arguments lors de l'événement de mise à jour du nombre de génération.
    /// </summary>
    public class UpdateGenerationEventArgs : EventArgs
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="UpdateGenerationEventArgs"/>.
        /// </summary>
        /// <param name="nombreGenerations">Nombre de générations à passer en argument.</param>
        public UpdateGenerationEventArgs(int nombreGenerations)
        {
            this.NombreGenerations = nombreGenerations;
        }

        /// <summary>
        /// Obtient ou définit le nombre de générations courant.
        /// </summary>
        public int NombreGenerations { get; set; }
    }
}
