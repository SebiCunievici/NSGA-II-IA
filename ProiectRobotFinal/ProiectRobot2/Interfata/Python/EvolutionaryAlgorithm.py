##########################################################################
#                                                                        #
#  Copyright:   (c) 2024, Florin Leon                                    #
#  E-mail:      florin.leon@academic.tuiasi.ro                           #
#  Website:     http://florinleon.byethost24.com/lab_ia.html             #
#  Description: Evolutionary Algorithms                                  #
#               (Artificial Intelligence lab 8)                          #
#                                                                        #
#  This code and information is provided "as is" without warranty of     #
#  any kind, either expressed or implied, including but not limited      #
#  to the implied warranties of merchantability or fitness for a         #
#  particular purpose. You are free to use this source code in your      #
#  applications as long as the original copyright notice is included.    #
#                                                                        #
##########################################################################


import random
import math

# Interface for optimization problems
class IOptimizationProblem:
    def compute_fitness(self, chromosome):
        raise NotImplementedError("This method needs to be implemented by a subclass")

    def make_chromosome(self):
        raise NotImplementedError("This method needs to be implemented by a subclass")


class Equation(IOptimizationProblem):
    def make_chromosome(self):
        # un cromozom are o gena (x) care poate lua valori in intervalul (-5, 5)
        return Chromosome(1, [-5], [5])

    def compute_fitness(self, chromosome):
        raise Exception("Aceasta metoda trebuie completata")


class Fence(IOptimizationProblem):
    def make_chromosome(self):
        # un cromozom are doua gene (x si y) care pot lua valori in intervalul (0, 100)
        return Chromosome(2, [0, 0], [100, 100])

    def compute_fitness(self, chromosome):
        raise Exception("Aceasta metoda trebuie completata")


class Chromosome:
    def __init__(self, no_genes, min_values, max_values):
        self.no_genes = no_genes
        self.genes = [0.0] * no_genes
        self.min_values = list(min_values)
        self.max_values = list(max_values)
        self.fitness = 0.0
        self._initialize_genes()

    def _initialize_genes(self):
        for i in range(self.no_genes):
            self.genes[i] = self.min_values[i] + random.random() * (self.max_values[i] - self.min_values[i])

    def __copy__(self):
        new_copy = Chromosome(self.no_genes, self.min_values, self.max_values)
        new_copy.genes = list(self.genes)
        new_copy.fitness = self.fitness
        return new_copy

    def copy_from(self, other):
        self.no_genes = other.no_genes
        self.genes = list(other.genes)
        self.min_values = list(other.min_values)
        self.max_values = list(other.max_values)
        self.fitness = other.fitness


class Selection:
    @staticmethod
    def tournament(population):
        raise Exception("Aceasta metoda trebuie implementata")

    @staticmethod
    def get_best(population):
        raise Exception("Aceasta metoda trebuie implementata")


class Crossover:
    @staticmethod
    def arithmetic(mother, father, rate):
        raise Exception("Aceasta metoda trebuie implementata")


class Mutation:
    @staticmethod
    def reset(child, rate):
        raise Exception("Aceasta metoda trebuie implementata")


class EvolutionaryAlgorithm:
    def solve(self, problem, population_size, max_generations, crossover_rate, mutation_rate):
        raise Exception("Aceasta metoda trebuie completata")

        population = [problem.make_chromosome() for _ in range(population_size)]
        for individual in population:
            problem.compute_fitness(individual)

        for gen in range(max_generations):
            new_population = [Selection.get_best(population)]

            for i in range(1, population_size):
                # selectare 2 parinti: Selection.tournament
                # generarea unui copil prin aplicare crossover: Crossover.arithmetic
                # aplicare mutatie asupra copilului: Mutation.reset
                # calculare fitness pentru copil: compute_fitness din problema p
                # introducere copil in new_population

            population = new_population

        return Selection.get_best(population)


if __name__ == "__main__":
    raise Exception("Aceasta metoda trebuie completata")

    ea = EvolutionaryAlgorithm()

    # solution = ea.solve(Equation(), ...)  # de completat parametrii algoritmului
    # se foloseste -solution.Fitness pentru ca algoritmul evolutiv maximizeaza, iar aici avem o problema de minimizare
    # print(f"{solution.genes[0]:.6f} -> {-solution.fitness:.6f}")
          
    # solution = ea.solve(Fence(), ...)  # de completat parametrii algoritmului
    # print(f"{solution.genes[0]:.2f} {solution.genes[1]:.2f} -> {solution.fitness:.4f}")
