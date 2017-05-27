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
using System.Windows.Shapes;
using System.Data.Linq;

namespace BPVTests
{
    /// <summary>
    /// Логика взаимодействия для CreateCat.xaml
    /// </summary>
    public partial class CreateCat : Window
    {
        public CreateCat()
        {
            InitializeComponent();
        }
        private void CreateCatB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool exist;
                if (NewCatName.Text.Length == 0) throw new Exception(App.Current.Resources["CreateCatProblem0"].ToString());
                DataContext db = new DataContext(Resourses.connectionS);
                Table<dbCats> table = db.GetTable<dbCats>();
                List<dbCats> list = table.ToList();
                exist = list.All(u => u.CatName != NewCatName.Text);
                if (exist){
                    db.ExecuteCommand("Insert Categories Values({0})", NewCatName.Text);
                    DialogResult = true;
                    db.Dispose();
                    this.Close();
                }
                else throw new Exception(App.Current.Resources["CreateCatProblem1"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }

        }
        private void CancelCat_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
