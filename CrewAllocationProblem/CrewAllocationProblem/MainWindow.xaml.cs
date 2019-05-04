using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


class Crew
{
    public string Number { get; set; }
    public string Name { get; set; }
    public string Steward { get; set; }
    public string Hostess { get; set; }
    public string French { get; set; }
    public string Spanish { get; set; }
    public string German { get; set; }

    public Crew() { }

    public Crew(string number, string name, string steward, string hostess, string french, string spanish, string german)
    {
        Number = number;
        Name = name;
        Steward = steward;
        Hostess = hostess;
        French = french;
        Spanish = spanish;
        German = german;
    }

    public List<Crew> crewList(List<string> names, List<int[]> attributes)
    {
        List<Crew> crews = new List<Crew>();

        for (int i = 0; i < names.Count; i++)
        {
            crews.Add(
                new Crew(
                    (i+1).ToString(),
                    names[i],
                    attributes[i][0] == 0 ? "No" : "Yes",
                    attributes[i][1] == 0 ? "No" : "Yes",
                    attributes[i][2] == 0 ? "No" : "Yes",
                    attributes[i][3] == 0 ? "No" : "Yes",
                    attributes[i][4] == 0 ? "No" : "Yes"));
        }
        return crews;
    }
}

class RequiredCrew
{
    public RequiredCrew() { }

    public RequiredCrew(int flight, int staff, int steward, int hostess, int french, int spanish, int german)
    {
        Flight = flight;
        Staff = staff;
        Steward = steward;
        Hostess = hostess;
        French = french;
        Spanish = spanish;
        German = german;
    }

    public int Flight { get; set; }
    public int Staff { get; set; }
    public int Steward { get; set; }
    public int Hostess { get; set; }
    public int French { get; set; }
    public int Spanish { get; set; }
    public int German { get; set; }

    public List<RequiredCrew> requiredCrewList(List<int[]> required_crew)
    {
        List<RequiredCrew> crewquiredCrewews = new List<RequiredCrew>();

        for (int i = 0; i < required_crew.Count; i++)
        {
            crewquiredCrewews.Add(
                new RequiredCrew(
                    i+1,
                    required_crew[i][0],
                    required_crew[i][1],
                    required_crew[i][2],
                    required_crew[i][3],
                    required_crew[i][4],
                    required_crew[i][5]));
        }
        return crewquiredCrewews;
    }

}

class Flights
{
    public string FlightNumber { get; set; }
    public string Person1 { get; set; }
    public string Person2 { get; set; }
    public string Person3 { get; set; }
    public string Person4 { get; set; }
    public string Person5 { get; set; }
    public string Person6 { get; set; }
    public string Person7 { get; set; }

    public Flights()
    {
    }

    public Flights(string flightNumber, string person1, string person2, string person3, string person4, string person5, string person6, string person7)
    {
        FlightNumber = flightNumber;
        Person1 = person1;
        Person2 = person2;
        Person3 = person3;
        Person4 = person4;
        Person5 = person5;
        Person6 = person6;
        Person7 = person7;
    }

    public List<Flights> flightList(string[,] flight_crew)
    {
        List<Flights> flights = new List<Flights>();

        for (int i = 0; i < flight_crew.GetLength(0); i++)
        {
            flights.Add(
                new Flights(
                    (i + 1).ToString(),
                    flight_crew[i, 0] ?? "-",
                    flight_crew[i, 1] ?? "-",
                    flight_crew[i, 2] ?? "-",
                    flight_crew[i, 3] ?? "-",
                    flight_crew[i, 4] ?? "-",
                    flight_crew[i, 5] ?? "-",
                    flight_crew[i, 6] ?? "-"));
        }
        return flights;
    }

}

namespace CrewAllocationProblem
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logic logic = new Logic();

        public MainWindow()
        {
            InitializeComponent();
            logic.loadStartingData();
            logic.Solve();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Crew crew = new Crew();
            var items = crew.crewList(logic.names, logic.attributes);
            var grid = sender as DataGrid;
            grid.ItemsSource = items;
        }

        private void DataGrid_Loaded2(object sender, RoutedEventArgs e)
        {
            RequiredCrew requiredCrew = new RequiredCrew();
            var items = requiredCrew.requiredCrewList(logic.required_crew);
            var grid = sender as DataGrid;
            grid.ItemsSource = items;
        }

        private void DataGrid_Loaded3(object sender, RoutedEventArgs e)
        {
            Flights flights = new Flights();
            logic.Solve();

            var items = flights.flightList(logic.flight_crew);

            var grid = sender as DataGrid;
            grid.ItemsSource = items;

        }

        private void DelCrew_Click(object sender, RoutedEventArgs e)
        {
            var number = textBox.Text;

            logic.names.RemoveAt(Convert.ToInt32(number) - 1);
            logic.attributes.RemoveAt(Convert.ToInt32(number) - 1);
            this.DataGrid_Loaded(DG1, new RoutedEventArgs());
        }

        private void AddCrew_Click(object sender, RoutedEventArgs e)
        {
            var listNewItem = this.DG1.Items;
            int length = listNewItem.Count;
            var newItem = listNewItem[length - 2] as Crew;
            logic.names.Add(newItem.Name);
            logic.attributes.Add(new int[] {newItem.Steward == "Yes" ? 1 : 0,
                                            newItem.Hostess == "Yes" ? 1 : 0,
                                            newItem.French == "Yes" ? 1 : 0,
                                            newItem.Spanish == "Yes" ? 1 : 0,
                                            newItem.German == "Yes" ? 1 : 0});


        }

        private void AddRequiredCrew_Click(object sender, RoutedEventArgs e)
        {

            var listNewItem = this.DG2.Items;
            int length = listNewItem.Count;
            var newItem = listNewItem[length - 2] as RequiredCrew;
            logic.required_crew.Add(new int[] {newItem.Staff,
                                            newItem.Steward,
                                            newItem.Hostess,
                                            newItem.French,
                                            newItem.Spanish,
                                            newItem.German});


        }

        private void DelRequiredCrew_Click(object sender, RoutedEventArgs e)
        {
            var number = textBox2.Text;
            logic.required_crew.RemoveAt(Convert.ToInt32(number) - 1);
            logic.num_flights = logic.required_crew.Count;
            this.DataGrid_Loaded2(DG2, new RoutedEventArgs());
        }

    }
}
