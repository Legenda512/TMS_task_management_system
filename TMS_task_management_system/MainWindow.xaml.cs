using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TMS_task_management_system
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //переменная для подключения к базе данных
        ApplicationContext db = new ApplicationContext();

        //Список задач с вложенной структурой
        public static List<Task> tasks = new List<Task>();

        public MainWindow()
        {
            InitializeComponent(); 

            var datacontext = new ApplicationViewModel();
            
            this.DataContext = datacontext;
            

            ////Список задач с вложенной структурой
            //List<Task> tasks = new List<Task>();

            //получаем весь список задач из бд
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

            //обновляем ItemsSource у TreeView при добавлении новой задачи
            trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
            datacontext.NotifyAddCommand += task =>
            {
                tasks.Add(task);
                trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
                trvTasks.Items.Refresh();
            };

            //обновляем ItemsSource у TreeView при редактировании задачи
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

            //обновляем ItemsSource у TreeView при удалении задачи
            datacontext.NotifyDeleteCommand += task =>
            {
                DeleteTask(tasks, task.Id_Task);
                trvTasks.ItemsSource = tasks.Where(c => c.Ref_Task == 0);
                trvTasks.Items.Refresh();
            };

            //обновляем ItemsSource у TreeView при добавлении подзадачи
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
  


    }
}
