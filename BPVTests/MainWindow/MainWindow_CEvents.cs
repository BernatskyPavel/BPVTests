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
using Microsoft.Win32;

namespace BPVTests
{
    public partial class MainWindow
    {
        private void AddAnswer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FAddAnswer(data[Resourses.CurrentQuestionNum].Type, 0);
                data[Resourses.CurrentQuestionNum].Answs.Add(new AnswersData());
                if (data[Resourses.CurrentQuestionNum].Type == 2)
                {
                    data[Resourses.CurrentQuestionNum].Answs.ElementAt(data[Resourses.CurrentQuestionNum].ACount).Pos = data[Resourses.CurrentQuestionNum].ACount;
                }
                data[Resourses.CurrentQuestionNum].ACount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void SaveTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /* for (int i = 0; i < data.Length; i++)
                     if (data[i].Empty) throw new Exception("Не все вопросы заполнены!");*/
                CheckPossibleProblems();
                using (var tran = new TransactionScope())
                {
                    DataContext db = new DataContext(Resourses.connectionS);
                    for (int i = 0; i < data.Length; i++)
                    {
                        string sql = string.Empty;
                        if (data[i].images == null)
                        {
                            int result_q = db.ExecuteCommand("Insert Questions(QuestionText,QuestionDifficult,AnswersCount,QuestionType) values({0},{1},{2},{3})", data[i].Text, data[i].Difficult == -1 ? 2 : data[i].Difficult, data[i].ACount, data[i].Type);
                            if (result_q == 0) throw new Exception(App.Current.Resources["TestSaveError"].ToString());
                        }
                        else
                        {
                            int result_q = db.ExecuteCommand("Insert Questions(QuestionText,QuestionDifficult,AnswersCount,Picture,QuestionType) values({0},{1},{2},{3},{4})", data[i].Text, data[i].Difficult == -1 ? 2 : data[i].Difficult, data[i].ACount, data[i].images, data[i].Type);
                            if (result_q == 0) throw new Exception(App.Current.Resources["TestSaveError"].ToString());
                        }
                        Table<dbQuestion> table = db.GetTable<dbQuestion>();
                        var query = from el in table
                                    orderby el.QuestionID descending
                                    select el;
                        Resourses.CurrentQuestionID = query.ToList().ElementAt(0).QuestionID;
                        int result_t = db.ExecuteCommand("Insert Tests values({0},{1})", Resourses.CurrentTestID, Resourses.CurrentQuestionID);
                        if (result_t == 0) throw new Exception(App.Current.Resources["TestSaveError"].ToString());

                        for (int g = 0; g < data[i].ACount; g++)
                        {
                            string s = string.Empty;
                            if (data[i].Type == 2)
                            {
                                for (int z = 0; z < data[i].ACount; z++)
                                    s += data[i].Answs[z].Text;
                            }
                            int result_a = 0;
                            switch (data[i].Type)
                            {
                                case 0:
                                case 1:
                                case 5:
                                    result_a = db.ExecuteCommand("Insert Answers(QuestionID,AnswerText,TorF) values({0},{1},{2})", Resourses.CurrentQuestionID, data[i].Answs[g].Text, data[i].Answs[g].TorF);
                                    break;
                                case 2:
                                    result_a = db.ExecuteCommand("Insert Answers(QuestionID,AnswerText,TorF,DnD) values({0},{1},{2},{3})", Resourses.CurrentQuestionID, data[i].Answs[g].Text, data[i].Answs[g].TorF, s);
                                    break;
                                case 3:
                                case 4:
                                    result_a = db.ExecuteCommand("Insert Answers(QuestionID,TorF,Picture) values({0},{1},{2})", Resourses.CurrentQuestionID, data[i].Answs[g].TorF, data[i].Answs[g].images);
                                    break;
                                default: throw new Exception(App.Current.Resources["TestSaveError"].ToString());
                            }
                            if (result_a == 0) throw new Exception(App.Current.Resources["TestSaveError"].ToString());
                        }
                    }
                    tran.Complete();
                }
                CurrentTestSave = true;
                LoadTests();
                SaveTest.IsEnabled = false;
                Questions.IsEnabled = false;
                WorkPanel.IsEnabled = false;
            }
            catch (TransactionException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CurrentTestSave && TestON)
                {
                    DataContext db = new DataContext(Resourses.connectionS);
                    db.ExecuteCommand("Delete from Test Where TestID={0}", Resourses.CurrentTestID);
                    db.Dispose();
                }
                CreateTest create = new CreateTest();
                create.ShowDialog();
                if (create.DialogResult.Value)
                {
                    SaveTest.IsEnabled = true;
                    Questions.IsEnabled = true;
                    WorkPanel.IsEnabled = true;
                    WorkPanelMini.IsEnabled = false;
                    QuestionList.Children.Clear();
                    DataContext db = new DataContext(Resourses.connectionS);
                    Table<dbTest> table = db.GetTable<dbTest>();
                    var query = from el in table
                                orderby el.ID descending
                                select el;

                    AddQuestion(query.ToList().ElementAt(0).QuestionCount);
                    data = new QuestionsData[query.ToList().ElementAt(0).QuestionCount];

                    for (int i = 0; i < data.Length; i++)
                        data[i] = new QuestionsData();
                    //MessageBox.Show(data.Length.ToString());
                    Resourses.CurrentTestID = query.ToList().ElementAt(0).ID;
                    TestON = true;
                    WorkPanel.IsEnabled = true;
                    db.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void LabelQ_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                WorkPanelMini.IsEnabled = true;
                write = false;
                Answers.Children.Clear();
                QuestionText.Clear();
                write = true;
                Grid send = sender as Grid;
                //int x = QuestionList.Children.IndexOf(send);
                foreach (var x in QuestionList.Children)
                    (x as Grid).Background = Brushes.White;
                (sender as Grid).Background = Brushes.LightGray;
                Resourses.CurrentQuestionNum = QuestionList.Children.IndexOf(send);
                if (data[Resourses.CurrentQuestionNum].Empty)
                {
                    QuestionPicture.Source = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].image, UriKind.RelativeOrAbsolute));
                    QuestionPicture.Height = 32;
                    QuestionPicture.Width = 32;
                    TypeComboBox.SelectedIndex = -1;
                    DifficultComboBox.SelectedIndex = -1;
                    AddAnswer.IsEnabled = false;
                }
                else
                {
                    Resourses.ChangeBox = 1;
                    write = false;

                    if (data[Resourses.CurrentQuestionNum].DefaultImage)
                    {
                        QuestionPicture.Source = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].image, UriKind.RelativeOrAbsolute));
                        QuestionPicture.Height = 32;
                        QuestionPicture.Width = 32;
                    }
                    else
                    {
                        MemoryStream memoryStream = new MemoryStream(data[Resourses.CurrentQuestionNum].images);
                        QuestionPicture.Source = BitmapFrame.Create(memoryStream);
                        QuestionPicture.Height = 128;
                        QuestionPicture.Width = 128;
                    }
                    QuestionText.Text = data[Resourses.CurrentQuestionNum].Text;
                    write = true;
                    if (data[Resourses.CurrentQuestionNum].Type != -1)
                        AddAnswer.IsEnabled = true;
                    if (data[Resourses.CurrentQuestionNum].Type == 2)
                        AddAnswer.AllowDrop = true;
                    else
                        AddAnswer.AllowDrop = false;



                    TypeComboBox.SelectedIndex = data[Resourses.CurrentQuestionNum].Type;
                    DifficultComboBox.SelectedIndex = data[Resourses.CurrentQuestionNum].Difficult;
                    for (int i = 0; i < data[Resourses.CurrentQuestionNum].ACount; i++)
                    {
                        FAddAnswer(data[Resourses.CurrentQuestionNum].Type, 0);
                        Grid grid = Answers.Children[i] as Grid;
                        switch (data[Resourses.CurrentQuestionNum].Type)
                        {
                            case 0:
                                (grid.Children[0] as TextBox).Text = data[Resourses.CurrentQuestionNum].Answs[i].Text;
                                (grid.Children[1] as RadioButton).IsChecked = data[Resourses.CurrentQuestionNum].Answs[i].TorF;
                                break;
                            case 1:
                                (grid.Children[0] as TextBox).Text = data[Resourses.CurrentQuestionNum].Answs[i].Text;
                                (grid.Children[1] as CheckBox).IsChecked = data[Resourses.CurrentQuestionNum].Answs[i].TorF;
                                break;
                            case 2:
                                (grid.Children[0] as TextBox).Text = data[Resourses.CurrentQuestionNum].Answs[i].Text;
                                break;
                            case 3:
                                if (data[Resourses.CurrentQuestionNum].Answs[i].DefaultImage)
                                    (grid.Children[0] as Image).Source = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].Answs[i].image, UriKind.RelativeOrAbsolute));
                                else
                                {
                                    MemoryStream memoryStream = new MemoryStream(data[Resourses.CurrentQuestionNum].Answs[i].images);
                                    (grid.Children[0] as Image).Source = BitmapFrame.Create(memoryStream);
                                    //(grid.Children[0] as Image).Source  = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].Answs[i].image,UriKind.RelativeOrAbsolute));

                                    (grid.Children[0] as Image).Height = 128;
                                    (grid.Children[0] as Image).Width = 128;
                                }
                                //(grid.Children[0] as Image).MouseDown += ImageOpen_MouseDown;
                                (grid.Children[1] as RadioButton).IsChecked = data[Resourses.CurrentQuestionNum].Answs[i].TorF;
                                break;
                            case 4:
                                if (data[Resourses.CurrentQuestionNum].Answs[i].DefaultImage)
                                    (grid.Children[0] as Image).Source = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].Answs[i].image, UriKind.RelativeOrAbsolute));
                                else
                                {
                                    MemoryStream memoryStream = new MemoryStream(data[Resourses.CurrentQuestionNum].Answs[i].images);
                                    (grid.Children[0] as Image).Source = BitmapFrame.Create(memoryStream);
                                    //(grid.Children[0] as Image).Source  = new BitmapImage(new Uri(data[Resourses.CurrentQuestionNum].Answs[i].image,UriKind.RelativeOrAbsolute));

                                    (grid.Children[0] as Image).Height = 128;
                                    (grid.Children[0] as Image).Width = 128;
                                }
                                //(grid.Children[0] as Image).MouseDown += ImageOpen_MouseDown;
                                (grid.Children[1] as CheckBox).IsChecked = data[Resourses.CurrentQuestionNum].Answs[i].TorF;
                                break;
                            case 5:
                                (grid.Children[0] as TextBox).Text = data[Resourses.CurrentQuestionNum].Answs[i].Text;
                                break;
                        }
                    }
                }
                if (!QuestionForm.IsVisible) QuestionForm.Visibility = Visibility.Visible;
                if (!QuestionForm.IsEnabled) QuestionForm.IsEnabled = true;
                //AddAnswer.IsEnabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Answers.Children.Clear();
                ComboBox box = sender as ComboBox;
                if (!(box.SelectedIndex == -1) && Resourses.ChangeBox == -1)
                {
                    AddAnswer.IsEnabled = true;
                    data[Resourses.CurrentQuestionNum].Type = box.SelectedIndex;
                    data[Resourses.CurrentQuestionNum].Answs = new List<AnswersData>();
                    data[Resourses.CurrentQuestionNum].Answs.Add(new AnswersData());
                    data[Resourses.CurrentQuestionNum].ACount = data[Resourses.CurrentQuestionNum].Answs.Count;

                    FAddAnswer(box.SelectedIndex, 1);
                    if (box.SelectedIndex == 5)
                    {
                        AddAnswer.IsEnabled = false;
                        data[Resourses.CurrentQuestionNum].Answs[0].TorF = true;
                    }
                    if (box.SelectedIndex == 2)
                        Answers.AllowDrop = true;
                    else Answers.AllowDrop = false;
                    data[Resourses.CurrentQuestionNum].Empty = false;
                }
                Resourses.ChangeBox = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void QuestionText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (write)
                {
                    data[Resourses.CurrentQuestionNum].Text = (sender as TextBox).Text;
                    data[Resourses.CurrentQuestionNum].Empty = false;
                }
                write = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void DifficultComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            if (!(box.SelectedIndex == -1))
                try
                {
                    data[Resourses.CurrentQuestionNum].Difficult = box.SelectedIndex;
                    data[Resourses.CurrentQuestionNum].Empty = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
                }
        }
        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton x = sender as RadioButton;
                Grid xx = x.Parent as Grid;
                StackPanel xxx = xx.Parent as StackPanel;
                data[Resourses.CurrentQuestionNum].Answs.ForEach(t => t.TorF = false);
                data[Resourses.CurrentQuestionNum].Answs.ElementAt(xxx.Children.IndexOf(xx)).TorF = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void CB_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox x = sender as CheckBox;
                Grid xx = x.Parent as Grid;
                StackPanel xxx = xx.Parent as StackPanel;
                data[Resourses.CurrentQuestionNum].Answs.ElementAt(xxx.Children.IndexOf(xx)).TorF = x.IsChecked.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox x = sender as TextBox;
                Grid xx = x.Parent as Grid;
                StackPanel xxx = xx.Parent as StackPanel;
                data[Resourses.CurrentQuestionNum].Answs.ElementAt(xxx.Children.IndexOf(xx)).Text = x.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void ImageOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                file.DefaultExt = ".png";
                file.Filter = "Изображение(.png,.jpg,.jpeg)|*.png; *.jpg; *jpeg;";

                if (file.ShowDialog() == true)
                {
                    if ((sender as Image).Name == "QuestionPicture")
                    {
                        data[Resourses.CurrentQuestionNum].DefaultImage = false;
                        (sender as Image).Source = new BitmapImage(new Uri(file.FileName, UriKind.RelativeOrAbsolute));
                        (sender as Image).Height = 128;
                        (sender as Image).Width = 128;
                        System.Drawing.Image image = System.Drawing.Image.FromFile(file.FileName);
                        MemoryStream memoryStream = new MemoryStream();
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] b = memoryStream.ToArray();
                        data[Resourses.CurrentQuestionNum].images = b;
                        data[Resourses.CurrentQuestionNum].Empty = false;
                    }
                    else
                    {
                        Image send = sender as Image;
                        int num = Answers.Children.IndexOf((send.Parent as Grid));
                        data[Resourses.CurrentQuestionNum].Answs.ElementAt(num).DefaultImage = false;
                        (sender as Image).Source = new BitmapImage(new Uri(file.FileName, UriKind.RelativeOrAbsolute));
                        (sender as Image).Height = 128;
                        (sender as Image).Width = 128;
                        data[Resourses.CurrentQuestionNum].Answs.ElementAt(num).image = file.FileName;
                        System.Drawing.Image image = System.Drawing.Image.FromFile(file.FileName);
                        MemoryStream memoryStream = new MemoryStream();
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] b = memoryStream.ToArray();
                        data[Resourses.CurrentQuestionNum].Answs.ElementAt(num).images = b;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Current.Resources["Error"].ToString());
            }
        }
        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("UIElement"))
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("UIElement"))
                {
                    UIElement droptarget = e.Source as UIElement;
                    int droptargetIndex = -1, i = 0;
                    foreach (UIElement element in this.Answers.Children)
                    {
                        if (element.Equals(droptarget))
                        {
                            droptargetIndex = i;
                            break;
                        }
                        i++;
                    }
                    if (droptargetIndex != -1)
                    {
                        this.Answers.Children.Remove(_realDragSource);
                        this.Answers.Children.Insert(droptargetIndex, _realDragSource);
                    }
                    _isDown = false;
                    _isDragging = false;
                    _realDragSource.ReleaseMouseCapture();
                    saveDnD();
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.Source == this.Answers)
                {
                }
                else
                {
                    _isDown = true;
                    _startPoint = e.GetPosition(this.Answers);
                }
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }
            catch (Exception)
            { }
        }
        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isDown)
                {
                    if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.Answers).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.Answers).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    {
                        _isDragging = true;
                        _realDragSource = e.Source as UIElement;
                        _realDragSource.CaptureMouse();
                        DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}
