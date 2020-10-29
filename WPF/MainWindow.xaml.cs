using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace CaptionSummaryCustomization
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(this.datagrid.View != null)
            {
                this.datagrid.View.BeginInit();
                ChangeRows();
                this.datagrid.View.EndInit();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void ChangeRows()
        {
            var dataContext = this.maingrid.DataContext as ViewModel;
            var employees = dataContext.Employees;
            var count = employees.Count;
            Random r = new Random();
            for (int i = 0; i < count; ++i)
            {
                int recNo = r.Next(employees.Count);
                double d = r.NextDouble();
                var salary = employees[recNo].EmployeeSalary;
                if (d < .5)
                    employees[recNo].EmployeeSalary +=100;
                else
                    employees[recNo].EmployeeSalary -= 100;
            }
        }
    }
}
