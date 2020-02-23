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
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;


namespace TMS_task_management_system
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ApplicationViewModel();

            //db = new ApplicationContext();
            //db.Tasks.Load();
            //this.DataContext = db.Tasks.Local.ToBindingList();
        }

        //// добавление
        //private void Add_Click(object sender, RoutedEventArgs e)
        //{
        //    TaskWindow taskWindow = new TaskWindow(new Task());
        //    if (taskWindow.ShowDialog() == true)
        //    {
        //        Task task = taskWindow.Task;
        //        db.Tasks.Add(task);
        //        db.SaveChanges();
        //    }
        //}

        //// редактирование
        //private void Edit_Click(object sender, RoutedEventArgs e)
        //{
        //    // если ни одного объекта не выделено, выходим
        //    if (tasksList.SelectedItem == null) return;
        //    // получаем выделенный объект
        //    Task task = tasksList.SelectedItem as Task;

        //    TaskWindow taskWindow = new TaskWindow(new Task
        //    {
        //        Id_Task = task.Id_Task,
        //        Name_Task = task.Name_Task,
        //        Description_Task = task.Description_Task,
        //        Performers_Task = task.Performers_Task,
        //        Date_of_registration_Task = task.Date_of_registration_Task,
        //        Status_Task = task.Status_Task,
        //        Planned_time_Task = task.Planned_time_Task,
        //        Actual_time_Task = task.Actual_time_Task,
        //        Date_of_completion_Task = task.Date_of_completion_Task
        //    });

        //    if (taskWindow.ShowDialog() == true)
        //    {
        //        // получаем измененный объект
        //        task = db.Tasks.Find(taskWindow.Task.Id_Task);
        //        if (task != null)
        //        {
        //            task.Name_Task = taskWindow.Task.Name_Task;
        //            task.Description_Task = taskWindow.Task.Description_Task;
        //            task.Performers_Task = taskWindow.Task.Performers_Task;
        //            task.Date_of_registration_Task = taskWindow.Task.Date_of_registration_Task;
        //            task.Status_Task = taskWindow.Task.Status_Task;
        //            task.Planned_time_Task = taskWindow.Task.Planned_time_Task;
        //            task.Actual_time_Task = taskWindow.Task.Actual_time_Task;
        //            task.Date_of_completion_Task = taskWindow.Task.Date_of_completion_Task;

        //            db.Entry(task).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }
        //}
        //// удаление
        //private void Delete_Click(object sender, RoutedEventArgs e)
        //{
        //    // если ни одного объекта не выделено, выходим
        //    if (tasksList.SelectedItem == null) return;
        //    // получаем выделенный объект
        //    Task task = tasksList.SelectedItem as Task;
        //    db.Tasks.Remove(task);
        //    db.SaveChanges();
        //}


    }
}
