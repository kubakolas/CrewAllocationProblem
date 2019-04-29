using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public string Name { get; set; }
    public string Steward { get; set; }
    public string Hostess { get; set; }
    public string French { get; set; }
    public string Spanish { get; set; }
    public string German { get; set; }

    public Crew(string name, string steward, string hostess, string french, string spanish, string german)
    {
        Name = name;
        Steward = steward;
        Hostess = hostess;
        French = french;
        Spanish = spanish;
        German = german;
    }
}

class RequiredCrew
{
    public RequiredCrew(string staff, string steward, string hostess, string french, string spanish, string german)
    {
        Staff = staff;
        Steward = steward;
        Hostess = hostess;
        French = french;
        Spanish = spanish;
        German = german;
    }

    public string Staff { get; set; }
    public string Steward { get; set; }
    public string Hostess { get; set; }
    public string French { get; set; }
    public string Spanish { get; set; }
    public string German { get; set; }

   
}

class Flights
{
    public Flights(string staff, string steward, string hostess, string french, string spanish, string german)
    {
        Staff = staff;
        Steward = steward;
        Hostess = hostess;
        French = french;
        Spanish = spanish;
        German = german;
    }

    public string Staff { get; set; }
    public string Steward { get; set; }
    public string Hostess { get; set; }
    public string French { get; set; }
    public string Spanish { get; set; }
    public string German { get; set; }


}

namespace CrewAllocationProblem
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Create a List of objects.
            var items = new List<Crew>();
            items.Add(new Crew("Tom", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("David", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Jeremy", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Joe", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Bill", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Fred", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Bod", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Mario", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("Ed", "Yes", "No", "Yes", "No", "Yes"));
            items.Add(new Crew("", "", "", "", "", ""));

            // ... Assign ItemsSource of DataGrid.
            var grid = sender as DataGrid;
            grid.ItemsSource = items;
        }

        private void DataGrid_Loaded2(object sender, RoutedEventArgs e)
        {
            // ... Create a List of objects.
            var items = new List<RequiredCrew>();
            items.Add(new RequiredCrew("4", "1", "1", "1", "1", "1"));
            items.Add(new RequiredCrew("5", "1", "1", "1", "1", "1"));
            items.Add(new RequiredCrew("5", "1", "1", "1", "1", "1"));
            items.Add(new RequiredCrew("6", "2", "2", "1", "1", "1"));
            items.Add(new RequiredCrew("7", "3", "3", "1", "1", "1"));
            items.Add(new RequiredCrew("4", "1", "1", "1", "1", "1"));
            items.Add(new RequiredCrew("", "", "", "", "", ""));

            // ... Assign ItemsSource of DataGrid.
            var grid = sender as DataGrid;
            grid.ItemsSource = items;
        }

        private void DataGrid_Loaded3(object sender, RoutedEventArgs e)
        {
            // ... Create a List of objects.
            var items = new List<Flights>();
            items.Add(new Flights("Tom","David","Jeremy","Ron","",""));
            items.Add(new Flights("Tom", "David", "Jeremy", "Ron", "Joe", "Bill"));
            items.Add(new Flights("Tom", "David", "Jeremy", "", "", ""));
            items.Add(new Flights("Tom", "David", "Jeremy", "Ron", "Joe", "Bill"));
            items.Add(new Flights("Tom", "David", "Jeremy", "Ron", "Joe", ""));
            items.Add(new Flights("Tom", "David", "Jeremy", "Ron", "Joe", "Bill"));
            items.Add(new Flights("Tom", "David", "Jeremy", "Ron", "", ""));

            // ... Assign ItemsSource of DataGrid.
            var grid = sender as DataGrid;
            grid.ItemsSource = items;
        }
    }
}
