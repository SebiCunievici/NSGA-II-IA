using System;

namespace EvolutionaryAlgorithm
{
    /// <summary>
    /// Clasa care reprezinta un individ din populatie adaptat pentru NSGA-II
    /// </summary>
    public class Chromosome
    {
        public int NoGenes { get; set; } // Numarul de gene ale individului
        public double[] Genes { get; set; } // Valorile genelor (W1, W2 pentru robot)
        public double[] MinValues { get; set; } // Limitele inferioare ale genelor
        public double[] MaxValues { get; set; } // Limitele superioare ale genelor

        //+NSGA II

        //tablou obiective
        public double[] Objectives { get; set; }

        // rang Pareto, cu 1 cel mai bun
        public int Rank { get; set; }

        // diversitate pt evitare aglomerare/valori similare->alg se duce in 2 directii difertie
        public double CrowdingDistance { get; set; }

        private static Random _rand = new Random();

      
        public Chromosome(int noGenes, double[] minValues, double[] maxValues)
        {
            NoGenes = noGenes;
            Genes = new double[noGenes];
            MinValues = (double[])minValues.Clone();
            MaxValues = (double[])maxValues.Clone();

            
            Objectives = new double[2];

            for (int i = 0; i < noGenes; i++)
            {
                // Initializare aleatorie uniforma in domeniul specificat
                Genes[i] = minValues[i] + _rand.NextDouble() * (maxValues[i] - minValues[i]);
            }
        }

        /// <summary>
        /// Constructor de copiere - Critic pentru elitismul NSGA-II (populatia combinata 2N)
        /// </summary>
        public Chromosome(Chromosome c)
        {
            NoGenes = c.NoGenes;
            Rank = c.Rank;
            CrowdingDistance = c.CrowdingDistance;

            Genes = (double[])c.Genes.Clone();
            MinValues = (double[])c.MinValues.Clone();
            MaxValues = (double[])c.MaxValues.Clone();
            Objectives = (double[])c.Objectives.Clone();
        }
    }
}