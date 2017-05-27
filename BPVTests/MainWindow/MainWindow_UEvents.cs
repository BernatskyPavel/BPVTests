using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.Linq;
using System.Data.Design;
using System.Data.Entity;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Transactions;

namespace BPVTests
{
    partial class MainWindow
    {
        private void Test_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Label x = sender as Label;
                //Resourses.CurrentUserTestName = x.Content.ToString();
                TestName.Content = x.Content;
                TestName.Tag = 1;

                TreeViewItem y = x.Parent as TreeViewItem;
                //Resourses.CurrentUserCategoryName = y.Header.ToString();
                TestCategory.Content = y.Header;
                TestCategory.Tag = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void TestCat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItem x = sender as TreeViewItem;
                //Resourses.CurrentUserTestName = string.Empty;
                TestName.Content = string.Empty;
                TestName.Tag = 0;

                //Resourses.CurrentUserCategoryName = x.Header.ToString();
                TestCategory.Content = x.Header;
                TestCategory.Tag = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestName.Tag.ToString() == "0" || TestCategory.Tag.ToString() == "0") throw new Exception("Тест не выбран!");
                if (ResultFormat.SelectedIndex == -1) throw new Exception("Не выбран формат результата теста!");
                else Resourses.FormatID = ResultFormat.SelectedIndex;
                if (FisherYates.IsChecked.Value) Resourses.FisherA = true;
                else Resourses.FisherA = false;
                if (FisherYatesQuestion.IsChecked.Value) Resourses.FisherQ = true;
                else Resourses.FisherQ = false;
                if (StrongQuestions.IsChecked.Value) Resourses.StrongQ = true;
                else Resourses.StrongQ = false;
                if (DifficultQuestions.IsChecked.Value) Resourses.DifficultQ = true;
                else Resourses.DifficultQ = false;
                if (Timer.IsChecked.Value)
                {
                    Resourses.TimeQ = true;
                    Resourses.TimeP = TimerPick.SelectedTime;
                }
                else Resourses.TimeQ = false;

                DataContext db = new DataContext(Resourses.connectionS);
                IEnumerable<dbTest> testr = db.ExecuteQuery<dbTest>("Select * from Test Where TestName = {0} and Category = {1}", TestName.Content.ToString(), TestCategory.Content.ToString());
                List<dbTest> test = testr.ToList();
                int TID = test[0].ID;
                Resourses.Min = test[0].MinSuccess;

                IEnumerable<dbTests> Tests = db.ExecuteQuery<dbTests>("Select QuestionID from Tests where TestID = {0}", TID);
                List<dbTests> listTests = Tests.ToList();
                List<int> QIDs = new List<int>();
                foreach (var x in listTests)
                    QIDs.Add(x.QuestionID);
                List<dbQuestion> questions = new List<dbQuestion>();

                foreach (var qid in QIDs)
                {
                    IEnumerable<dbQuestion> qu = db.ExecuteQuery<dbQuestion>("Select * from Questions where QuestionID = {0}", qid);
                    questions.AddRange(qu);
                }
                Resourses.CurrentTestUser = questions;

                Test newTest = new Test(QIDs.Count);
                newTest.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestName.Tag.ToString() == "0" || TestCategory.Tag.ToString() == "0") throw new Exception(App.Current.Resources["TestSelectProblem"].ToString());
                DataContext db = new DataContext(Resourses.connectionS);
                IEnumerable<dbTest> testr = db.ExecuteQuery<dbTest>("Select * from Test Where TestName = {0} and Category = {1}", TestName.Content.ToString(), TestCategory.Content.ToString());
                List<dbTest> test = testr.ToList();
                int TID = test[0].ID;


                IEnumerable<dbTests> Tests = db.ExecuteQuery<dbTests>("Select QuestionID from Tests where TestID = {0}", TID);
                List<dbTests> listTests = Tests.ToList();
                List<int> QIDs = new List<int>();
                using (var tran = new TransactionScope())
                {
                    int TestsDelete = db.ExecuteCommand("Delete Tests where TestID = {0}", TID);
                    foreach (var x in listTests)
                    {
                        int ADelete = db.ExecuteCommand("Delete Answers where QuestionID = {0}", x.QuestionID);
                        int QDelete = db.ExecuteCommand("Delete Questions where QuestionID = {0}", x.QuestionID);
                    }
                    //int TestsDelete = db.ExecuteCommand("Delete Tests where TestID = {0}", TID);
                    int TestDelete = db.ExecuteCommand("Delete Test where TestID = {0}", TID);
                    tran.Complete();
                }
                LoadTests();

                TestName.Content = string.Empty;
                TestName.Tag = 0;

                TestCategory.Content = string.Empty;
                TestCategory.Tag = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
    }
}
