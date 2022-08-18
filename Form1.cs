using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HW_extra_mineSweeper
{
    public partial class Form1 : Form
    {
        PictureBox[] picLala = new PictureBox[17];  //for contain Lala Pictures
        PictureBox[] PB;  //Array of PictureBox 
        PictureBox[] gOver = new PictureBox[8];  //game over pictures
        PictureBox[] gClear = new PictureBox[8]; //game clear pictures
        PictureBox[] showCount = new PictureBox[3]; //mmCss  count of spaces 
        PictureBox[] showMarks = new PictureBox[2];//           ↑here
        bool gOCheck = false;  //gameover check
        int[] pMines;  //position of mines
        int[] mCount; //Number of nearby mines for every position
        int[] haveClicked;  //If Clicked, keep codition.
        int[] cPosibility; //Posibility to show Dala or Bula for every position
        int gClearCount = 0;  //trigger game clear
        int num = 100;//number of the rest of the cells
        int mark = 0; //number of marked cells
        int mNumber;  //number of nearby mines
        int temp;//count nearby mines of every position
        Button[,] btn = new Button[3, 3];//for OX game
        int count = 0;//for OX game

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < showCount.Length; i++)
            {
                showCount[i] = new PictureBox();
                if (i < showMarks.Length)
                    showMarks[i] = new PictureBox();
            }         
        }
        private void invisible()
        {
            for (int i = 0; i < showCount.Length; i++)
            {
                showCount[i].Visible = false;
                if(i< showMarks.Length)
                    showMarks[i].Visible = false;
            }
        }
        void SMark(int n)
        {
            int a, b;
            a = n % 10;  //a= units digit
            b = (n - a) / 10;  //b= tens digit

            for (int i = 0; i < showMarks.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        showMarks[i].Image = picLala[b].Image;//picture of tens digit
                        break;
                    case 1:
                        showMarks[i].Image = picLala[a].Image;  // picture of units digit
                        break;
                }
                showMarks[i].Location = new Point(160 + i * 50, 10);
                showMarks[i].SizeMode = PictureBoxSizeMode.AutoSize;
                showMarks[i].MouseEnter +=new EventHandler(tipMessage);
                this.Controls.Add(showMarks[i]);
            }
        }
        public void Mcount(int n)  // show remaining mines and spaces
        {
            int a, b;
            a = n % 10;  //a= units digit
            b = (n - a) / 10;  //b= tens digit

            for (int i = 0; i < showCount.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        showCount[i].Image = picLala[b].Image;//picture of tens digit
                        break;
                    case 1:
                        showCount[i].Image = picLala[a].Image;  //picture of units digit
                        break;
                    case 2:
                        showCount[i].Image = picLala[16].Image;
                        break;
                }
                showCount[i].Location = new Point(10 + i * 50, 10);
                showCount[i].SizeMode = PictureBoxSizeMode.AutoSize;
                showCount[i].MouseEnter += new EventHandler(tipMessage);
                this.Controls.Add(showCount[i]);
            }
        }

        private void tipMessage(object sender, EventArgs e)
        {
            PictureBox PB = (PictureBox)sender;
            ToolTip tipBox = new ToolTip();
            if (PB == showCount[0] || PB == showCount[1])
                tipBox.SetToolTip(PB, "剩餘的空格數量");
            else if (PB == showCount[2])
                tipBox.SetToolTip(PB, "吃瓜民眾分隔線");
            else
                tipBox.SetToolTip(PB, "尚待標記(右鍵)的數量");
        }

        public void gameover()
        {
            for (int i = 0; i < gOver.Length; i++)
            {
                gOver[i] = new PictureBox();
                gOver[i].Image = Image.FromFile(@"..\..\PIC\gameover\" + i.ToString() + ".gif");
                gOver[i].Location = new Point(10 + i * 50, 10);
                gOver[i].SizeMode = PictureBoxSizeMode.AutoSize;
                this.Controls.Add(gOver[i]);
            }
            invisible();
        }

        public void gameClear()
        {
            for (int i = 0; i < gOver.Length; i++)
            {
                gClear[i] = new PictureBox();
                gClear[i].Image = Image.FromFile(@"..\..\PIC\gameclear\" + i.ToString() + ".gif");
                gClear[i].Location = new Point(10 + i * 50, 10);
                gClear[i].SizeMode = PictureBoxSizeMode.AutoSize;
                this.Controls.Add(gClear[i]);
            }
            invisible();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showCount[2].Image = picLala[15].Image;  //zzSu →cigaLa
            for (int i = 0; i < num; i++)
            {
                PB[i].Image = picLala[mCount[i]].Image;
                haveClicked[i] = 1;
                for (int j = 0; j < pMines.Length; j++)
                {
                    if (i == pMines[j])
                    {
                        gOCheck = true;  //game over
                        PB[i].Image = picLala[13].Image;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            linkLabel1.Visible = true;
            label1.Visible = label2.Visible = button3.Visible = false;
            if (textBox1.Text == "" || textBox3.Text == "")  //mu space
                return;
            else if (Convert.ToInt32(textBox1.Text) > 100 ||  //mu >100
                          Convert.ToInt32(textBox3.Text) > 100)
                return;

            textBox1.ReadOnly = textBox3.ReadOnly = true;//disable keyin after set
            button2.Visible = false;
            button1.Visible = true;  //show "Open the price" button
            cPosibility = new int[num];    // % of showing hints
            mNumber = Convert.ToInt32(textBox3.Text);  //number of Mines
            mCount = new int[num];    //number of nearby mines
            haveClicked = new int[num];  //clicked check for every cell(LALA)

            //Posibility to show Dala or Bula
            Random rnd = new Random();  // don't use plural Random variant
            for (int i = 0; i < num; i++)  //possibility MAX=99 
                cPosibility[i] = rnd.Next(1, 100) + Convert.ToInt32(textBox1.Text);

            //Random without overlapping (first LINQ)  
            List<int> randomList = Enumerable.Range(0, 100).OrderBy(x => rnd.Next()).Take(mNumber).ToList();

            //position of mines
            pMines = new int[mNumber];
            for (int i = 0; i < pMines.Length; i++)
                pMines[i] = randomList[i];  //define every mines' position

            //MCount:number of nearby mines 
            foreach (int item in pMines)  //every mine add its nearby MCount's count
            {
                if (item % 10 == 0)  //0,10,~90 the [X axis=0] side
                {
                    for (int x = 0; x < 2; x++)  //row  0,+1
                    {
                        for (int y = -1; y < 2; y++)  //column  -10  0 +10 
                            Mcounter(x, y, item);
                    }
                }
                else if (item % 10 == 9)  //9, 19~99 [X asid=9] side
                {
                    for (int x = -1; x < 1; x++)  //row  -1, 0
                    {
                        for (int y = -1; y < 2; y++)  //column  -10  0 +10
                            Mcounter(x, y, item);
                    }
                }
                else
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                            Mcounter(x, y, item);
                    }
                }
            }
            //add Lala picture to picLala 0.gif~16.gif
            for (int j = 0; j < picLala.Length; j++)
            {
                picLala[j] = new PictureBox();
                picLala[j].Image = Image.FromFile(@"..\..\PIC\" + j.ToString() + ".gif");
            }

            PB = new PictureBox[num];  
            int row = 0;    // Y axis  (misunderstood!  normally It should be X
            int column = 0; // X axis  (misunderstood!  normally It should be Y
            for (int i = 0; i < num; i++)
            {
                PB[i] = new PictureBox();  
                PB[i].BorderStyle = BorderStyle.Fixed3D;  
                if (10 + column * 50 >= 510)
                {
                    row += 1;
                    column = 0;
                }
                PB[i].Location = new Point(10 + column * 50, 50 + row * 50);
                column += 1;
                PB[i].SizeMode = PictureBoxSizeMode.AutoSize;
                PB[i].Image = picLala[10].Image;   //default Jolala
                this.Controls.Add(PB[i]);  //0~9 number  10=Jolala 11= Dalala 12=Bulala 13=Wazibasu
                PB[i].Tag = i;  //for counting the number of surrounding mines 
                PB[i].MouseDown += PB_Click;  //actually it's MouseDown event
                PB[i].MouseEnter += PB_MouseEnter;
                PB[i].MouseLeave += PB_MouseLeave;
            }
            Mcount(100 - pMines.Length);  //count the number of remaining space.
            SMark(0);//count the number of marked cells
        }
        private void Mcounter(int X, int Y, int Item)//count nearby mines of every position.
        {
            temp = Item + X + Y * 10;
            if (temp < mCount.Length && temp >= 0)
                mCount[temp]++;
        }
        void PB_Click(object sender, MouseEventArgs e)
        {
            if (gOCheck && checkBox1.Checked)  //for Debug
                return;  //stop the function if GameOver with the checked checkBox1
            int index = (int)(sender as PictureBox).Tag;  // define which pic was clicked

            if (e.Button == MouseButtons.Left && haveClicked[index] != 1)  //lift mousedown
            {
                PB[index].Image = picLala[mCount[index]].Image;  //number of nearby mines
                haveClicked[index] = 1; //define whether the pic has been clicked.
                gClearCount++;  //for triggering Game Clear;
                Mcount(100 - gClearCount - pMines.Length);  //show the number of remaining space
                for (int i = 0; i < pMines.Length; i++)
                {
                    if (index == pMines[i])
                    {
                        gOCheck = true;  //game over
                        PB[index].Image = picLala[13].Image;  //wazjbusu
                        gameover();
                    }
                }
                ChainClick(index);
            }
            else if (e.Button == MouseButtons.Right)
            {    //check whether the pic has been clicked
                if (PB[index].Image == picLala[10].Image || //Nilala
                    PB[index].Image == picLala[11].Image || //Julala
                    PB[index].Image == picLala[12].Image)  //Bulala
                {
                    haveClicked[index] = 1;   //avoid marked one being clicked
                    PB[index].Image = picLala[14].Image;  //mark it
                    SMark(++mark);
                }
                else if (PB[index].Image == picLala[14].Image)  //Unmark it
                {
                    haveClicked[index] = 0;
                    PB[index].Image = picLala[10].Image;
                    SMark(--mark);
                }
            }

            if (gClearCount + pMines.Length == 100 && gOCheck == false)
                gameClear();
        }

        void PB_MouseEnter(object sender, EventArgs e)
        {
            int index = (int)(sender as PictureBox).Tag;
            if (haveClicked[index] == 1)  //don't show hint on clicked pic
                return;
            if (cPosibility[index] < 100)  // decide whether to show the hint by its possibility 
                return;
            if (gOCheck && checkBox1.Checked)
                return;

            PB[index].Image = picLala[11].Image;  //not mines
            for (int i = 0; i < pMines.Length; i++)
            {
                if (index == pMines[i])
                    PB[index].Image = picLala[12].Image;  //mines 
            }
        }

        void PB_MouseLeave(object sender, EventArgs e)  //return to Nilala
        {
            int index = (int)(sender as PictureBox).Tag;
            if (haveClicked[index] == 1)
                return;
            PB[index].Image = picLala[10].Image;
        }

        private void ChainClick(int x)  //automatically open part 1  (first recursive
        {
            if ((x % 10 == 0))   //0,10,~90 don't go out of the left borderline
            {
                ChainClick2(x + 1);
                ChainClick2(x + 10);
                ChainClick2(x - 10);
            }
            else if (x % 10 == 9)  //9,19~99 don't go out of the right borderline
            {
                ChainClick2(x - 1);
                ChainClick2(x + 10);
                ChainClick2(x - 10);
            }
            else
            {
                ChainClick2(x + 1);  //go to 4 ways
                ChainClick2(x + 10);
                ChainClick2(x - 1);
                ChainClick2(x - 10);
            }
        }
        private void ChainClick2(int x)  //automatically open part 2
        {
            if (x < 100 && x >= 0) //check the border (upper and lower sides)
            {   //nearby mines =0  and  PB = unclicked 
                if (mCount[x] == 0 && PB[x].Image == picLala[10].Image)
                {
                    for (int i = 0; i < pMines.Length; i++)
                    { //don't open mines;
                        if (x == pMines[i])
                            return;
                    }
                    PB[x].Image = picLala[mCount[x]].Image;  //same as clicked
                    haveClicked[x] = 1;
                    gClearCount++;
                    Mcount(100 - gClearCount - pMines.Length);
                    ChainClick(x);  //recursivly call ChainClink
                }
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                    e.Handled = true;
            }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (e.KeyChar < '0' || e.KeyChar > '9')
                    e.Handled = true;
            }
        }
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tipBox = new ToolTip();
            tipBox.SetToolTip(button1, "有圖有真相");
        }
        private void textBox3_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tipBox = new ToolTip();
            tipBox.SetToolTip(textBox3, "請自行設定地雷數量(0~100)");
        }
        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tipBox = new ToolTip();
            tipBox.SetToolTip(textBox1, "因為我很不會玩踩地雷，\r\n所以追加了每格出現提示機率(%)的功能，\r\n請設定提示的出現機率(0~100)：");
        }
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tipBox = new ToolTip();
            tipBox.SetToolTip(button2, "設定完成，開始遊戲");
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.peso.nu/html/penguin01.html");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            btnReset.Visible = btnEsc.Visible = button4.Visible = false;
            textBox3.Visible = textBox1.Visible =  button2.Visible =
            button3.Visible = label1.Visible = label2.Visible = true;
            Width = 532; Height = 627;
            this.Text = "HW_extra_mineSweeper";
            for (int i = 0; i < btn.GetLength(0); i++)
            {
                for (int j = 0; j < btn.GetLength(1); j++)
                {
                    Controls.Remove(btn[i, j]);
                }
            }
        }
        //--------------------------OX Game Borderline-------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            btnReset.Visible = btnEsc.Visible = button4.Visible = true;
            textBox3.Visible = textBox1.Visible = button1.Visible = button2.Visible = 
            button3.Visible = label1.Visible = label2.Visible = false;
            Width = 465; Height = 430;
            this.Text = "HW_0714_XOGame";
            for (int i = 0; i < btn.GetLength(0); i++)
            {
                for (int j = 0; j < btn.GetLength(1); j++)
                {
                    btn[i, j] = new Button();
                    btn[i, j].Location = new Point(120 + i * 70, 70 + j * 70);
                    btn[i, j].BackColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
                    btn[i, j].Font = new Font("微軟正黑體", 24F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
                    btn[i, j].ForeColor = SystemColors.ControlDarkDark;
                    btn[i, j].Size = new Size(60, 60);
                    btn[i, j].Click += new EventHandler(btnClick);
                    this.Controls.Add(btn[i, j]);
                }
            }
        }
        void btnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Text = count++ % 2 == 0 ? "X" : "O";
            btn.Enabled = false;

            if (count >= 4)
                checkGame(btn.Text);
        }

        void checkGame(string s)
        {
            if (btn[0, 0].Text != "" && btn[0, 0].Text == btn[1, 1].Text && btn[0, 0].Text == btn[2, 2].Text ||
                btn[0, 2].Text != "" && btn[0, 2].Text == btn[1, 1].Text && btn[0, 2].Text == btn[2, 0].Text)
            {
                winGame(s);
                return;
            }

            for (int i = 0; i < btn.GetLength(0); i++)
            {
                if (btn[i, 0].Text != "" && btn[i, 0].Text == btn[i, 1].Text && btn[i, 0].Text == btn[i, 2].Text ||
                    btn[0, i].Text != "" && btn[0, i].Text == btn[1, i].Text && btn[0, i].Text == btn[2, i].Text)
                {
                    winGame(s);
                    return;
                }
            }

            if (count == 9)
            {
                MessageBox.Show("平手! 按下確定重新開始", "完局", MessageBoxButtons.OK);
                resetGame();
            }
        }

        void winGame(string s)
        {
            MessageBox.Show(s + " 手獲勝!", "完局!", MessageBoxButtons.OK);
            resetGame();
        }
        void resetGame()
        {
            count = 0;
            for (int i = 0; i < btn.GetLength(0); i++)
            {
                for (int j = 0; j < btn.GetLength(1); j++)
                {
                    btn[i, j].Text = "";
                    btn[i, j].Enabled = true;
                }
            }
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            resetGame();
        }

        private void btnEsc_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
                resetGame();
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
