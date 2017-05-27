using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.Data.Design;
using System.Data.Entity;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Transactions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Converters;
using Microsoft.SqlServer.Types;
using BPVTests.DataClasses;

namespace BPVTests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        bool CurrentTestSave = false;
        bool TestON = false;
        private bool write = true;
        QuestionsData[] data;
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();
        public MainWindow()
        {
            InitializeComponent();               
        }
        public MainWindow(bool key)
        {
            InitializeComponent();
            if (key)
            {
                UserItem.Focus();
                CreatorItem.Focusable = false;
                DeleteTest.IsEnabled = false;
            }
            else {
                QuestionList.Focusable = false;
            }
            SaveTest.IsEnabled = false;
            Questions.IsEnabled = false;
            WorkPanel.IsEnabled = false;

            
        }
        private void AddQuestion(int Count)
        {
            for (int x = 0; x < Count; x++)
            {
                Grid newQuestion = new Grid();
                newQuestion.Background = Brushes.White;
                Label name = new Label();
                name.HorizontalContentAlignment = HorizontalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Center;
                name.Content = App.Current.Resources["EveryQuestion"].ToString()+ " №" + (x+1).ToString();
                newQuestion.Children.Add(name);
                newQuestion.MouseLeftButtonDown += LabelQ_MouseDown;
                QuestionList.Children.Add(newQuestion);
            }
            
        }
        private void LoadTests()
        {
            try
            {
                TestsTree.Items.Clear();
                DataContext db = new DataContext(Resourses.connectionS);
                Table<dbCats> tableCat = db.GetTable<dbCats>();
                List<dbCats> listCat = tableCat.ToList();
                for (int i = 0; i < listCat.Count; i++)
                {
                    TreeViewItem cat = new TreeViewItem();
                    cat.Header = listCat.ElementAt(i).CatName;
                    cat.PreviewMouseDown += TestCat_MouseDown;
                    
                    IEnumerable<dbTest> tableT = db.ExecuteQuery<dbTest>("select * from Test where Category = {0}", listCat.ElementAt(i).CatName);
                    List<dbTest> listT = tableT.ToList();
                    for (int k = 0; k < listT.Count; k++)
                    {
                        Label newT = new Label();
                        newT.MouseDown += Test_MouseDown;
                        newT.Content = listT.ElementAt(k).TestName;
                        cat.Items.Add(newT);
                    }
                    TestsTree.Items.Add(cat);
                }
                db.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void FAddAnswer(int type, int ForN)
        {
            try
            {
                if (ForN == 1)
                    Answers.Children.Clear();
                Grid x = new Grid();
                x.ColumnDefinitions.Add(new ColumnDefinition());
                x.ColumnDefinitions.Add(new ColumnDefinition());
                x.RowDefinitions.Add(new RowDefinition());
                x.RowDefinitions.Add(new RowDefinition());
                x.RowDefinitions.ElementAt(1).MinHeight = 30;
                x.Background = Brushes.LightGray;
                TextBox text = new TextBox();
                text.Height = 25; text.Width = 250;
                text.Background = Brushes.Wheat;
                text.TextChanged += TB_TextChanged;
                RadioButton radio = new RadioButton();
                radio.HorizontalAlignment = HorizontalAlignment.Center;
                radio.VerticalAlignment = VerticalAlignment.Center;
                radio.GroupName = "Answer" + Resourses.CurrentQuestionNum;
                radio.Checked += RB_Checked;
                CheckBox check = new CheckBox();
                check.HorizontalAlignment = HorizontalAlignment.Center;
                check.VerticalAlignment = VerticalAlignment.Center;
                check.Checked += CB_Checked;
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Icon/add.png", UriKind.Absolute));
                image.Height = 32; image.Width = 32;
                image.MouseDown += ImageOpen_MouseDown;
                // image.SourceUpdated += I_SourceUpdated;
                Grid.SetColumn(text, 0); Grid.SetColumn(radio, 1);
                Grid.SetColumn(check, 1); Grid.SetColumn(image, 0);
                switch (type)
                {
                    case 0:
                        x.Children.Add(text); x.Children.Add(radio);
                        Answers.Children.Add(x);
                        break;
                    case 1:
                        x.Children.Add(text); x.Children.Add(check);
                        Answers.Children.Add(x);
                        break;
                    case 2:
                        x.Children.Add(text);
                        Answers.Children.Add(x);
                        break;
                    case 3:
                        x.Children.Add(image); x.Children.Add(radio);
                        Answers.Children.Add(x);
                        break;
                    case 4:
                        x.Children.Add(image); x.Children.Add(check);
                        Answers.Children.Add(x);
                        break;
                    case 5:
                        x.Children.Add(text);
                        Answers.Children.Add(x);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void CheckPossibleProblems()
        {
            try
            {
                if (Resourses.CurrentTestID == -1) throw new Exception(App.Current.Resources["TestCreateProblem"].ToString());
                int QuestionProblems = 0,QuestionTypeProblems=0, AnswersTextProblems = 0,AnswersPictureProblems=0,AnswersTorFProblems=0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Text == string.Empty) QuestionProblems++;
                    if (data[i].Type == -1) QuestionTypeProblems++;
                    switch (data[i].Type)
                    {
                        case 0:
                            bool problem = true;
                            for (int g = 0; g < data[i].ACount; g++)
                            {
                                if (data[i].Answs[g].Text == string.Empty) AnswersTextProblems++;
                                if (data[i].Answs[g].TorF) problem = false;
                            }
                            if (problem) AnswersTorFProblems++;
                            break;
                        case 1:
                            for (int g = 0; g < data[i].ACount; g++)
                                if (data[i].Answs[g].Text == string.Empty) AnswersTextProblems++;
                            break;
                        case 2:
                            for (int g = 0; g < data[i].ACount; g++)
                                if (data[i].Answs[g].Text == string.Empty) AnswersTextProblems++;
                            break;
                        case 3:
                            bool problemp = true;
                            for (int g = 0; g < data[i].ACount; g++)
                            {
                                if (data[i].Answs[g].DefaultImage == true) AnswersPictureProblems++;
                                if (data[i].Answs[g].TorF) problemp = false;
                            }
                            if (problemp) AnswersTorFProblems++;
                            break;
                        case 4:
                            for (int g = 0; g < data[i].ACount; g++)
                                if (data[i].Answs[g].DefaultImage == true) AnswersPictureProblems++;
                            break;
                        case 5:
                            for (int g = 0; g < data[i].ACount; g++)
                                if (data[i].Answs[g].Text == string.Empty) AnswersTextProblems++;
                            break;
                    }
                }
                string problems = string.Empty;
                if (QuestionProblems != 0) problems += App.Current.Resources["TestSaveProblem0"].ToString() + QuestionProblems+ '\n';
                if (QuestionTypeProblems != 0) problems += App.Current.Resources["TestSaveProblem1"].ToString() + QuestionTypeProblems + '\n';
                if (AnswersTextProblems != 0) problems += App.Current.Resources["TestSaveProblem2"].ToString() + AnswersTextProblems + '\n';
                if (AnswersTorFProblems != 0) problems += App.Current.Resources["TestSaveProblem3"].ToString() + AnswersTorFProblems + '\n';
                if (AnswersPictureProblems != 0) problems += App.Current.Resources["TestSaveProblem4"].ToString() + AnswersPictureProblems + '\n';

                if (problems != string.Empty) throw new Exception(problems);

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void saveDnD()
        {
            try
            {
                for (int i = 0; i < data[Resourses.CurrentQuestionNum].ACount; i++)
                    data[Resourses.CurrentQuestionNum].Answs[i].Text = ((Answers.Children[i] as Grid).Children[0] as TextBox).Text;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            Auth newAuth = new Auth();
            if (!CurrentTestSave && TestON)
            {
                MessageBoxButton but = MessageBoxButton.OKCancel;
                MessageBoxImage res = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(App.Current.Resources["TestSaveWarn"].ToString(), App.Current.Resources["Error"].ToString(), but, res);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        DataContext db = new DataContext(Resourses.connectionS);
                        db.ExecuteCommand("Delete from Test Where TestID={0}", Resourses.CurrentTestID);
                        db.Dispose();
                        CurrentTestSave = true;
                        newAuth.Show();
                        this.Close();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else {
                newAuth.Show();
                this.Close();
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About newAbout = new About();
            newAbout.Show();
        }
        private void Options_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = (sender as MenuItem).Tag.ToString();
                App.Current.Resources.MergedDictionaries[0].Source = new Uri(String.Format("pack://application:,,,/Localisation/Language.{0}.xaml", s));
                CFG.Default.Lang = s;
                CFG.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!CurrentTestSave && TestON)
                {
                    MessageBoxButton but = MessageBoxButton.OKCancel;
                    MessageBoxImage res = MessageBoxImage.Warning;
                    MessageBoxResult result = MessageBox.Show(App.Current.Resources["TestSaveWarn"].ToString(), App.Current.Resources["Error"].ToString(), but, res);
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            DataContext db = new DataContext(Resourses.connectionS);
                            db.ExecuteCommand("Delete from Test Where TestID={0}", Resourses.CurrentTestID);
                            db.Dispose();
                            break;
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void UserItem_Loaded(object sender, RoutedEventArgs e)
        {
            //TestsTree;
            LoadTests();
        }
    }
}
