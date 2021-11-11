using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Create by SKIF4A --> https://github.com/evilSKIF4A

namespace Sapper
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Button[,] playing_field = new Button[8,8];
        private int[,] playing_field_coordinates = new int[8,8]; 
        private Image Sprite;
        private int bomb_before_victory = 0;
        private int bomb_before_victory_score = 0;
        private int all_bomb = 0;
        private Label count_bomb = new Label();
        private Button restart = new Button();
        private Label lose = new Label();
        private Label win = new Label();
        public Form1()
        {
            InitializeComponent();
            Sprite = new Bitmap(@"Sprite.jpg");
            Image picture = new Bitmap(50, 50);
            Graphics gr = Graphics.FromImage(picture);
            Start();
        }
        // Заново
        private void Restart(object sender, EventArgs e)
        {
            bomb_before_victory = 0;
            bomb_before_victory_score = 0;
            all_bomb = 0;
            this.Controls.Remove(count_bomb);
            //this.Controls.Remove(restart);
            this.Controls.Remove(lose);
            this.Controls.Remove(win);
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    playing_field[i, j].Dispose();
            Start();
        }
        // Старт 
        private void Start()
        {
            this.Controls.Clear();
            Random_Coordinates();
            count_bomb.Font = new Font(count_bomb.Font.Name, 20, count_bomb.Font.Style);
            count_bomb.Text = bomb_before_victory.ToString() + " / " + all_bomb.ToString();
            count_bomb.Size = new Size(100, 50);
            count_bomb.ForeColor = Color.Black;
            count_bomb.Location = new Point(500, 70);
            this.Controls.Add(count_bomb);
            restart.Font = new Font(restart.Font.Name, 16, restart.Font.Style);
            restart.Text = "Заново";
            restart.Size = new Size(100, 50);
            restart.ForeColor = Color.Black;
            restart.BackColor = Color.White;
            restart.Location = new Point(500, 130);
            restart.Click += new EventHandler(Restart);
            this.Controls.Add(restart);
            Createmap();
        }
        // рандомное заполнение полей
        private void Random_Coordinates()
        {
            // 0 - пустая клетка
            // 1 - мина
            Random rand_coord = new Random();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    playing_field_coordinates[i, j] = rand_coord.Next(0, 5);
                    if (playing_field_coordinates[i, j] != 1) playing_field_coordinates[i, j] = 0;
                    else all_bomb++;
                }
            }
        }
        // создание игрового поля
        private void Createmap()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    playing_field[i, j] = new Button();
                    Button cage = new Button();
                    cage.Size = new Size(50, 50);
                    cage.Location = new Point(i * 50, j * 50);
                    Image picture = new Bitmap(50, 50);
                    Graphics graphics = Graphics.FromImage(picture);
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 0, 0, 150, 150, GraphicsUnit.Pixel);
                    cage.BackgroundImage = picture;

                    // при нажатии на клетку происходит событие PressFigure
                    cage.MouseDown += new MouseEventHandler(PressCell);
                    //cage.Text = playing_field_coordinates[i, j].ToString();
                    this.Controls.Add(cage);

                    // запоминаем созданную кнопку, чтобы использовать в будущем
                    playing_field[i, j] = cage;
                }
            }
        }
        // нажатие на клетку
        private void PressCell(object sender, MouseEventArgs e)
        {
            count_bomb.Focus();
            Button picture_cage = sender as Button;
            Image picture = new Bitmap(50, 50);
            Graphics graphics = Graphics.FromImage(picture);
            if (e.Button == MouseButtons.Left)
            {
                if(isMin(picture_cage.Location.X/50, picture_cage.Location.Y/50))
                {
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 260, 0, 150, 150, GraphicsUnit.Pixel);
                    picture_cage.BackgroundImage = picture;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if(playing_field_coordinates[i,j] % 10 == 1)
                                playing_field[i, j].BackgroundImage = picture;

                            playing_field[i, j].Enabled = false;
                        }
                    }
                    lose.Font = new Font(lose.Font.Name, 20, lose.Font.Style);
                    lose.Text = "ВЫ ПРОИГРАЛИ!";
                    lose.Size = new Size(300, 50);
                    lose.ForeColor = Color.Red;
                    lose.Location = new Point(400, 200);
                    this.Controls.Add(lose);
                }
                else
                {
                    if(count_min(picture_cage.Location.X / 50, picture_cage.Location.Y / 50) == 0)
                    {
                        open_empty_cage(picture_cage.Location.X / 50, picture_cage.Location.Y / 50, playing_field[picture_cage.Location.X / 50, picture_cage.Location.Y / 50]);
                    }
                    else
                    {
                        picture = draw_picture_count_min(count_min(picture_cage.Location.X / 50, picture_cage.Location.Y / 50));
                        picture_cage.BackgroundImage = picture;
                        picture_cage.Enabled = false;
                    }
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                
                if(playing_field_coordinates[picture_cage.Location.X/50,picture_cage.Location.Y/50] < 10)
                {
                    if(playing_field_coordinates[picture_cage.Location.X / 50, picture_cage.Location.Y / 50] == 1)
                        bomb_before_victory++;
                    playing_field_coordinates[picture_cage.Location.X / 50, picture_cage.Location.Y / 50] += 10;
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 130, 0, 150, 150, GraphicsUnit.Pixel);
                    picture_cage.BackgroundImage = picture;       
                    bomb_before_victory_score++;
                    count_bomb.Text = bomb_before_victory_score + " / " + all_bomb;
                }
                else
                {
                    playing_field_coordinates[picture_cage.Location.X / 50, picture_cage.Location.Y / 50] -= 10;
                    if (playing_field_coordinates[picture_cage.Location.X / 50, picture_cage.Location.Y / 50] == 1)
                        bomb_before_victory--;
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 0, 0, 150, 150, GraphicsUnit.Pixel);
                    picture_cage.BackgroundImage = picture;
                    bomb_before_victory_score--;
                    count_bomb.Text = bomb_before_victory_score + " / " + all_bomb;
                }
                if(bomb_before_victory == all_bomb && bomb_before_victory == bomb_before_victory_score)
                {
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            playing_field[i, j].Enabled = false;
                    win.Font = new Font(win.Font.Name, 20, win.Font.Style);
                    win.Text = "ВЫ ВЫИГРАЛИ!";
                    win.Size = new Size(300, 50);
                    win.ForeColor = Color.Green;
                    win.Location = new Point(400, 200);
                    this.Controls.Add(win);
                }
            }
        }
        // мина или нет
        private bool isMin(int X, int Y)
        {
            if (playing_field_coordinates[X, Y] % 10 == 1) return true;
            return false;
        }
        // количество мин вокруг клетки
        private int count_min(int X, int Y)
        {
            // количество мин вокруг клетки
            int count_min = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i != 0 || j != 0) && X + i >= 0 && X + i <= 7 && Y + j >= 0 && Y + j <= 7 && playing_field_coordinates[X + i, Y + j] % 10 == 1)
                    {
                        count_min++;
                    }
                }
            }
            return count_min;
        }
        // открывает пустые клетки
        private void open_empty_cage(int X, int Y, Button empty_cage)
        {
            if (count_min(X, Y) == 0 && X >= 0 && X <= 7 && Y >= 0 && Y <= 7 && playing_field_coordinates[X,Y] == 0)
            {
                empty_cage.BackgroundImage = draw_picture_count_min(0);
                empty_cage.Enabled = false;
                playing_field_coordinates[X, Y] -= 10;
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if(X+i >= 0 && X+i <= 7 && Y+j >= 0 && Y+j <= 7)
                            open_empty_cage(X + i, Y + j, playing_field[X+i,Y+j]);
            }
            else
            {
                empty_cage.Enabled = false;
                empty_cage.BackgroundImage = draw_picture_count_min(count_min(X, Y));
            }
        }
        // возвращает картинку(пустую или количество мин вокруг клетки)
        private Image draw_picture_count_min(int n)
        {
            Image picture = new Bitmap(50, 50);
            Graphics graphics = Graphics.FromImage(picture);
            switch (n)
            {
                case 0:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 380, 0, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 1:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 0, 120, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 2:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 130, 130, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 3:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 260, 130, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 4:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 390, 130, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 5:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 0, 260, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 6:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 130, 260, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 7:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 260, 260, 150, 150, GraphicsUnit.Pixel);
                    break;
                case 8:
                    graphics.DrawImage(Sprite, new Rectangle(0, 0, 50, 50), 390, 260, 150, 150, GraphicsUnit.Pixel);
                    break;
            }
            return picture;
        }
    }
}
