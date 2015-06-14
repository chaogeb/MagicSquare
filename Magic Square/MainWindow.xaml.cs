using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

        // Center MainWindow
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void LoadMagicSquare(int rank)
        {
            MagicSquare.Children.Clear();
            MagicSquare.RowDefinitions.Clear();
            MagicSquare.ColumnDefinitions.Clear();
            
            var square = new MSquare(rank);

            for (int i = 0; i < rank; i++)
            {
                MagicSquare.RowDefinitions.Add(new RowDefinition());
                MagicSquare.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int row, col;
            double fontsize = (this.ActualHeight - 400) / rank;
            for (row = 0; row < rank; row++)
            {
                for (col = 0; col < rank; col++)
                {
                    var btn = new Button();
                    btn.Content = square.a[row][col];
                    btn.FontStretch = FontStretches.Condensed;
                    btn.FontSize = fontsize;
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    btn.SetValue(Grid.RowProperty, row);
                    btn.SetValue(Grid.ColumnProperty, col);
                    btn.Background = null;
                    MagicSquare.Children.Add(btn);
                }
            }
        }
        private void CantLoadMagicSquare(string str)
        {
            MagicSquare.Children.Clear();
            MagicSquare.RowDefinitions.Clear();
            MagicSquare.ColumnDefinitions.Clear();
            
            var btn = new Button();
            btn.Content = str;
            btn.FontSize = this.ActualWidth / 15;
            btn.FontWeight = FontWeights.Bold;
            btn.Foreground = new SolidColorBrush(Color.FromRgb(169, 169, 169));
            btn.Background = null;
            MagicSquare.Children.Add(btn);
        }

        private void LoadMagicSquare(object sender, TextChangedEventArgs e)
        {
            CantLoadMagicSquare("Please Input the Rank");
            short val;
            if (!Int16.TryParse(RankTbx.Text, out val))
                return;
            else if (val < 1 || val == 2)
            {
                CantLoadMagicSquare("Are you sure?");
                return;
            }
            else if (val > 999)
            {
                CantLoadMagicSquare("（╯‵□′）╯︵┴─┴");
                return;
            }
            else if (val >= 30)
            {
                CantLoadMagicSquare("NOT AVAILABLE");
                return;
            }
            LoadMagicSquare(int.Parse(RankTbx.Text));
        }

        private void pnl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            short val;
            if (!Int16.TryParse(e.Text, out val))
            {
                e.Handled = true;
            }
        }

        private void pnl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }

    class MSquare
    {
        public int[][] a    { get; private set; }
        public int N        { get; private set; }

        public MSquare(int rank)
        {
            N = rank;
            a = new int[N][];
            for (int k = 0; k < N; k++)
            {
                a[k] = new int[N];
            }
            MagicSquare();
        }

        private void MagicSquare()
        {
            if (N >= 1 && N % 2 != 0)
                Odd();
            else if (N >= 4 && N % 2 == 0)
                Even();
        }

        /// <summary>
        /// Odd Magic square
        /// </summary>
        // 奇魔方（阶数 n = 2 * m + 1，m = 1，2，3…… ）规律如下
        // 1.数字1位于方阵中的第一行中间一列
        // 2.数字a(1 < a ≤ n^2）所在行数比a-1行数少1，若a-1的行数为1，则a的行数为n
        // 3.数字a（1 < a ≤ n^2）所在列数比a-1列数大1，若a-1的列数为n，则a的列数为1
        // 4.如果a-1是n的倍数，则a（1 < a ≤ n^2）的行数比a-1行数大1，列数与a-1相同
        private void Odd()
        {
            int i;
            int col, row;

            col = (N - 1) / 2;
            row = 0;

            a[row][col] = 1;

            for (i = 2; i <= N * N; i++)
            {
                if ((i - 1) % N == 0)
                {
                    row++;
                }
                else
                {
                    // if row = 0, then row = N-1, or row = row - 1
                    row--;
                    row = (row + N) % N;

                    // if col = N, then col = 0, or col = col + 1
                    col++;
                    col %= N;
                }
                a[row][col] = i;
            }

            //OutPut();
            return;
        }

        /// <summary>
        /// Even Magic square
        /// </summary>
        // 分为阶数 n = 4 * m（m = 1，2，3……）的情况
        // 和阶数 n = 4 * m + 2（m = 1，2，3……）情况两种
        private void Even()
        {
            if (N >= 4 && N % 4 == 0)
                even4m();
            else if (N >= 6 && N % 4 == 2)
                even4mp2();
        }

        /// <summary>
        /// Double Even Magic square
        /// </summary>
        // 双偶魔方（阶数 n = 4 * m（m =1，2，3……）的偶魔方）的规律如下
        // 1.按数字从小到大，即1，2，3……n2顺序对魔方阵从左到右，从上到下进行填充
        // 2.将魔方中间n/2列的元素上、下进行翻转
        // 3.将魔方中间n/2行的元素左、右进行翻转
        private void even4m()
        {
            int i, temp;    //临时变量
            int col, row;   //col 列，row 行

            i = 1;
            for (row = 0; row < N; row++)
            {
                for (col = 0; col < N; col++)
                {
                    a[row][col] = i;
                    i++;
                }
            }

            //翻转中间列
            for (row = 0; row < N / 2; row++)
            {
                for (col = N / 4; col < N / 4 * 3; col++)
                {
                    temp = a[row][col];
                    a[row][col] = a[N - row - 1][col];
                    a[N - row - 1][col] = temp;
                }
            }

            //翻转中间行
            for (col = 0; col < N / 2; col++)
            {
                for (row = N / 4; row < N / 4 * 3; row++)
                {
                    temp = a[row][col];
                    a[row][col] = a[row][N - col - 1];
                    a[row][N - col - 1] = temp;
                }
            }

            //OutPut();
            return;
        }

        /// <summary>
        /// Single Even Magic square
        /// </summary>
        // 单偶魔方（阶数 n = 4 * m + 2（m=1，2，3……）的魔方）的规律如下
        // 1.将魔方分成 A、B、C、D 四个k阶方阵，这四个方阵都为奇方阵，利用上面讲到的方法依次将 A、D、B、C 填充为奇魔方
        // 2.交换 A、C 魔方元素，对魔方的中间行，交换从中间列向右的m列各对应元素；对其他行，交换从左向右m列各对应元素
        // 3.交换 B、D 魔方元素，交换从中间列向左 m-1 列各对应元素
        private void even4mp2()
        {
            int i, j, temp;
            int col, row;// col 列，row 行

            //初始化
            j = N / 2;
            col = (j - 1) / 2;
            row = 0;
            a[row][col] = 1;
            //生成奇魔方A
            for (i = 2; i <= j * j; i++)
            {
                if ((i - 1) % j == 0)//前一个数是3的倍数
                {
                    row++;
                }
                else
                {
                    // if row = 0, then row = N-1, or row = row - 1
                    row--;
                    row = (row + j) % j;

                    // if col = N, then col = 0, or col = col + 1
                    col++;
                    col %= j;
                }
                a[row][col] = i;
            }

            //根据A生成B、C、D魔方
            for (row = 0; row < j; row++)
            {
                for (col = 0; col < j; col++)
                {
                    a[row + j][col + j] = a[row][col] + j * j;
                    a[row][col + j] = a[row][col] + 2 * j * j;
                    a[row + j][col] = a[row][col] + 3 * j * j;
                }
            }

            // Swap A and C
            for (row = 0; row < j; row++)
            {
                if (row == j / 2)//中间行，交换从中间列向右的m列，N = 2*(2m+1)
                {
                    for (col = j / 2; col < j - 1; col++)
                    {
                        temp = a[row][col];
                        a[row][col] = a[row + j][col];
                        a[row + j][col] = temp;
                    }
                }
                else//其他行，交换从左向右m列,N = 2*(2m+1)
                {
                    for (col = 0; col < j / 2; col++)
                    {
                        temp = a[row][col];
                        a[row][col] = a[row + j][col];
                        a[row + j][col] = temp;
                    }
                }
            }

            // Swap B and D
            for (row = 0; row < j; row++)//交换中间列向左m-1列，N = 2*(2m+1)
            {
                for (i = 0; i < (j - 1) / 2 - 1; i++)
                {
                    temp = a[row][j + j / 2 - i];
                    a[row][j + j / 2 - i] = a[row + j][j + j / 2 - i];
                    a[row + j][j + j / 2 - i] = temp;
                }
            }

            //OutPut();
            return;
        }

        private void OutPut()
        {
            int col, row;
            Console.WriteLine("Magic square of {0:d}:", N);
            for (row = 0; row < N; row++)
            {
                int amount = 0;
                for (col = 0; col < N; col++)
                {
                    amount += a[row][col];
                    Console.Write("{0,5:d}", a[row][col]);
                }
                Console.WriteLine("{0,8:d}", amount);
            }
            int diagonal = 0;
            for (col = 0; col < N; col++)
            {
                int amount = 0;
                for (row = 0; row < N; row++)
                {
                    amount += a[row][col];
                }
                Console.Write("{0,5:d}", amount);
                diagonal += a[col][col];
            }
            Console.WriteLine("{0,8:d}\n", diagonal);
            return;
        }
    }

}
