using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionaryAlgorithm
{
    public class RobotEvolution : IOptimizationProblem
    {
        private Random _r = new Random();

        public Chromosome MakeChromosome()
        {
            // Gene: W1 (greutate viteza), W2 (greutate franare)
            double[] minime = new double[] { 30.0, 0.0 }; // Minim pt W1 si W2
            double[] maxime = new double[] { 130.0, 1.0 }; // Maxim pt W1 si W2

            return new Chromosome(2, minime, maxime);
        }

        public void ComputeFitness(Chromosome c)
        {
            double distance = 100; // distanta in kilometri

            c.Objectives[0] = distance / c.Genes[0];

            double optimal_fuel_performance = 15; // 15km / l
            double optimal_average_speed = 80; // 80 km/h
            double warm_up_distance = 5; // 5 km, timpul necesar incalzirii motorului
            double alfa = 0.00005; // parametrii pentru functiile exponentiale
            double beta = 0.25;

            // Calculate penalty factors
            double speedPenalty = Math.Exp(-alfa * Math.Pow(c.Genes[0] - optimal_average_speed, 2));
            double aggressivenessPenalty = Math.Exp(-beta * c.Genes[1]);
            double warmUpFactor = 1 - Math.Exp(-distance / warm_up_distance);

            // Effective fuel efficiency
            double effectiveEta = optimal_fuel_performance * speedPenalty * aggressivenessPenalty * warmUpFactor;

            // Safety check to avoid division by zero
            if (effectiveEta < 0.1) effectiveEta = 0.1;

            // Fuel consumed (objective)
            c.Objectives[1] = distance / effectiveEta;

        }

    }
}