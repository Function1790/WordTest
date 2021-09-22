using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WordTest;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //클래스

        //패널 설정
        class Page
        {
            public List<Grid> page = new List<Grid>();
            public List<Line> line = new List<Line>();
            public TextBlock button_left=new TextBlock();
            public TextBlock button_right = new TextBlock();
            public Label page_num = new Label();
            int selectNum = 0;

            //참조 전용
            public Page(){ }

            //참조 및 설정
            public Page(Grid panel,Word[] word, bool eng, bool kor)
            {
                int count = (int)(word.Length / 10);
                int ct = (int)(word.Length / 10);
                if (word.Length % 10 != 0)
                {
                    count++;
                }

                for (int i=0; i<count; i++)
                {
                    int ch = -70;
                    page.Add(new Grid());
                    page[i].Width = panel.Width;
                    page[i].Height = panel.Height + ch;
                    page[i].Margin = new Thickness(0, ch, 0, 0);
                    panel.Children.Add(page[i]);
                }

                for (int i = 0; i < word.Length; i++)
                {
                    line.Add(new Line());
                    line[i].Set(word[i]);
                    line[i].Eng.Focusable = eng;
                    line[i].Kor.Focusable = kor;

                    if (eng == false)
                    {
                        line[i].Eng.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    }
                    if (kor == false)
                    {
                        line[i].Kor.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                    }
                }

                for (int i = 0; i < ct; i++)
                {
                    for(int j=0; j<10; j++)
                    {
                        int n = j + i * 10;
                        page[i].Children.Add(line[n]);
                        page[i].Visibility = Visibility.Hidden;
                        line[n].Margin = new Thickness(-150, -650 + (j * (line[n].Height + 70)), 0, 0);
                    }
                }

                for(int i=0; i< word.Length % 10; i++)
                {
                    int n = i + ct * 10;
                    page[ct].Children.Add(line[n]);
                    page[ct].Visibility = Visibility.Hidden;
                    line[n].Margin = new Thickness(-150, -650 + (i * (line[n].Height + 70)), 0, 0);
                }

                page[selectNum].Visibility = Visibility.Visible;

                button_left.FontSize = 30;
                button_left.Text = "◀";
                button_left.Width = 50;
                button_left.MouseDown += new MouseButtonEventHandler(LeftMouseDown);
                button_left.MouseEnter += new MouseEventHandler(TextBlock_MouseEnter);
                button_left.MouseLeave += new MouseEventHandler(TextBlock_MouseLeave);
                button_right.FontSize = 30;
                button_right.Text = "▶";
                button_right.Width = 50;  
                button_right.MouseDown += new MouseButtonEventHandler(RightMouseDown);
                button_right.MouseEnter += new MouseEventHandler(TextBlock_MouseEnter);
                button_right.MouseLeave += new MouseEventHandler(TextBlock_MouseLeave);
                page_num.Content = "0";
                page_num.FontSize = 35;
                page_num.Width = 70;
                page_num.Foreground = new SolidColorBrush(Colors.White);
                int half = (int)(panel.Width / 2);
                int left = 400;
                int top = 650;
                button_left.Margin = new Thickness(half - left - 450, top + 5, 0, 0);
                button_right.Margin = new Thickness(half - left - 90, top + 5, 0, 0);
                page_num.Margin = new Thickness(half - left - 250, top, 0, 0);

                panel.Children.Add(button_left);
                panel.Children.Add(button_right);
                panel.Children.Add(page_num);
            }

            private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
            {
                (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(193, 211, 249));
            }

            private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
            {
                (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

            //페이지 추가
            public void addPageNum(int add)
            {
                if (selectNum + add < page.Count && selectNum + add >= 0) 
                {
                    page[selectNum].Visibility = Visibility.Hidden;
                    selectNum += add;
                    page_num.Content = selectNum;
                    page[selectNum].Visibility = Visibility.Visible;
                }
            }

            void LeftMouseDown(object sender, MouseButtonEventArgs e)
            {
                addPageNum(-1);
            }

            void RightMouseDown(object sender, MouseButtonEventArgs e)
            {
                addPageNum(1);
            }
        }

        //파티클
        class Particle
        {
            public Rectangle rect = new Rectangle();
            public Point vector;
            public Duration duration;
            
            public Particle(Point minV, Point maxV, int minD, int maxD)
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(30, 120, 200, 230));
                rect.RadiusX = 50;
                rect.RadiusY = 50;
                rect.Width = 50;
                rect.Height = 50;
                Random r = new Random();
                vector = new Point(r.Next((int)minV.X, (int)maxV.X), r.Next((int)minV.Y, (int)maxV.Y));
                duration = new Duration(TimeSpan.FromSeconds(r.Next(minD, maxD) * 0.01));
            }
        }

        //변수
        public static int Size_Word = 22;
        bool is_Open = true;
        Grid Before_Panel=new Grid();
        Grid Now_Panel = new Grid();
        Page _Page_Test;
        Page _Page_Random;
        Page _Page_Result;
        Page _Page_Input;
        Word[] _Word_Main = new Word[Size_Word];
        Word[] _Word_Random = new Word[Size_Word];
        public static string[] path = { "WDeng.txt", "WDkor.txt", "WDinfo.txt" };
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer particleT = new DispatcherTimer();
        int[] time = { 0, 0, 0 };
        Particle[] particle = new Particle[50];
        int pCnt = 0;
        Point partP;
        List<alertBox> alerts = new List<alertBox>();

        //함수

        //모두 리셋
        void delPage(Grid grid,Page page)
        {
            for(int i=0; i < page.page.Count; i++)
            {
                try
                {
                    grid.Children.Remove(page.page[i]);
                }
                catch (Exception)
                {
                    alert("delPage Exception");
                }
            }
            grid.Children.Remove(page.page_num);
            grid.Children.Remove(page.button_left);
            grid.Children.Remove(page.button_right);
        }

        void ResetEveryThing()
        {
            Log("ResetEveryThing", "entry");

            delPage(_Panel_Test, _Page_Test);
            delPage(_Panel_Random, _Page_Random);
            delPage(_Panel_Input, _Page_Input);
            delPage(_Panel_Result, _Page_Result);

            Word[] before = copyWord(_Word_Main);
            _Word_Main = cutWord(before);
            _Word_Random = cutWord(before);

            _Page_Result = new Page(_Panel_Result, _Word_Main, false, false);
            _Page_Result.line[0].Width = 750;
            _Page_Test = new Page(_Panel_Test, _Word_Main, false, true);
            _Page_Random = new Page(_Panel_Random, _Word_Random, false, true);
            _Page_Input = new Page(_Panel_Input, _Word_Main, true, true);

            setAllPage();
            Log("ResetEveryThing", "complete");
        }

        //단어 전체 복사
        Word[] copyWord(Word[] word)
        {
            Word[] result = new Word[word.Length];
            for(int i = 0; i<word.Length; i++)
            {
                result[i] = word[i].deepCopy();
            }
            return result;
        }

        //단어 추가/감소
        Word[] cutWord(Word[] word)
        {
            Word[] result = new Word[Size_Word];
            for(int i=0; i<Size_Word; i++)
            {
                try
                {
                    result[i] = word[i].deepCopy();
                }
                catch (Exception)
                {
                    result[i] = new Word("e" + i, "k" + i);
                }
            }
            return result;
        }

        //문자열 로그
        void Log(string title, string content)
        {
            Console.WriteLine($"[{title}] : {content}");
        }

        //자료형 로그
        void Log(string title, object content)
        {
            Console.WriteLine($"[{title}] : {content}");
        }

        //알림
        public void alert(string text)
        {
            List<int> del_num = new List<int>();
            while (true)
            {
                bool exit = true;
                for (int i = 0; i < alerts.Count; i++)
                {
                    if (alerts[i].hide == true)
                    {
                        del_num.Add(i);
                        _Panel_Alert.Children.Remove(alerts[i]);
                        alerts.RemoveAt(del_num[i]);
                        exit = false;
                        break;
                    }
                }
                if (exit == true) break;
            }

            int n = alerts.Count;
            alerts.Add(new alertBox(text));
            alerts[n].Margin = new Thickness(alerts[n].Width+10, 0, 0, 0);
            _Panel_Alert.Children.Add(alerts[n]);

            ThicknessAnimation an = new ThicknessAnimation();
            an.From = alerts[n].Margin;
            an.To = new Thickness(0, 0, 0, 0);
            an.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            alerts[n].BeginAnimation(alertBox.MarginProperty, an);
        }

        //타이머 - 0으로 변환
        string setZero(int v)
        {
            if (v < 10)
            {
                return "0" + v;
            }
            return v + "";
        }

        //파티클 타이머 틱
        void Particle_Tick(object sender, EventArgs e)
        {
            partP = new Point(200, 200);
            try
            {
                _Panel_Timer.Children.Remove(particle[pCnt].rect);
            }
            catch (Exception) { }
            particle[pCnt] = new Particle(new Point(50, 50), new Point(150, 150), 50, 200);
            _Panel_Timer.Children.Add(particle[pCnt].rect);
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(partP.X, partP.Y, 0, 0);
            animation.To = new Thickness(particle[pCnt].vector.X, particle[pCnt].vector.Y, 0, 0);
            animation.Duration = particle[pCnt].duration;
            particle[pCnt].rect.BeginAnimation(Rectangle.MarginProperty, animation);
        }

        //스톱워치 타이어 틱
        void Timer_Tick(object sender, EventArgs e)
        {
            time[0]++;
            if (time[0] >= 60)
            {
                time[1]++;
                time[0] = 0;
            }
            if (time[1] >= 60)
            {
                time[2]++;
                time[1] = 0;
            }
            timerL.Content = $"{setZero(time[2])}:{setZero(time[1])}:{setZero(time[0])}";
        }

        //문자열 자르기
        string Cut(string text, int from, int to)
        {
            try
            {
                string result = "";
                for (int i = from; i < to; i++)
                {
                    result += text[i];
                }
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        //문자열 특정 기준으로 나누기
        string[] Split(string text, string arg)
        {
            string a = text;
            int n = a.IndexOf(arg);
            List<string> arr = new List<string>();
            if (n == -1)
            {
                return new string[] { text };
            }
            while (true)
            {
                if (n == -1)
                {
                    break;
                }
                else
                {
                    arr.Add(Cut(a, 0, n));
                    a = Cut(a, n + 1, a.Length);
                    n = a.IndexOf(arg);
                    if (n == -1)
                    {
                        if (a.Length == 0)
                        {
                            break;
                        }
                        arr.Add(Cut(a, 0, a.Length));
                        break;
                    }
                }
            }
            string[] result = new string[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                result[i] = arr[i];
            }
            return result;
        }

        //Result 초기화
        void resetResult()
        {
            for(int i=0; i<_Page_Result.line.Count; i++)
            {
                _Page_Result.line[i].Set(_Word_Main[i]);
                _Page_Result.line[i].Eng.Foreground = new SolidColorBrush(Colors.White);
                _Page_Result.line[i].Kor.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        //모든 패널 리셋
        void setAllPage()
        {
            Log("setAllPage", "entry");
            CountL.Content = $"0/{_Word_Main.Length}";
            Log("setAllPage", "Set _Page_Test");
            //setLine(_Page_Test, _Word_Main);
            Log("setAllPage", "Set _Page_Result");
            //setLine(_Page_Result, _Word_Main);
            Log("setAllPage", "Set _Page_Test");
            //setLine(_Page_Input, _Word_Main);
            Log("setAllPage", "Reset _Page_Test");
            Log("setAllPage", "Mix");
            Mix();
            resetLine(_Page_Test, _Word_Main);
            resetLine(_Page_Random, _Word_Random);
            resetResult();
            Log("setAllPage", "complete");
        }

        //단어 섞기
        void Mix()
        {
            int[] nums = new int[_Word_Random.Length];
            int[] result = new int[_Word_Random.Length];
            int n = -1;

            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = i;
                result[i] = -1;
            }

            for (int i = 0; i <  nums.Length; i++)
            {
                
                while (true)
                {
                    n = new Random().Next(0, nums.Length);
                    bool can_exit = true;

                    for (int j = 0; j < nums.Length; j++)
                    {
                        if (n == result[j])
                        {
                            can_exit = false;
                        }
                    }

                    if (can_exit == true)
                    {
                        result[i] = n;
                        break;
                    }
                }
            }

            for(int i=0; i<result.Length; i++)
            {
                _Word_Random[i] = new Word(_Word_Main[result[i]]);
            }
        }

        //저장하기
        void Save()
        {
            Log("Save", "entry");
            StreamWriter fe = new StreamWriter(path[0]);
            StreamWriter fk = new StreamWriter(path[1]);
            StreamWriter fi = new StreamWriter(path[2]);

            for (int i = 0; i < _Word_Main.Length; i++)
            {
                try
                {
                    fe.WriteLine(_Page_Input.line[i].Eng.Text);
                    fk.WriteLine(_Page_Input.line[i].Kor.Text);
                    
                }
                catch (Exception)
                {
                    fe.WriteLine("eng" + i);
                    fk.WriteLine("kor" + i);
                }
            }

            try
            {
                fi.WriteLine(Size_Word);
            }
            catch (Exception)
            {
                fi.WriteLine(22);
            }

            fe.Close();
            fk.Close();
            fi.Close();
            Log("Save", "complete");
        }

        //불러오기
        void Load()
        {
            try
            {
                Log("Load", "entry");
                StreamReader fe = new StreamReader(path[0]);
                StreamReader fk = new StreamReader(path[1]);
                StreamReader fi = new StreamReader(path[2]);

                try
                {
                    Size_Word = Convert.ToInt32(fi.ReadLine());
                }
                catch (Exception)
                {
                    Size_Word = 22;
                }

                for (int i = 0; i < _Word_Main.Length; i++)
                {
                    _Word_Main[i]=new Word("e" + i, "k" + i);
                    try
                    {
                        _Word_Main[i].Set(fe.ReadLine(), fk.ReadLine());
                    }
                    catch (Exception){ }
                }

                

                fi.Close();
                fe.Close();
                fk.Close();
            }
            catch (Exception)
            {
                Log("Load", "exception");
                Save();
                Load();
            }
            Log("Load", "complete");
        }

        //채점 도움 함수
        void Checking(Page page, Word[] word)
        {
            SolidColorBrush color;
            int Count = 0;
            for (int i=0; i<page.line.Count; i++)
            {
                color = new SolidColorBrush(Color.FromRgb(255, 153, 153));
                string[] text = Split(word[i].Kor, ",");
                for (int j = 0; j < text.Length; j++)
                {
                    if (page.line[i].Kor.Text.Replace(" ", "") == text[j].Replace(" ",""))
                    {
                        Count++;
                        color = new SolidColorBrush(Color.FromRgb(150, 255, 150));
                        break;
                    }
                }
                _Page_Result.line[i].Eng.Text = page.line[i].Eng.Text;
                _Page_Result.line[i].Kor.Text = $"{page.line[i].Kor.Text}({word[i].Kor})";
                _Page_Result.line[i].Eng.Foreground = color;
                _Page_Result.line[i].Kor.Foreground = color;
            }
            CountL.Content = $"{Count}/{page.line.Count}";
            color = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (Count == page.line.Count)
            {
                color = new SolidColorBrush(Color.FromRgb(150, 255, 150));
            }
            else if (Count >= page.line.Count-3)
            {
                color = new SolidColorBrush(Color.FromRgb(234, 234, 175));
            }
            CountL.Foreground = color;
            Log("Checking", "complete");
        }

        //패널당 단어 및 페이지
        void Check()
        {
            if (Before_Panel == _Panel_Test)
            {
                Checking(_Page_Test, _Word_Main);
            }
            if (Before_Panel == _Panel_Random)
            {
                Checking(_Page_Random, _Word_Random);
            }
        }

        //사이즈 설정
        void setSize(Control ct, int width, int height)
        {
            ct.Width = width;
            ct.Height = height;
        }

        /*--Window_Load--*/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Window_Load", "entry");
            Before_Panel = _Panel_Result;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            particleT.Interval = TimeSpan.FromSeconds(0.01);
            particleT.Tick += new EventHandler(Particle_Tick);
            setTimer();
            //particleT.Start();

            Load();
            Mix();
            Log("Setting Page", "Result");
            _Page_Result = new Page(_Panel_Result, _Word_Main, false, false);
            _Page_Result.line[0].Width = 750;
            Log("Setting Page", "Test");
            _Page_Test = new Page(_Panel_Test, _Word_Main, false, true);
            Log("Setting Page", "Random");
            _Page_Random = new Page(_Panel_Random, _Word_Random, false, true);
            Log("Setting Page", "Input");
            _Page_Input = new Page(_Panel_Input, _Word_Main, true, true);
            Log("Window_Load", "setAllPage");

            Log("Window_Load", "ResetEveryThing");
            ResetEveryThing();
            Load();
            setAllPage();
            Log("Window_Load", "complete");
        }

        //참조
        public MainWindow()
        {
            InitializeComponent();
        }

        //App 이동시키기
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        //App 닫기
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        //닫기 버튼 보이기
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(100, 90, 45, 45));
        }

        //닫기 버튼 가리기
        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(0, 30, 30, 30));
        }

        //Grid Move Animation
        void GridMove(Grid grid, double x, double y, double sec)
        {
            ThicknessAnimation anm = new ThicknessAnimation();
            anm.From = grid.Margin;
            anm.To = new Thickness(x, y, 0, 0);
            anm.Duration = new Duration(TimeSpan.FromSeconds(sec));
            grid.BeginAnimation(Grid.MarginProperty, anm);
        }

        //메뉴 ON / OFF
        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Rectangle rt = sender as Rectangle;
            if (is_Open == true)
            {
                rt.Stroke = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                is_Open = false;
                GridMove(SelectBar, -SelectBar.Width + 10, SelectBar.Margin.Top, 0.1);
                GridMove(MenuBar, MenuBar.Margin.Left, -SelectBar.Height, 0.2);
            }
            else
            {
                rt.Stroke = new SolidColorBrush(Color.FromRgb(65, 65, 65));
                is_Open = true;
                GridMove(SelectBar, 0, SelectBar.Margin.Top, 0.1);
                GridMove(MenuBar, MenuBar.Margin.Left, 0, 0.2);
            }
            Log("Menu", is_Open+"");
        }

        //패널 선택
        void SelectPanel(Grid panel, TextBlock tb)
        {
            Before_Panel.Visibility = Visibility.Hidden;
            panel.Visibility = Visibility.Visible;
            Before_Panel = panel;
            ThicknessAnimation ani = new ThicknessAnimation();
            ani.From = new Thickness(SelectBorder.Margin.Left, SelectBorder.Margin.Top, 0, 0);
            ani.To = new Thickness(SelectBorder.Margin.Left, tb.Margin.Top, 0, 0);
            ani.Duration = new Duration(TimeSpan.FromSeconds(0.15));
            SelectBorder.BeginAnimation(TextBlock.MarginProperty, ani);
            Log("Select", "Panel : " + tb.Text);
        }

        //패널 선택 버튼 이벤트
        //Test
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectPanel(_Panel_Test, (sender as TextBlock));
            }
        }

        //Random
        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectPanel(_Panel_Random, (sender as TextBlock));
            }
        }

        //Result
        private void TextBlock_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectPanel(_Panel_Result, (sender as TextBlock));
            }
        }

        //Input
        private void TextBlock_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectPanel(_Panel_Input, (sender as TextBlock));
            }
        }

        //Timer
        private void TextBlock_MouseDown_10(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectPanel(_Panel_Timer, (sender as TextBlock));
            }
        }

        //단어 적용
        void setLine(Page page, Word[] word)
        {
            for (int i = 0; i < word.Length; i++) {
                //page.line[i].Set(word[i]);
            }
        }

        //라인 세팅
        void resetLine(Page page, Word[] word)
        {
            Log("resetLine", "entry");
            for (int i = 0; i < word.Length; i++)
            {
                Word wd = new Word(word[i].Eng, word[i].Kor);
                if (page.line[i].Eng.Focusable == true)
                {
                    wd.Eng = "";
                }
                if (page.line[i].Kor.Focusable == true)
                {
                    wd.Kor = "";
                }
                page.line[i].Set(wd);
                page.line[i].Eng.Foreground = new SolidColorBrush(Colors.White);
                page.line[i].Kor.Foreground = new SolidColorBrush(Colors.White);
            }
            Log("resetLine", "complete");
        }

        //패널 -> 페이지 반환
        Page getPage(Grid grid)
        {
            Page page = new Page();
            if (grid == _Panel_Test)
            {
                page = _Page_Test;
            }
            else if (grid == _Panel_Random)
            {
                page = _Page_Random;
            }
            else if (grid == _Panel_Result)
            {
                page = _Page_Result;
            }
            else if (grid == _Panel_Input)
            {
                page = _Page_Input;
            }
            else if (grid == _Panel_Timer)
            {
                page = _Page_Result;
            }

            return page;
        }

        //패널 -> 단어 반화
        Word[] getWord(Grid grid)
        {
            if (grid == _Panel_Random)
            {
                return _Word_Random;
            }
            else
            {
                return _Word_Main;
            }
        }

        //Button Apply
        private void TextBlock_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < _Page_Input.line.Count; i++)
            {
                _Word_Main[i].Set(_Page_Input.line[i].Eng.Text, _Page_Input.line[i].Kor.Text);
            }
            setAllPage();
            alert("Complete Apply");
        }

        //Button Reset
        private void TextBlock_MouseDown_5(object sender, MouseButtonEventArgs e)
        {
            resetLine(getPage(Before_Panel), getWord(Before_Panel));
        }

        //Button Save
        private void TextBlock_MouseDown_6(object sender, MouseButtonEventArgs e)
        {
            Save();
            alert("Complete Save");
        }

        //Button Load
        private void TextBlock_MouseDown_7(object sender, MouseButtonEventArgs e)
        {
            Load();
            setAllPage();
            alert("Complete Load");
        }

        //Button Mix
        private void TextBlock_MouseDown_8(object sender, MouseButtonEventArgs e)
        {
            Mix();
            resetLine(_Page_Random, _Word_Random);
        }

        //Button Check
        private void TextBlock_MouseDown_9(object sender, MouseButtonEventArgs e)
        {
            Check();
        }

        //TextBox Enter -> ForeColor
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(193,211,249));
        }

        //TextBox Leave -> ForeColor
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        //타이머 상태 설정
        void setTimer()
        {
            int left = 328;
            if (timer.IsEnabled == true)
            {
                Log("Timer", "on");
                statusL.Margin = new Thickness(left, statusL.Margin.Top, 0, 0);
                statusL.Text = "START";
                statusRt.Fill = new SolidColorBrush(Color.FromRgb(140, 226, 99));
                timer.Stop();
            }
            else
            {
                Log("Timer", "off");
                statusL.Margin = new Thickness(left + 12, statusL.Margin.Top, 0, 0);
                statusL.Text = "STOP";
                statusRt.Fill = new SolidColorBrush(Color.FromRgb(225, 100, 100));
                timer.Start();
            }
        }

        //Button Timer status
        private void statusRt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            setTimer();
        }

        //Button Timer reset
        private void Rectangle_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            statusL.Margin = new Thickness(328, statusL.Margin.Top, 0, 0);
            statusL.Text = "START";
            statusRt.Fill = new SolidColorBrush(Color.FromRgb(140, 226, 99));
            timer.Stop();
            time = new int[]{ 0, 0, 0, 0 };
            timerL.Content = $"{setZero(time[2])}:{setZero(time[1])}:{setZero(time[0])}";
        }

        //Margin -> Pos
        Point toPos(Thickness margin)
        {
            return new Point(margin.Left + 102, margin.Top + 25);
        }

        //Pos -> Margin
        Thickness toMargin(Point pos)
        {
            return new Thickness(pos.X - 102, pos.Y - 102, 0, 0);
        }

        private void _Panel_Line_MouseEnter(object sender, MouseEventArgs e)
        {
            GridMove(MinusP, MinusP.Margin.Left, 50, 0.1);
            MinusRect.Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60));
        }

        private void _Panel_Line_MouseLeave(object sender, MouseEventArgs e)
        {
            MinusRect.Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        }
        

        private void AddP_MouseEnter(object sender, MouseEventArgs e)
        {
            GridMove(MinusP, MinusP.Margin.Left, 50, 0.1);
            AddRect.Fill = new SolidColorBrush(Color.FromRgb(60, 60, 60));
        }

        private void AddP_MouseLeave(object sender, MouseEventArgs e)
        {
            AddRect.Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        }
        
        private void _Panel_Line_MouseEnter_1(object sender, MouseEventArgs e)
        {
            GridMove(MinusP, MinusP.Margin.Left, 50, 0.1);
        }

        private void _Panel_Line_MouseLeave_1(object sender, MouseEventArgs e)
        {
            GridMove(MinusP, MinusP.Margin.Left, 105, 0.1);
        }

        Visibility visible_modify = Visibility.Hidden;
        private void Rectangle_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            if(visible_modify == Visibility.Hidden)
            {
                visible_modify = Visibility.Visible;
                (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(10, 90, 250, 65));
            }
            else
            {
                visible_modify = Visibility.Hidden;
                (sender as Rectangle).Fill = new SolidColorBrush(Color.FromArgb(10, 255, 155, 155));
            }
            CountTB.Visibility = visible_modify;
            CountTB.Text = Size_Word + "";
        }

        private void Rectangle_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            int before = Size_Word;
            try
            {
                int value = Convert.ToInt32(CountTB.Text);
                if(value > 0 && value < 300)
                {
                    Size_Word = value;
                }
            }
            catch(Exception){}
            if (Size_Word != before)
            {
                ResetEveryThing();
                alert("Reseted Every Thing");
            }
        }

        int getLine()
        {
            Page p = getPage(Before_Panel);
            if (p != _Page_Result)
            {
                for (int i = 0; i < p.line.Count; i++)
                {
                    if (p.line[i].Eng.IsFocused == true || p.line[i].Kor.IsFocused == true)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        void moveFocus(int a)
        {
            Page p = getPage(Before_Panel);
            int n = getLine();
            if (a == -1)
            {
                if (p.line[n].Kor.IsFocused)
                {
                    p.line[n].Eng.Focus();
                }
                else if (p.line[n].Eng.IsFocused)
                {
                    if (n - 1 >= 0)
                    {
                        p.line[n - 1].Kor.Focus();
                    }
                }
            }
            if (a == 1)
            {
                if (p.line[n].Eng.IsFocused)
                {
                    p.line[n].Kor.Focus();
                }
                else if (p.line[n].Kor.IsFocused)
                {
                    if (n + 1 <= p.line.Count)
                    {
                        p.line[n + 1].Eng.Focus();
                    }
                }
            }
        }

        //키 입력
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                moveFocus(-1);
            }
            if (e.Key == Key.F2)
            {
                moveFocus(1);
            }
            if (e.Key == Key.PageUp)
            {
                getPage(Before_Panel).addPageNum(1);
            }
            if (e.Key == Key.Next)
            {
                getPage(Before_Panel).addPageNum(-1);
            }
        }
    }//class
}//namespace