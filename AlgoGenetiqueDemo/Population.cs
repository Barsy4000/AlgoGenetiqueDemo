﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AlgoGenetiqueDemo
{
    /// <summary>
    /// Classe permettant de représenter une population d'individus.
    /// </summary>
    internal class Population
    {
        /// <summary>
        /// Individus constituant la population.
        /// </summary>
        private Individu[] individus;

        /// <summary>
        /// Liste des gènes disponibles.
        /// </summary>
        private IEnumerable<Point> genes;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Population"/>.
        /// </summary>
        /// <param name="point0">Point de départ et d'arrivée du problème.</param>
        /// <param name="genes">Liste des gènes que peut contenir un individu.</param>
        /// <param name="taille">Nombre d'individus dans la population.</param>
        public Population(Point point0, IEnumerable<Point> genes, int taille)
        {
            this.Rand = new Random();

            this.Point0 = point0;
            this.genes = genes;
            this.individus = new Individu[taille];
        }

        /// <summary>
        /// Obtient le générateur de nombre aléatoire.
        /// </summary>
        public Random Rand { get; }

        /// <summary>
        /// Obtient le point de départ et d'arrivée du problème.
        /// </summary>
        public Point Point0 { get; }

        /// <summary>
        /// Obtient le meilleur individu de la population.
        /// </summary>
        public Individu MeilleurIndividu => this.individus.OrderBy(individus => individus.Valeur).First();

        /// <summary>
        /// Génère aléatoirement les individus de la population.
        /// </summary>
        public void GenerationAleatoire()
        {
            for (int i = 0; i < this.individus.Length; i++)
            {
                Individu individu = new Individu(this.Point0, this.genes.OrderBy(gene => this.Rand.Next()));
                this.individus[i] = individu;
            }
        }

        /// <summary>
        /// Sélectionne des individus pour former des couples par la méthode du tournoi.
        /// </summary>
        /// <param name="nombreCombattants">Nombre d'individus tirés au sort pour le tournoi.</param>
        /// <returns>Retourne des couples d'individus.</returns>
        public IEnumerable<Individu>[] SelectionTournoi(int nombreCombattants)
        {
            IEnumerable<Individu>[] couples = new IEnumerable<Individu>[this.individus.Length];

            for (int i = 0; i < this.individus.Length; i++)
            {
                List<Individu> combattants = new List<Individu>();

                for (int j = 0; j < nombreCombattants; j++)
                {
                    // Tire des combattants au sort.
                    combattants.Add(this.individus[this.Rand.Next(this.individus.Length)]);
                }

                // Garde les deux meilleurs pour former un couple.
                IEnumerable<Individu> gagnants = combattants.OrderBy(individu => individu.Valeur).Take(2).OrderBy(individu => this.Rand.Next());

                couples[i] = gagnants;
            }

            return couples;
        }

        /// <summary>
        /// Sélectionne des individus pour former des couples par la méthode aléatoire pondérée par valeur.
        /// </summary>
        /// <returns>Retourne des couples d'individus.</returns>
        public IEnumerable<Individu>[] SelectionPonderee()
        {
            IEnumerable<Individu>[] couples = new IEnumerable<Individu>[this.individus.Length];

            double sommeValeurs = this.individus.Sum(individu => individu.Valeur);
            double sommeInverses = this.individus.Sum(individu => sommeValeurs / individu.Valeur);

            for (int i = 0; i < this.individus.Length; i++)
            {
                Individu parent1 = null;
                Individu parent2 = null;

                double rand1 = this.Rand.NextDouble() * sommeInverses;
                double rand2 = this.Rand.NextDouble() * sommeInverses;
                double temp = 0;
                int index = 0;

                while (parent1 == null || parent2 == null)
                {
                    temp += sommeValeurs / this.individus[index].Valeur;

                    if (parent1 == null && temp > rand1)
                    {
                        parent1 = this.individus[index];
                    }

                    if (parent2 == null && temp > rand2)
                    {
                        parent2 = this.individus[index];
                    }

                    index++;
                }

                couples[i] = new[] { parent1, parent2 };
            }

            return couples;
        }

        /// <summary>
        /// Sélectionne des individus pour former des couples par la méthode aléatoire pondérée par rang.
        /// </summary>
        /// <returns>Retourne des couples d'individus.</returns>
        public IEnumerable<Individu>[] SelectionRang()
        {
            IEnumerable<Individu>[] couples = new IEnumerable<Individu>[this.individus.Length];

            double sommeRangs = this.individus.Length * (this.individus.Length + 1d) / 2d;
            double sommeInverses = 0;

            for (int i = 0; i < this.individus.Length; i++)
            {
                sommeInverses += sommeRangs / (i + 1d);
            }

            for (int i = 0; i < this.individus.Length; i++)
            {
                Individu parent1 = null;
                Individu parent2 = null;

                double rand1 = this.Rand.NextDouble() * sommeInverses;
                double rand2 = this.Rand.NextDouble() * sommeInverses;
                double temp = 0;
                int index = 0;

                while (parent1 == null || parent2 == null)
                {
                    temp += sommeRangs / (index + 1);

                    if (parent1 == null && temp > rand1)
                    {
                        parent1 = this.individus[index];
                    }

                    if (parent2 == null && temp > rand2)
                    {
                        parent2 = this.individus[index];
                    }

                    index++;
                }

                couples[i] = new[] { parent1, parent2 };
            }

            return couples;
        }

        /// <summary>
        /// Lance la reproduction de tous les couples.
        /// </summary>
        /// <param name="couples">Liste des couples.</param>
        public void Reproduction(IEnumerable<Individu>[] couples)
        {
            Individu[] nouveauxIndividus = new Individu[this.individus.Length];

            // Pour chaque couple.
            for (int indexCouple = 0; indexCouple < couples.Length; indexCouple++)
            {
                Individu parent1 = couples[indexCouple].ElementAt(0);
                Individu parent2 = couples[indexCouple].ElementAt(1);
                Individu enfant = new Individu(this.Point0, parent1.Taille);

                int separation = this.Rand.Next(1, parent1.Taille - 1);

                // Ajoute tous les gènes du parent1 jusqu'à la segmentation.
                for (int indexGeneP1 = 0; indexGeneP1 < separation; indexGeneP1++)
                {
                    enfant.Points[indexGeneP1] = parent1.Points[indexGeneP1];
                }

                // Ajoute les gènes du parent2 sauf s'ils ont déjà été hérités du parent1.
                int indexEnfant = parent2.Taille - 1;
                for (int indexGeneP2 = parent2.Taille - 1; indexGeneP2 >= 0; indexGeneP2--)
                {
                    if (!enfant.Points.Contains(parent2.Points[indexGeneP2]))
                    {
                        enfant.Points[indexEnfant] = parent2.Points[indexGeneP2];
                        indexEnfant--;
                    }
                }

                nouveauxIndividus[indexCouple] = enfant;
            }

            this.individus = nouveauxIndividus;
        }

        /// <summary>
        /// Exécute des mutations sur les gènes des individus.
        /// </summary>
        /// <param name="probabilite">Probabilité qu'une mutation se produise sur un gène.</param>
        public void Mutation(double probabilite)
        {
            // pour chaque individu.
            for (int indexIndividu = 0; indexIndividu < this.individus.Length; indexIndividu++)
            {
                // Pour chaque gène de cet individu.
                for (int indexGene1 = 0; indexGene1 < this.individus[indexIndividu].Points.Length; indexGene1++)
                {
                    // Effectue une mutation si la valeur tirée au sort est inférieure à la probabilité.
                    if (this.Rand.NextDouble() < probabilite)
                    {
                        int indexGene2 = this.Rand.Next(0, this.individus[indexIndividu].Points.Length);
                        Point gene1 = this.individus[indexIndividu].Points[indexGene1];
                        Point gene2 = this.individus[indexIndividu].Points[indexGene2];

                        this.individus[indexIndividu].Points[indexGene1] = gene2;
                        this.individus[indexIndividu].Points[indexGene2] = gene1;
                    }
                }
            }
        }
    }
}
