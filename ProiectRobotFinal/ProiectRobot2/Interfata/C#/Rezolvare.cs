using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms; 

namespace EvolutionaryAlgorithm
{
    // Operatorii rămân aceiași, adaptați pentru codificarea reală [cite: 1014, 1040]
    public class Selection
    {
        private static Random _rand = new Random();
        public static Chromosome Tournament(List<Chromosome> population)
        {
            Chromosome a = population[_rand.Next(population.Count)];
            Chromosome b = population[_rand.Next(population.Count)];

            if (a.Rank < b.Rank) 
                return new Chromosome(a);//apel constructor copiere,in caz ca voi folosi mai departe acel copil sa nu modific indivizii din generatia anterioara
            if (b.Rank < a.Rank) 
                return new Chromosome(b);//apel constructor copiere

            return a.CrowdingDistance > b.CrowdingDistance ? new Chromosome(a) : new Chromosome(b);
        }
    }

    public class Crossover
    {
        private static Random _rand = new Random();
        public static Chromosome Arithmetic(Chromosome mother, Chromosome father, double rate)
        {
            Chromosome child = new Chromosome(mother);
            if (_rand.NextDouble() < rate)
            {
                double alpha = _rand.NextDouble();
                for (int i = 0; i < child.NoGenes; i++)
                    child.Genes[i] = alpha * mother.Genes[i] + (1 - alpha) * father.Genes[i];
            }
            return child;
        }
    }

    public class Mutation
    {
        private static Random _rand = new Random();
        public static void Reset(Chromosome child, double rate)
        {
            for (int i = 0; i < child.NoGenes; i++)
            {
                if (_rand.NextDouble() < rate)
                    child.Genes[i] = child.MinValues[i] + _rand.NextDouble() * (child.MaxValues[i] - child.MinValues[i]);
            }
        }
    }

    public class EvolutionaryAlgorithm
    {
        public List<Chromosome> RunOneGeneration(IOptimizationProblem p, List<Chromosome> P, int N, double crRate, double mutRate)
        {
            List<Chromosome> Q = new List<Chromosome>();//lista goala copii noua generatie
            while (Q.Count < N)
            {
                Chromosome m = Selection.Tournament(P);
                Chromosome f = Selection.Tournament(P);
                Chromosome child = Crossover.Arithmetic(m, f, crRate);
                Mutation.Reset(child, mutRate);
                p.ComputeFitness(child);
                Q.Add(child);
            }

            // Reuniunea R=P+Q (dimensiune 2N)->elitismul,se pun la loc parintii cu copii
            List<Chromosome> R = new List<Chromosome>(P);
            R.AddRange(Q);

            var fronts = FastNonDominatedSort(R);

            List<Chromosome> nextGen = new List<Chromosome>();//lista cei mai buni roboti din cele mai bune fronturi, cei ce vor supravietui in generatia urmatoare
            int frontIdx = 0;
            while (frontIdx < fronts.Count && nextGen.Count + fronts[frontIdx].Count <= N)//cat timp mai incap indivizi
            {
                CalculateCrowdingDistance(fronts[frontIdx]);
                nextGen.AddRange(fronts[frontIdx]);
                frontIdx++;
            }

            if (nextGen.Count < N && frontIdx < fronts.Count)//cand un front intreg nu mai are loc in totalitate
            {
                CalculateCrowdingDistance(fronts[frontIdx]);
                var lastFrontSorted = fronts[frontIdx].OrderByDescending(c => c.CrowdingDistance).ToList();//cei mai izolati (cu o identitate mai mare) vor fi primii
                nextGen.AddRange(lastFrontSorted.Take(N - nextGen.Count));//urmatorul front
            }
            return nextGen;
        }

        private List<List<Chromosome>> FastNonDominatedSort(List<Chromosome> population)
        {
            // Logica de sortare Pareto pe ranguri
            int n = population.Count;//cati roboti in total in 2N
            var fronts = new List<List<Chromosome>> { new List<Chromosome>() }; //lista de fronturi (lista de liste)
            int[] dominationCount = new int[n];//pt fiecare robot cati il domina
            List<int>[] dominatedSet = new List<int>[n];//pt fiecare robot pe cati alti domina acesta

            for (int i = 0; i < n; i++)//compara fiecare robot cu toti ceilalti
            {
                dominatedSet[i] = new List<int>();//lista de roboti mai slabi decat robotul i
                for (int j = 0; j < n; j++)
                {
                    if (Dominates(population[i], population[j])) 
                        dominatedSet[i].Add(j);
                    else if (Dominates(population[j], population[i])) 
                        dominationCount[i]++;
                }

                //dominationCount == 0  ar inseamna ca nimeni nu e mai bun ca el=> rank=1,primul front Pareto
                if (dominationCount[i] == 0)
                {
                    population[i].Rank = 1;
                    fronts[0].Add(population[i]);//primul front pareto, nu e dominat de nimeni
                }
            }

            int k = 0;
            while (fronts[k].Count > 0)//parcurge fiecare front pentru a vedea urmatorii imediat mai buni decat elita si a-i pune in urmatorul front
            {
                var nextFront = new List<Chromosome>();//urmatorul nivel (front2)
                foreach (var dominator in fronts[k])//luam fiecare individ din front
                {
                    int pIdx = population.IndexOf(dominator);//fiecare individ din front si comparam cu parent (index dominator)
                    foreach (int qIdx in dominatedSet[pIdx])//ce am zis mai sus
                    {
                        dominationCount[qIdx]--;//vedem daca dominatorul curent e singurul care il bate
                        if (dominationCount[qIdx] == 0)//daca da il pun in urmatorul front
                        {
                            population[qIdx].Rank = k + 2;//fronts[0] contine robotii de rank1
                            nextFront.Add(population[qIdx]);
                        }
                    }
                }
                k++;
                fronts.Add(nextFront);
            }
            return fronts;
        }

