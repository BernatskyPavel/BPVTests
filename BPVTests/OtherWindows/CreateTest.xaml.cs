using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BPVTests
{
    /// <summary>
    /// Логика взаимодействия для CreateTest.xaml
    /// </summary>
    public partial class CreateTest : Window
    {
        public CreateTest()
        {
            InitializeComponent();
            try
            {
                FillCat();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        void FillCat()
        {
            DataContext db = new DataContext(Resourses.connectionS);
            if (!db.DatabaseExists()) throw new Exception("Ошибка базы данных");
            Table<dbCats> table = db.GetTable<dbCats>();
            List<dbCats> list = table.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = list.ElementAt(i).CatName;
                item.Content = list.ElementAt(i).CatName;
                TestTheme.Items.Add(item);
            }
            db.Dispose();
        }
        private void AcceptCreate_Click(object sender, RoutedEventArgs e)
        {
            try{
                int TP, PR;
                bool def = true;
                if (TestName.Text == string.Empty) throw new Exception(App.Current.Resources["TestCreateProblem0"].ToString());
                if (TestTheme.Text == string.Empty) throw new Exception(App.Current.Resources["TestCreateProblem1"].ToString());
                if (QuestionNumber.Text.Length == 0) throw new Exception(App.Current.Resources["TestCreateProblem2"].ToString());
                else if (!Int32.TryParse(QuestionNumber.Text, out TP)) throw new Exception(App.Current.Resources["TestCreateProblem3"].ToString());
                     else if(Int32.Parse(QuestionNumber.Text)<1 || Int32.Parse(QuestionNumber.Text) >100) throw new Exception(App.Current.Resources["TestCreateProblem4"].ToString());
                if (Percent.Text.Length != 0)
                {
                    if (!Int32.TryParse(Percent.Text, out PR)) throw new Exception(App.Current.Resources["TestCreateProblem5"].ToString());
                    else if(!(PR>=0 && PR<=100)) throw new Exception(App.Current.Resources["TestCreateProblem6"].ToString());
                    def = false;
                }
                DataContext db = new DataContext(Resourses.connectionS);
                if(!db.DatabaseExists()) throw new Exception(App.Current.Resources["TestCreateProblem7"].ToString());
                IEnumerable<dbTest> check = db.ExecuteQuery<dbTest>("Select * from Test where TestName = {0} and Category = {1}", TestName.Text, TestTheme.Text);
                if(check.ToList().Count!=0) throw new Exception(App.Current.Resources["TestCreateProblem8"].ToString());
                int result = db.ExecuteCommand("Insert Test(TestName,QuestionCount,Category,MinSuccess) Values({0},{1},{2},{3})",TestName.Text, QuestionNumber.Text,TestTheme.Text, def? "40":Percent.Text);
                if (result != 0)
                    this.DialogResult = true;
                else this.DialogResult = false;
                db.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void CancelCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CreateNewCategory_Click(object sender, RoutedEventArgs e)
        {
            try{
                CreateCat cat = new CreateCat();
                cat.ShowDialog();
                switch (cat.DialogResult)
                {
                    case true:
                        TestTheme.Items.Clear();
                        FillCat();
                        break;
                    case false:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
    }
}
