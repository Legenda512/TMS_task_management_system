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
            this.DataContext = new ApplicationViewModel();

            //создание объекта treeview программно
            TreeView treeview = new TreeView();
            treeview.ClearValue(ItemsControl.ItemsSourceProperty);
            //treeview.ItemsSource = "Tasks";

            layoutGrid.Children.Add(treeview);

            //получаем список задач, которые ни на кого не ссылаются,т.е. являются заглавными
            var task_list = db.Tasks.Where(c => c.Ref_Task == 0);

            //проходим по списку задач и выводим их в treeview
            foreach (var task in task_list)
            {
                //создаем объект класса treeview
                TreeViewItem item = new TreeViewItem();
                item.Expanded += tasks_Expanded;
                //задаем ему заголовок, которым является имя задачи
                item.Header = task.Name_Task;
                item.Items.Add("*");
                treeview.Items.Add(item);
            }


        }
        private void tasks_Expanded(object sender, RoutedEventArgs e)
        {
            //получаем элемент, для которого пытаемся раскрыть список
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            //очищаем список, чтобы каждый раз обновлять содержимое
            item.Items.Clear();

            //получаем из элемента treeview его заголовок
            var name_task = item.Header;
            //находим объект класса Task в котором название задачи совпадаем с заголовком элемента treeview
            var taskParent = db.Tasks.Where(c => c.Name_Task == name_task.ToString());

            //проходимся по списку класса Task и будем находить подзадачи, которые ссылаются на задачу
            foreach (var parent in taskParent)
            {
                //находим список объектов Task у которых поле Ref_Task == Id_Task, т.е. ссылаются на задачу
                var taskchild_list = db.Tasks.Where(c => c.Ref_Task == parent.Id_Task);
                foreach (var taskchild in taskchild_list)
                {
                    //создаем объект класса treeview
                    TreeViewItem newItem = new TreeViewItem();
                    //задаем ему заголовок, которым является имя задачи
                    newItem.Header = taskchild.Name_Task;
                    newItem.Items.Add("*");
                    item.Items.Add(newItem);
                }
            }

        }

    }
}
