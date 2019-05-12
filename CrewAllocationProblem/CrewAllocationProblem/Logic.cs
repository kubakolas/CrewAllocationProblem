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
        public int num_persons;
        public List<int[]> attributes;
        public List<int[]> required_crew;
        public string[,] flight_crew;
        public int num_flights;

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

            num_persons = names.Count;

            attributes = new List<int[]>()
            {
                // steward, hostess, french, spanish, german
              new int[]{1,0,0,0,1},  // Tom     = 0
              new int[]{1,0,0,0,0},   // David   = 1
              new int[]{1,0,0,0,1},   // Jeremy  = 2
              new int[]{1,0,0,0,0},   // Ron     = 3
              new int[]{1,0,0,1,0},   // Joe     = 4
              new int[]{1,0,1,1,0},   // Bill    = 5
              new int[]{1,0,0,1,0},   // Fred    = 6
              new int[]{1,0,0,0,0},   // Bob     = 7
              new int[]{1,0,0,1,1},   // Mario   = 8
              new int[]{1,0,0,0,0},   // Ed      = 9
              new int[]{0,1,0,0,0},   // Carol   = 10
              new int[]{0,1,0,0,0},   // Janet   = 11
              new int[]{0,1,0,0,0},   // Tracy   = 12
              new int[]{0,1,0,1,0},   // Marilyn = 13
              new int[]{0,1,0,0,0},   // Carolyn = 14
              new int[]{0,1,0,0,1},   // Cathy   = 15
              new int[]{0,1,1,1,0},   // Inez    = 16
              new int[]{0,1,1,0,0},   // Jean    = 17
              new int[]{0,1,0,1,0},   // Heather = 18
              new int[]{0,1,1,0,1}    // Juliet  = 19
            };

            var a = attributes[1][2];
            // Required number of crew members.
            //
            // The columns are in the following order:
            // staff     : Overall number of cabin crew needed
            // stewards  : How many stewards are required
            // hostesses : How many hostesses are required
            // french    : How many French speaking employees are required
            // spanish   : How many Spanish speaking employees are required
            // german    : How many German speaking employees are required
            //
            required_crew = new List<int[]>() {
                new int[]{4,1,1,1,1,1}, // Flight 1
                new int[]{5,1,1,1,1,1}, // Flight 2
                new int[]{5,1,1,1,1,1}, // ..
                new int[]{6,2,2,1,1,1},
                new int[]{7,3,3,1,1,1},
                new int[]{4,1,1,1,1,1},
                new int[]{5,1,1,1,1,1},
                new int[]{6,1,1,1,1,1},
                new int[]{6,2,2,1,1,1}, // ...
                new int[]{7,3,3,1,1,1}  // Flight 10
            };

            num_flights = required_crew.Count;

            flight_crew = new string[num_flights, 7];
        }

        public async Task Solve()
        {
            await Task.Run(() =>
            { 
            num_flights = required_crew.Count;
            flight_crew = new string[num_flights, 7];

            VariableInteger[,] s = new VariableInteger[num_flights, num_persons];
            for (int i = 0; i < num_flights; i++)
            {
                for (int j = 0; j < num_persons; j++)
                {
                    s[i, j] = new VariableInteger("crew" + i.ToString() + j.ToString(), 0, 1);
                }
            }

            var variables = s.Cast<VariableInteger>().ToList();
            var working_persons_num = new VariableInteger("working_persons", 1, num_persons);

            // Constraints

            var csArray = new List<ConstraintInteger>();

            // number of working persons

            ExpressionInteger[] nw = new ExpressionInteger[num_persons];
            for (int p = 0; p < num_persons; p++)
            {
                VariableInteger[] tmp = new VariableInteger[num_flights];
                for (int f = 0; f < num_flights; f++)
                {
                    tmp[f] = s[f, p];
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


            for (int f = 0; f < num_flights; f++)
            {
                // size of crew
                ExpressionInteger[] tmp = new ExpressionInteger[num_persons];
                for (int p = 0; p < num_persons; p++)
                {
                    tmp[p] = s[f, p];
                }
                var sum = new ExpressionInteger(0);
                foreach (var v in tmp)
                {
                    sum += v;
                }
                var sizesEqual = (sum == required_crew[f][0]);
                csArray.Add(new ConstraintInteger(sizesEqual));

                // attributes and requirements
                for (int a = 0; a < 5; a++)
                {
                    ExpressionInteger[] tmp2 = new ExpressionInteger[num_persons];
                    for (int p = 0; p < num_persons; p++)
                    {
                        tmp2[p] = (s[f, p] * attributes[p][a]);
                    }
                    var sum2 = new ExpressionInteger(0);
                    foreach (var v in tmp2)
                    {
                        sum2 += v;
                    }
                    var attEqual = sum2 >= required_crew[f][a + 1];
                    csArray.Add(new ConstraintInteger(attEqual));
                }
            }

            // after a flight, break for at least two flights
            for (int f = 0; f < num_flights - 2; f++)
            {
                for (int i = 0; i < num_persons; i++)
                {
                    var cs = ((s[f, i] + s[f + 1, i] + s[f + 2, i]) <= 1);
                    csArray.Add(new ConstraintInteger(cs));
                }
            }

            // Search
            IVariable<int> optiseVar = new VariableInteger("optiser", 1, 100);
            IState<int> state = new StateInteger(variables, csArray);
            state.StartSearch(out StateOperationResult searchResult, optiseVar, out IDictionary<string, IVariable<int>> solution, 10);
            if(searchResult == StateOperationResult.Unsatisfiable || searchResult == StateOperationResult.TimedOut)
            {
                MessageBox.Show("The result is timed out or unsatisfiable", "Crew Allocation Problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            for (int i = 0; i < num_flights; i++)
            {
                    try
                    {
                        int count = 0;
                        Console.Write("Flight " + i.ToString() + " :   ");
                        for (int j = 0; j < num_persons; j++)
                        {
                            if (s[i, j].Value == 1)
                            {
                                flight_crew[i, count] = names[j];
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
