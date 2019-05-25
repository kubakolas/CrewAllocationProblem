using System;
using System.Collections.Generic;
using System.Linq;
using Decider.Csp.BaseTypes;
using Decider.Csp.Integer;
using CrewAllocationProblem;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;

namespace CrewAllocationProblem
{
    class Logic
    {
        public List<string> names;
        public int crewCount;
        public List<int[]> personsAttributes;
        public List<int[]> crewRequirements;
        public string[,] flightsToNames;
        public int flightsCount;

        public void loadStartingData()
        {
            names = new List<string>
                      { "Tom",
                      "David",
                      "Jeremy",
                      "Ron",
                      "Joe",
                      "Bill",
                      "Fred",
                      "Bob",
                      "Mario",
                      "Ed",
                      "Carol",
                      "Janet",
                      "Tracy",
                      "Marilyn",
                      "Carolyn",
                      "Cathy",
                      "Inez",
                      "Jean",
                      "Heather",
                      "Juliet"};

            crewCount = names.Count;

            // bool values which describes each person
            // steward, hostess, french, spanish, german
            personsAttributes = new List<int[]>()
            {
              new int[]{1,0,0,0,1},
              new int[]{1,0,0,0,0},
              new int[]{1,0,0,0,1},
              new int[]{1,0,0,0,0},
              new int[]{1,0,0,1,0},
              new int[]{1,0,1,1,0},
              new int[]{1,0,0,1,0},
              new int[]{1,0,0,0,0},
              new int[]{1,0,0,1,1},
              new int[]{1,0,0,0,0},
              new int[]{0,1,0,0,0},
              new int[]{0,1,0,0,0},
              new int[]{0,1,0,0,0},
              new int[]{0,1,0,1,0},
              new int[]{0,1,0,0,0},
              new int[]{0,1,0,0,1},
              new int[]{0,1,1,1,0},
              new int[]{0,1,1,0,0},
              new int[]{0,1,0,1,0},
              new int[]{0,1,1,0,1}
            };

            // each column is a required number for each flight
            // columns: staff, stewards, hostesses, french, spanish, german
            crewRequirements = new List<int[]>() {
                new int[]{4,1,1,1,1,1}, // flight 1
                new int[]{5,1,1,1,1,1}, // flight 2
                new int[]{5,1,1,1,1,1},
                new int[]{6,2,2,1,1,1},
                new int[]{7,3,3,1,1,1},
                new int[]{4,1,1,1,1,1},
                new int[]{5,1,1,1,1,1},
                new int[]{6,1,1,1,1,1},
                new int[]{6,2,2,1,1,1},
                new int[]{7,3,3,1,1,1}
            };

            flightsCount = crewRequirements.Count;
            flightsToNames = new string[flightsCount, 7];
        }

        public async Task Solve()
        {
            await Task.Run(() =>
            {

                flightsCount = crewRequirements.Count;
                flightsToNames = new string[flightsCount, 7];

                VariableInteger[,] flightsToPersons = new VariableInteger[flightsCount, crewCount];
                for (int i = 0; i < flightsCount; i++)
                {
                    for (int j = 0; j < crewCount; j++)
                    {
                        flightsToPersons[i, j] = new VariableInteger("crew" + i.ToString() + j.ToString(), 0, 1);
                    }
                }

                var variables = flightsToPersons.Cast<VariableInteger>().ToList();
                var working_persons_num = new VariableInteger("working_persons", 1, crewCount);

                // Constraints

                var csArray = new List<ConstraintInteger>();

                // number of working persons

                ExpressionInteger[] nw = new ExpressionInteger[crewCount];
                for (int p = 0; p < crewCount; p++)
                {
                    VariableInteger[] tmp = new VariableInteger[flightsCount];
                    for (int f = 0; f < flightsCount; f++)
                    {
                        tmp[f] = flightsToPersons[f, p];
                    }
                    var sum = new ExpressionInteger(0);
                    foreach (var v in tmp)
                    {
                        sum += v;
                    }
                    nw[p] = sum;
                }
                var global_sum = new ExpressionInteger(0);
                foreach (var v in nw)
                {
                    global_sum += v;
                }
                var working_number_cs = new ConstraintInteger(global_sum == working_persons_num);
                csArray.Add(working_number_cs);


                for (int f = 0; f < flightsCount; f++)
                {
                    // size of crew
                    ExpressionInteger[] tmp = new ExpressionInteger[crewCount];
                    for (int p = 0; p < crewCount; p++)
                    {
                        tmp[p] = flightsToPersons[f, p];
                    }
                    var sum = new ExpressionInteger(0);
                    foreach (var v in tmp)
                    {
                        sum += v;
                    }
                    var sizesEqual = (sum == crewRequirements[f][0]);
                    csArray.Add(new ConstraintInteger(sizesEqual));

                    // personsAttributes and requirements
                    for (int a = 0; a < 5; a++)
                    {
                        ExpressionInteger[] tmp2 = new ExpressionInteger[crewCount];
                        for (int p = 0; p < crewCount; p++)
                        {
                            tmp2[p] = (flightsToPersons[f, p] * personsAttributes[p][a]);
                        }
                        var sum2 = new ExpressionInteger(0);
                        foreach (var v in tmp2)
                        {
                            sum2 += v;
                        }
                        var attEqual = sum2 >= crewRequirements[f][a + 1];
                        csArray.Add(new ConstraintInteger(attEqual));
                    }
                }

                // after a flight, break for at least two flights
                for (int f = 0; f < flightsCount - 2; f++)
                {
                    for (int i = 0; i < crewCount; i++)
                    {
                        var cs = ((flightsToPersons[f, i] + flightsToPersons[f + 1, i] + flightsToPersons[f + 2, i]) <= 1);
                        csArray.Add(new ConstraintInteger(cs));
                    }
                }

                // Search
                IVariable<int> optimiser = new VariableInteger("optiser", 1, 100);
                IState<int> state = new StateInteger(variables, csArray);
                state.StartSearch(out StateOperationResult searchResult, optimiser, out IDictionary<string, IVariable<int>> solution, 10);
                if (searchResult == StateOperationResult.Unsatisfiable || searchResult == StateOperationResult.TimedOut)
                {
                    MessageBox.Show("The result is timed out or unsatisfiable", "Crew Allocation Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                for (int i = 0; i < flightsCount; i++)
                {
                    try
                    {
                        int count = 0;
                        Console.Write("Flight " + i.ToString() + " :   ");
                        for (int j = 0; j < crewCount; j++)
                        {
                            if (flightsToPersons[i, j].Value == 1)
                            {
                                flightsToNames[i, count] = names[j];
                                count++;
                            }
                        }
                    }
                    catch { }
                    Console.WriteLine();
                }
                Console.WriteLine("Runtime:\t{0}\nBacktracks:\t{1}\n", state.Runtime, state.Backtracks);
            });

        }

    }
}
