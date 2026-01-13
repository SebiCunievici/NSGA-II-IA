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
            double[] minime = new double[] { 0.0, 0.0 }; // Minim pt W1 si W2
            double[] maxime = new double[] { 1.0, 1.0 }; // Maxim pt W1 si W2

            return new Chromosome(2, minime, maxime);
        }

        public void ComputeFitness(Chromosome c)
        {
            double totalDist = 0;
            double minSafety = double.MaxValue;

            // Folosim un set de obstacole pentru evaluare
            for (int i = 0; i < 20; i++)
            {
                double obstacle = 5.0 + _r.NextDouble() * 18.0;

                // NOUA FORMULA: Velocitatea foloseste ambele gene
                // W1 (Genes[0]) impinge robotul inainte proportional cu distanta
                // W2 (Genes[1]) il franeaza invers proportional cu distanta
                double velocity = (c.Genes[0] * obstacle) - (c.Genes[1] / obstacle);//cele doua gene sunt invers proportionale

                // Limitare fizica: impiedicam robotul sa mearga cu spatele sau sa mearga prea repede
                velocity = Math.Max(0.5, Math.Min(10, velocity));

                double gap = obstacle - velocity;

                // Verificam coliziunea (izbirea)
                if (gap < 0)
                {
                    // Penalizare drastica pentru coliziune
                    totalDist -= 100;
                    minSafety = -10;
                    break;
                }

                totalDist += velocity;//daca velocitatea e mare inseamna ca nu a franat mult->inseamna ca a parcurs mai repede traseul
                if (gap < minSafety) 
                    minSafety = gap;//daca distanta la fiecare iteratie de obstacol e mare, insemna ca a franat din timp
            }



            // Setarea obiectivelor pentru NSGA-II
            c.Objectives[0] = -totalDist; // Obiectiv 1: Maximizare Performanta
            c.Objectives[1] = -minSafety; // Obiectiv 2: Maximizare Siguranta robotii foare prudenti (caz extrem front pareto) vor fi genetic creati sa 
                                          // franeze foarte din timp inaintea unui obstacol datorita raportului dintre w2 si dits.obstacol
        }

        public void RunLiveSimulation(Chromosome c, int generation)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($" GENERATIA: {generation} - Antrenament Pilot");
            Console.WriteLine($" Gene: W1={c.Genes[0]:F2} (Accel), W2={c.Genes[1]:F2} (Frana)");
            Console.WriteLine("========================================\n");

            string track = "________________________________________";
            int robotPos = 0;

            for (int step = 0; step < 15; step++)
            {
                Console.SetCursorPosition(0, 6);
                robotPos = (robotPos + 2) % 40;

                char[] visualTrack = track.ToCharArray();
                visualTrack[robotPos] = 'R';
                if (step % 4 == 0) visualTrack[(robotPos + 10) % 40] = 'X';

                Console.WriteLine("Traseu: " + new string(visualTrack));
                Console.WriteLine("\nSenzori: [DISTANTA OBSTACOL: OK]");

                // Vizualizarea vitezei medii obtinute
                double visualSpeed = Math.Max(0, -c.Objectives[0] / 20.0);
                int barCount = (int)Math.Min(30, visualSpeed * 3);

                Console.Write("Putere Motor: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(new string('>', barCount));
                Console.ResetColor();

                Thread.Sleep(100);
            }
        }
    }
}