        private bool Dominates(Chromosome a, Chromosome b)
        {
            //obiective minimizate->val mica inseamna performanta buna
            // un individ domina daca nu este mai slab decat altul la niciun obiectiv (macar egal) si este stricvt mai bun la cel putin unul
            
            bool isStrictlyBetterAtLeastOnce = false;

            for (int i = 0; i < a.Objectives.Length; i++)
            {
                 // conditia 1:sa nu fie a mai slab la vreun obiectiv in comparatie cu b
                 if (a.Objectives[i] > b.Objectives[i])
                 {
                     return false;
                 }

                 // conditia 2:sa fie macar un obiectiv une a este strict mai bun
                 if (a.Objectives[i] < b.Objectives[i])
                 {
                        isStrictlyBetterAtLeastOnce = true;
                 }
            }

               
                return isStrictlyBetterAtLeastOnce;
        }

        //CrowdingDistance ii pune mereu primii pe cei mai unici (e o sortare)
        private void CalculateCrowdingDistance(List<Chromosome> front)
        {
            // Menținerea diversității pe front [cite: 827-828, 837]
            int n = front.Count;
            if (n < 3) { front.ForEach(c => c.CrowdingDistance = 1e10); return; }
            front.ForEach(c => c.CrowdingDistance = 0);
            for (int m = 0; m < 2; m++)
            {
                var sorted = front.OrderBy(c => c.Objectives[m]).ToList();
                sorted[0].CrowdingDistance = 1e10;
                sorted[n - 1].CrowdingDistance = 1e10;
                double range = sorted[n - 1].Objectives[m] - sorted[0].Objectives[m];
                if (range > 0)
                    for (int i = 1; i < n - 1; i++)
                        sorted[i].CrowdingDistance += (sorted[i + 1].Objectives[m] - sorted[i - 1].Objectives[m]) / range;
            }
        }
    }

    public class Program
    {


        [STAThread]
        static void Main(string[] args)
        {
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm();
            RobotEvolution problem = new RobotEvolution();
            int popSize = 100; // Populație pentru diversitate
            List<Chromosome> population = new List<Chromosome>();

            // Initializarea populatiei
            for (int i = 0; i < popSize; i++)
            {
                var c = problem.MakeChromosome();
                problem.ComputeFitness(c);
                population.Add(c);
            }

            // 1. Monitorizarea Evoluției (Logare la fiecare 25 de generații)
            Console.WriteLine("=== PROGRES EVOLUTIE (NSGA-II) ===");
            Console.WriteLine("Gen | Viteza Max | Siguranta Max | Indivizi Elita");
            Console.WriteLine("-------------------------------------------------");

            for (int gen = 0; gen <= 100; gen++)
            {
                population = ea.RunOneGeneration(problem, population, popSize, 0.9, 0.1);

                if (gen % 25 == 0)
                {
                    var rank1 = population.Where(p => p.Rank == 1).ToList();
                    var bSpeed = rank1.OrderBy(p => p.Objectives[0]).First();
                    var bSafety = rank1.OrderBy(p => p.Objectives[1]).First();
                    // Objectives[0] este distanța totală negativă, deci afișăm cu minus pentru valoare pozitivă
                    Console.WriteLine($"{gen,3} | {-bSpeed.Objectives[0],10:F2} | {-bSafety.Objectives[1],13:F2} | {rank1.Count,12}");
                }
            }

            // 2. Extragerea și sortarea Frontului Pareto
            var finalFront = population.Where(p => p.Rank == 1)
                                       .OrderBy(p => p.Objectives[0])
                                       .ToList();

            // 3. Setări obligatorii pentru Interfața Grafică (apelate o singură dată)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool continua = true;
            while (continua)
            {
                // Afișăm catalogul la fiecare revenire în meniu
                Console.WriteLine("\n=== CATALOG DE PILOTI EVOLUATI (Frontul Pareto) ===");
                Console.WriteLine("ID  | Tip Pilot     | Viteza (Obj1) | Siguranta (Obj2) | Gena W1 | Gena W2");
                Console.WriteLine("-------------------------------------------------------------------------");

                for (int i = 0; i < finalFront.Count; i++)
                {
                    var c = finalFront[i];
                    string tip = (i == 0) ? "VITEZOMAN  " : (i == finalFront.Count - 1) ? "PRUDENT    " : "ECHILIBRAT ";
                    Console.WriteLine($"{i,-3} | {tip} | {-c.Objectives[0],13:F2} | {-c.Objectives[1],16:F2} | {c.Genes[0],7:F2} | {c.Genes[1],7:F2}");
                }

                Console.WriteLine("\nIntroduceti ID-ul robotului (0 - " + (finalFront.Count - 1) + ") pentru a lansa simularea.");
                Console.WriteLine("Introduceti 'q' pentru a inchide programul.");
                Console.Write("Alegere: ");

                string input = Console.ReadLine();

                if (input != null && input.ToLower() == "q")
                {
                    continua = false;
                }
                else if (int.TryParse(input, out int choice) && choice >= 0 && choice < finalFront.Count)
                {
                    Console.WriteLine($"\nSe lanseaza simularea pentru ID {choice}...");
                    Console.WriteLine("ÎNCHIDETI fereastra grafica pentru a reveni la meniul de selectie.");

                    // Această linie blochează consola până când închizi fereastra de simulare
                    Application.Run(new SimulationForm(finalFront[choice]));
                }
                else
                {
                    Console.WriteLine("\n!!! ID invalid! Va rugam sa introduceti un numar valid din tabel.");
                }
            }
        }
    }
}