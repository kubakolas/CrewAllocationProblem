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

        public void loadStartingData2()
        {
            names = new List<string>
                      { "Oliver",
                      "Jack",
                      "Harry",
                      "Jacob",
                      "Charlie",
                      "Thomas",
                      "Oscar",
                      "Bob",
                      "James",
                      "William",
                      "Jake",
                      "Margaret",
                      "Tracy",
                      "Marilyn",
                      "Carolyn",
                      "Michelle",
                      "Megan",
                      "Poppy",
                      "Elizabeth",
                      "Margaret"};

            crewCount = names.Count;

            // boolean values which describes each person
            // steward, hostess, french, spanish, german
            personsAttributes = new List<int[]>()
            {
              new int[]{1,0,1,0,1},
              new int[]{1,0,0,0,0},
              new int[]{1,0,0,0,1},
              new int[]{1,0,1,0,1},
              new int[]{1,0,0,0,0},
              new int[]{1,0,1,1,1},
              new int[]{1,0,0,1,0},
              new int[]{1,0,1,0,0},
              new int[]{1,0,0,1,1},
              new int[]{1,0,1,0,0},
              new int[]{1,1,0,1,0},
              new int[]{0,1,0,0,1},
              new int[]{0,1,1,0,0},
              new int[]{0,1,0,1,0},
              new int[]{0,1,1,0,1},
              new int[]{0,1,1,1,1},
              new int[]{0,1,0,1,0},
              new int[]{0,1,1,0,1},
              new int[]{0,1,1,0,0},
              new int[]{0,1,1,1,1}
            };

            // each column is a required number for each flight
            // columns: staff, stewards, hostesses, french, spanish, german
            crewRequirements = new List<int[]>() {
                new int[]{4,2,2,1,1,1}, // flight 1
                new int[]{4,2,2,1,2,1}, // flight 2
                new int[]{7,2,2,1,1,1},
                new int[]{6,1,1,1,2,1},
                new int[]{4,3,1,1,1,1},
                new int[]{4,1,3,1,1,1},
                new int[]{4,2,2,1,2,1},
                new int[]{7,1,2,2,1,1},
                new int[]{3,2,1,2,1,1},
                new int[]{3,2,1,1,2,1},
            };

            flightsCount = crewRequirements.Count;
            flightsToNames = new string[flightsCount, 7];
        }

        public async Task Solve()
        {
            await Task.Run(() =>
            {
                crewCount = names.Count;
                flightsCount = crewRequirements.Count;

                VariableInteger[,] flightsToPersons = new VariableInteger[flightsCount, crewCount];
                for (int i = 0; i < flightsCount; i++)
                {
                    for (int j = 0; j < crewCount; j++)
                    {
                        flightsToPersons[i, j] = new VariableInteger("crew" + i.ToString() + j.ToString(), 0, 1);
                    }
                }
                var variables = flightsToPersons.Cast<VariableInteger>().ToList();


                // Constraints
                var constraints = new List<ConstraintInteger>();

                // create constraints for each flight
                for (int f = 0; f < flightsCount; f++)
                {
                    // size of crew for each flight must be equal to input value
                    var crewForFlight = new ExpressionInteger[crewCount];
                    for (int p = 0; p < crewCount; p++)
                    {
                        crewForFlight[p] = flightsToPersons[f, p];
                    }
                    var flightCrewCount = crewForFlight.Aggregate((a, b) => a + b);
                    var crewSizesEqual = (flightCrewCount == crewRequirements[f][0]);
                    constraints.Add(new ConstraintInteger(crewSizesEqual));

                    // person attributes (is steward, is hostess, speaks french, speaks spanish, speaks german)
                    // sum of persons with each attribute must be greater than or equal to input value
                    for (int a = 0; a < 5; a++)
                    {
                        crewForFlight = new ExpressionInteger[crewCount];
                        for (int p = 0; p < crewCount; p++)
                        {
                            crewForFlight[p] = (flightsToPersons[f, p] * personsAttributes[p][a]);
                        }
                        var sum = crewForFlight.Aggregate((x, y) => x + y);
                        var attributesGreaterOrEqual = sum >= crewRequirements[f][a + 1];
                        constraints.Add(new ConstraintInteger(attributesGreaterOrEqual));
                    }
                }

                // crew member needs to have 2 flights break after his flight
                for (int f = 0; f < flightsCount - 2; f++)
                {
                    for (int i = 0; i < crewCount; i++)
                    {
                        var cs = ((flightsToPersons[f, i] + flightsToPersons[f + 1, i] + flightsToPersons[f + 2, i]) <= 1);
                        constraints.Add(new ConstraintInteger(cs));
                    }
                }

                // search for solution

                IVariable<int> optimiser = new VariableInteger("optimiser", 1, 100);
                IState<int> state = new StateInteger(variables, constraints);
                state.StartSearch(out StateOperationResult searchResult, optimiser, out IDictionary<string, IVariable<int>> solution, 10);
                if (searchResult == StateOperationResult.Unsatisfiable || searchResult == StateOperationResult.TimedOut)
                {
                    MessageBox.Show("The result is timed out or unsatisfiable", "Crew Allocation Problem",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }

                flightsToNames = new string[flightsCount, 7];
                for (int i = 0; i < flightsCount; i++)
                {
                    int count = 0;
                    for (int j = 0; j < crewCount; j++)
                    {
                        if (flightsToPersons[i, j].Value == 1)
                        {
                            flightsToNames[i, count] = names[j];
                            count++;
                        }
                    }
                }
            });
        }
    }
}
