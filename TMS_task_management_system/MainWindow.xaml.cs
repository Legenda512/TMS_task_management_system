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
using System.Collections.ObjectModel;
using System.IO;

namespace TMS_task_management_system
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //переменная для подключения к базе данных
        ApplicationContext db = new ApplicationContext();

        public MainWindow()
        {
            InitializeComponent(); 
            var datacontext = new ApplicationViewModel();
            this.DataContext = datacontext;
            

            //Список задач с вложенной структурой
            List<Task> tasks = new List<Task>();

            //весь нас список задач
            var task_list = db.Tasks.ToList();
            foreach (var task in task_list)
            {
                // для нашей задачи ищем все подзадачи
                var subtask_list = task_list.Where(c => c.Ref_Task == task.Id_Task);
                    // каждую подзадачу добавляем в поле подзадачи у родительской задачи 
                    foreach (var sub_task in subtask_list)
                        task.SubTasks.Add(sub_task);
                tasks.Add(task);
            }


            trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
            datacontext.NotifyAddCommand += task =>
            {
                tasks.Add(task);
                trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
                trvTasks.Items.Refresh();
            };


            datacontext.NotifyEditCommand += task =>
            {
                var newtask = FindTask(tasks.Where(c => c.Ref_Task == 0), task.Id_Task);

                if (newtask != null)
                {
                    newtask.Name_Task = task.Name_Task;
                    newtask.Description_Task = task.Description_Task;
                    newtask.Performers_Task = task.Performers_Task;
                    newtask.Date_of_registration_Task = task.Date_of_registration_Task;
                    newtask.Status_Task = task.Status_Task;
                    newtask.Planned_time_Task = task.Planned_time_Task;
                    newtask.Actual_time_Task = task.Actual_time_Task;
                    newtask.Date_of_completion_Task = task.Date_of_completion_Task;

                    trvTasks.Items.Refresh();
                }
                
            };

            datacontext.NotifyDeleteCommand += task =>
            {
                DeleteTask(tasks, task.Id_Task);
                trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
                trvTasks.Items.Refresh();
            };


            datacontext.NotifyAddSubCommand += task =>
            {
                trvTasks.Items.Refresh();
            };


        }

        private static Task FindTask(IEnumerable<Task> tasks, int taskId)
        {
            Task foundTask = null;
            foreach (var task in tasks)
            {
                if (task.Id_Task == taskId)
                {
                    return task;
                }

                if (task.SubTasks.Count > 0)
                {
                    foundTask = FindTask(task.SubTasks, taskId);
                    if(foundTask != null)
                        return foundTask;
                }

            }

            return foundTask;
        }

        private static void DeleteTask(List<Task> tasks, int taskId)
        {
            foreach (var task in tasks)
            {
                if (task.Id_Task == taskId)
                {
                    tasks.Remove(task);
                    return;
                }

                if (task.SubTasks.Count > 0)
                {
                    DeleteTaskRecursive(task, taskId);
                      
                }

            }

        }

        private static void DeleteTaskRecursive(Task sourceTask, int taskId)
        {
            foreach (var task in sourceTask.SubTasks)
            {
                if (task.Id_Task == taskId)
                {
                    sourceTask.SubTasks.Remove(task);
                    return;
                }

                if (task.SubTasks.Count > 0) DeleteTaskRecursive(task, taskId);
            }
        }

            //private void tasks_Expanded(object sender, RoutedEventArgs e)
            //{
            //    //получаем элемент, для которого пытаемся раскрыть список
            //    TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //    //очищаем список, чтобы каждый раз обновлять содержимое
            //    item.Items.Clear();

            //    //получаем из элемента treeview его заголовок
            //    var name_task = item.Header;
            //    //находим объект класса Task в котором название задачи совпадаем с заголовком элемента treeview
            //    var taskParent = db.Tasks.Where(c => c.Name_Task == name_task.ToString());

            //    //проходимся по списку класса Task и будем находить подзадачи, которые ссылаются на задачу
            //    foreach (var parent in taskParent)
            //    {
            //        //находим список объектов Task у которых поле Ref_Task == Id_Task, т.е. ссылаются на задачу
            //        var taskchild_list = db.Tasks.Where(c => c.Ref_Task == parent.Id_Task);
            //        foreach (var taskchild in taskchild_list)
            //        {
            //            //создаем объект класса treeview
            //            TreeViewItem newItem = new TreeViewItem();
            //            //задаем ему заголовок, которым является имя задачи
            //            newItem.Header = taskchild.Name_Task;
            //            newItem.Items.Add("*");
            //            item.Items.Add(newItem);
            //        }
            //    }

            //}

        }
}
