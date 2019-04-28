using System;
using System.Collections.Generic;
using System.Linq;
using Decider.Csp.BaseTypes;
using Decider.Csp.Integer;

namespace CrewAllocationProblem
{
    class Logic
    {

        private static void Solve()
        {
            // WEJSCIA 

            string[] names = {"Tom",
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

            int num_persons = 20;
            int[,] attributes = {
            // steward, hostess, french, spanish, german
              {1,0,0,0,1},   // Tom     = 0
              {1,0,0,0,0},   // David   = 1
              {1,0,0,0,1},   // Jeremy  = 2
              {1,0,0,0,0},   // Ron     = 3
              {1,0,0,1,0},   // Joe     = 4
              {1,0,1,1,0},   // Bill    = 5
              {1,0,0,1,0},   // Fred    = 6
              {1,0,0,0,0},   // Bob     = 7
              {1,0,0,1,1},   // Mario   = 8
              {1,0,0,0,0},   // Ed      = 9
              {0,1,0,0,0},   // Carol   = 10
              {0,1,0,0,0},   // Janet   = 11
              {0,1,0,0,0},   // Tracy   = 12
              {0,1,0,1,0},   // Marilyn = 13
              {0,1,0,0,0},   // Carolyn = 14
              {0,1,0,0,1},   // Cathy   = 15
              {0,1,1,1,0},   // Inez    = 16
              {0,1,1,0,0},   // Jean    = 17
              {0,1,0,1,0},   // Heather = 18
              {0,1,1,0,1}    // Juliet  = 19
        };

            //
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

            int[,] required_crew = {
                {4,1,1,1,1,1}, // Flight 1
                {5,1,1,1,1,1}, // Flight 2
                {5,1,1,1,1,1}, // ..
                {6,2,2,1,1,1},
                {7,3,3,1,1,1},
                {4,1,1,1,1,1},
                {5,1,1,1,1,1},
                {6,1,1,1,1,1},
                {6,2,2,1,1,1}, // ...
                {7,3,3,1,1,1}  // Flight 10
            };



            // KONIEC WEJSC

            int num_flights = required_crew.GetLength(0);

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
                var sizesEqual = (sum == required_crew[f, 0]);
                csArray.Add(new ConstraintInteger(sizesEqual));

                // attributes and requirements
                for (int a = 0; a < 5; a++)
                {
                    ExpressionInteger[] tmp2 = new ExpressionInteger[num_persons];
                    for (int p = 0; p < num_persons; p++)
                    {
                        tmp2[p] = (s[f, p] * attributes[p, a]);
                    }
                    var sum2 = new ExpressionInteger(0);
                    foreach (var v in tmp2)
                    {
                        sum2 += v;
                    }
                    var attEqual = sum2 >= required_crew[f, a + 1];
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

            IState<int> state = new StateInteger(variables, csArray);
            state.StartSearch(out StateOperationResult searchResult);

            for (int i = 0; i < num_flights; i++)
            {
                Console.Write("Flight " + i.ToString() + " :   ");
                for (int j = 0; j < num_persons; j++)
                {
                    if (s[i, j].Value == 1)
                    {
                        Console.Write(names[j] + " ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Runtime:\t{0}\nBacktracks:\t{1}\n", state.Runtime, state.Backtracks);
        }

    }
}
