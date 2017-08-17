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
using System.Collections.ObjectModel;
using System.Collections;

namespace FlightPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //createdFlightRoutes is used to store the flights inputted by the user
        //cheapestRouteAvailable stores the cheapest path comprised of one or multiple flights
        //currentPath keeps a path and compares it to cheapestRoute. cheapestRoute changes when a cheaper route is found
        //repeatArrivals keeps paths from using the same point twice
        public ObservableCollection<FlightRoute> createdFlightRoutes;
        public ObservableCollection<FlightRoute> cheapestRouteAvailable;
        List<FlightRoute> currentPath;
        ArrayList repeatArrivals;
        List<FlightRoute> cheapestRoute;

        public MainWindow()
        {
            InitializeComponent();
            createdFlightRoutes = new ObservableCollection<FlightRoute>();
            cheapestRouteAvailable = new ObservableCollection<FlightRoute>();
            currentPath = new List<FlightRoute>();
            repeatArrivals = new ArrayList();
            cheapestRoute = new List<FlightRoute>();
            lstFlightRoutes.ItemsSource = createdFlightRoutes;
            lstCheapestRoute.ItemsSource = cheapestRouteAvailable;

        }

        private void btnAddFlight_Click(object sender, RoutedEventArgs e)
        {
            string flightTime = txtFlightTime.Text;
            string flightHour = flightTime.Substring(0, 2);
            string flightMinute = flightTime.Substring(3, 2);
            int hour = int.Parse(flightHour);
            int minute = int.Parse(flightMinute);
            double cost = double.Parse(txtFlightCost.Text);

            FlightRoute flightRoute = new FlightRoute
            {
                FlightDeparture = txtDeparture.Text,
                FlightArrival = txtArrival.Text,
                FlightDate = dteFlightDate.SelectedDate.Value.Date,
                FlightTime = flightTime,
                FlightHour = hour,
                FlightMinute = minute,
                FlightCost = cost
            };

            createdFlightRoutes.Add(flightRoute);
        }

        private void btnFindRoute_Click(object sender, RoutedEventArgs e)
        {
            string startingPoint = txtRouteStart.Text;
            string endingPoint = txtRouteEnd.Text;
            double lowestCost = 0.0;
            double currentPathCost = 0.0;
            currentPath.Clear();
            cheapestRoute.Clear();
            repeatArrivals.Clear();
            cheapestRouteAvailable.Clear();

            repeatArrivals.Add(startingPoint);
            
            for(int i=0;i<createdFlightRoutes.Count;i++)
            {
                if (createdFlightRoutes.ElementAt<FlightRoute>(i).FlightDeparture.Equals(startingPoint))
                {
                    currentPath.Add(createdFlightRoutes.ElementAt<FlightRoute>(i));

                    if (!createdFlightRoutes.ElementAt<FlightRoute>(i).FlightArrival.Equals(endingPoint))
                    {
                        findCheapestRoute(createdFlightRoutes.ElementAt<FlightRoute>(i).FlightArrival, endingPoint);
                    }

                    //endingPoint is checked against the arrival of the last FlightRoute in currentPath, equal paths are eligible to become the lowest
                    if (currentPath.Last<FlightRoute>().FlightArrival.Equals(endingPoint))
                    {
                        for (int j = 0; j < currentPath.Count; j++)
                        {
                            currentPathCost = currentPathCost + currentPath.ElementAt<FlightRoute>(j).FlightCost;
                        }

                        if (lowestCost == 0.0 || currentPathCost < lowestCost)
                        {
                            cheapestRoute.Clear();
                            cheapestRoute.AddRange(currentPath);
                            repeatArrivals.RemoveRange(1, repeatArrivals.Count - 1);
                            lowestCost = currentPathCost;
                            currentPathCost = 0.0;
                        }
                    }
                }
            }

            if (cheapestRoute.Count > 0)
            {
                for(int i = 0; i < cheapestRoute.Count; i++)
                {
                    TimeSpan layoverTime;

                    if (i > 0)
                    {
                        layoverTime = cheapestRoute.ElementAt<FlightRoute>(i).FlightDate.Subtract(cheapestRoute.ElementAt<FlightRoute>(i - 1).FlightDate);
                        cheapestRoute.ElementAt<FlightRoute>(i).LayoverToNextFlight = layoverTime;
                    }

                    cheapestRouteAvailable.Add(cheapestRoute.ElementAt<FlightRoute>(i));
                }
            }
        }

        //findCheapestRoute method is used recursively to find existing paths
        //repeatArrivals list is checked, paths are obsolete if starting point is already used
        //Dates of flights are checked against the last flight in the path - flights must take place after the last flight in the path
        private void findCheapestRoute(string startingPoint, string endingPoint)
        {
            if (repeatArrivals.Contains(startingPoint))
            {
                return;
            }

            repeatArrivals.Add(startingPoint);

            for (int i = 0; i < createdFlightRoutes.Count; i++)
            {
                if (createdFlightRoutes.ElementAt<FlightRoute>(i).FlightDeparture.Equals(startingPoint)
                    && (currentPath.Last<FlightRoute>().FlightDate < createdFlightRoutes.ElementAt<FlightRoute>(i).FlightDate
                    || currentPath.Last<FlightRoute>().FlightDate == createdFlightRoutes.ElementAt<FlightRoute>(i).FlightDate
                    && currentPath.Last<FlightRoute>().FlightHour < createdFlightRoutes.ElementAt<FlightRoute>(i).FlightHour))
                {
                    currentPath.Add(createdFlightRoutes.ElementAt<FlightRoute>(i));

                    if (!createdFlightRoutes.ElementAt<FlightRoute>(i).FlightArrival.Equals(endingPoint))
                    {
                        findCheapestRoute(createdFlightRoutes.ElementAt<FlightRoute>(i).FlightArrival, endingPoint);
                    }
                }
            }
            return;
        }

        private void btnRemoveFlight_Click(object sender, RoutedEventArgs e)
        {
            if (lstFlightRoutes.SelectedItems.Count > 0)
             createdFlightRoutes.RemoveAt(lstFlightRoutes.SelectedIndex);     
        }

        private void btnClearRoute_Click(object sender, RoutedEventArgs e)
        {
            cheapestRouteAvailable.Clear();
            currentPath.Clear();
            cheapestRoute.Clear();
            repeatArrivals.Clear();
        }
    }
}
