using System.Globalization;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace LiveGame
{
    public partial class Form1 : Form
    {
        bool buttonStartPress = false;
        public delegate void ThreadStart();
        const int sz = 20;

        bool[,] currentState = new bool[sz, sz];
        bool[,] nextState = new bool[sz, sz];

        int speedRate = 100;
        int liveCellQuantity = 0;

        void ResetBoolMatrix(bool[,] array, int size)
        {
            for(int i = 0; i < size; i++)
                for(int j = 0; j < size; j++)
                    array[i,j] = false;
        }

        void CountLiveCell()
        {
            liveCellQuantity = 0;
            for (int i = 0; i < sz; i++)
                for (int j = 0; j < sz; j++)
                    if (currentState[i, j] == true)
                        liveCellQuantity++;
        }

        void RandomMatrix(bool[,] array, int size)
        {
            Random random = new();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    if(random.Next() % 2 == 1)
                        array[i, j] = true;
                    else
                        array[i, j] = false;
                }
                    
        }

        void CalculateNextState()
        {
            nextState = new bool[20, 20];

            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                {
                    if(currentState[i,j] == false && CountNeigborsCell(i,j) == 3)
                    {
                        nextState[i,j] = true;
                    }
                    else if (currentState[i, j] == true && (CountNeigborsCell(i, j) == 2 || CountNeigborsCell(i, j) == 3))
                    {
                        nextState[i, j] = true;
                    }
                    else if (currentState[i, j] == false && CountNeigborsCell(i, j) == 2 && mutation.Checked)
                    {
                        Random rand = new Random();
                        if(rand.Next() % 25 == 0)
                        {
                            nextState[i, j] = true;
                        }
                    }
                    else if (currentState[i, j] == false && creation.Checked)
                    {
                        Random rand = new Random();
                        {
                            if(rand.Next() % 200 == 0)
                                nextState[i, j] = true;
                        }
                    }
                }
            currentState = nextState;
        }

        void PrintCurrentState()
        {
            for(int i = 0; i < 20; i++)
                for(int j = 0; j < 20; j++)
                {
                    if (currentState[i, j])
                    {
                        tableMain.GetControlFromPosition(j, i).BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Biohazard));
                    }
                    else
                    {
                        tableMain.GetControlFromPosition(j, i).BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Black19));
                    }
                }
        }

        void Play()
        {
            while (buttonStartPress)
            {
                CalculateNextState();
                PrintCurrentState();
                CountLiveCell();
                QuantityPanel.Text = liveCellQuantity.ToString();
                Thread.Sleep(speedRate);
            }
        }

        bool CheckCurrentPosition(int x, int y)
        {
            return !(x < 0 || x > 19 || y < 0 || y > 19);
        }

        int CountNeigborsCell(int x, int y)
        {

            int quantity = 0;

            for(int i = x - 1; i <= x + 1; i++)
                for(int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;
                    if(CheckCurrentPosition(i, j))
                    {
                        if(currentState[i, j] == true)
                            quantity++;
                    }
                }            

            return quantity;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_MouseEnter(object sender, EventArgs e)
        {
            this.buttonStart.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.launch2));
        }

        private void buttonStart_MouseLeave(object sender, EventArgs e)
        {
            if (buttonStartPress)
            {
                this.buttonStart.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.launch3));
            }
            else
            { 
                this.buttonStart.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.launch));
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStartPress = !buttonStartPress;
            if (buttonStartPress)
            {
                this.buttonStart.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.launch3));

                if (random.Checked)
                {
                    RandomMatrix(currentState, 20);
                }

                Thread thread = new Thread(Play);
                thread.Start();
            }
            else
            {
                this.buttonStart.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.launch));
                ResetBoolMatrix(currentState, 20);
                PrintCurrentState();
            }
            QuantityPanel.Text = "";
        }

        private void buttonStart_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int bSize = 19;
            int bQuantity = 20;




            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                {
                    currentState[i, j] = false;
                    
                    Button cell = new();
                    cell.Tag = i + "#" + j;
                    cell.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Skill));
                    cell.BackgroundImageLayout = ImageLayout.Stretch;
                    cell.FlatStyle = FlatStyle.Flat;
                    cell.Width = bSize;
                    cell.Height = bSize;
                    cell.Dock = DockStyle.Fill;
                    cell.Margin = new Padding(1);
                    cell.Click += new EventHandler(buttonCell_Click);
                    tableMain.Controls.Add(cell);
                }
        }

        private void buttonCell_Click(object sender, EventArgs args)
        {
            var pressedButton = sender as Button;

            int x = int.Parse(pressedButton.Tag.ToString().Split('#')[0]);
            int y = int.Parse(pressedButton.Tag.ToString().Split('#')[1]);


            if (currentState[x, y] == false)
            {

                pressedButton.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Biohazard));
                currentState[x, y] = true;
            }
            else
            {
                pressedButton.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.Skill));
                currentState[x, y] = false;
            }
            
        }

        private void tableMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            speedRate = trackBar1.Value;
        }

        private void QuantityPanel_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}