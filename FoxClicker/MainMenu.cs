using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using WindowsInput.Native;
using WindowsInput;

namespace FoxClicker
{
    public partial class MainMenu : Form
    {
        private void MainMenu_Load(object sender, EventArgs e)
        { }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        //===========================================
        Timer timerClicker; //таймер, просто для примера
        Boolean isStarted = false;

        //Позиция курсора
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        //Для клика по координатам
        [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto,
        CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags,
        int dx,
        int dy,
        int dwData,
        int dwExtraInfo);
        //Нормированные абсолютные координаты
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //Нажатие на левую кнопку мыши
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //Поднятие левой кнопки мыши
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        //перемещение указателя мыши
        private const int MOUSEEVENTF_MOVE = 0x0001;

        globalKeyboardHook gkh = new globalKeyboardHook(); //Новый экз класса
        public MainMenu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //добавляем какие будут горячие клавиши
            gkh.HookedKeys.Add(Keys.F6);
            gkh.HookedKeys.Add(Keys.F7);
            // и т.п.
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
            Class_CursorPosition.MouseHook.MouseAction += new EventHandler(Event);

            openFileDialog_rec.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            saveFileDialog_rec.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
        }

        void startAutoClicker_Norm(int interval) //Запуск кликера в "Обычном режиме"
        {
            timerClicker = new Timer();
            timerClicker.Interval = interval;
            timerClicker.Tick += new EventHandler(timerClicker_Tick);
            timerClicker.Start();
        }
        void startAutoClicker_Interval(int interval_ot, int interval_do) //Запуск кликера в "Режиме интервала"
        {
            Random random = new Random();
            int itog = random.Next(interval_ot, interval_do);
            timerClicker = new Timer();
            timerClicker.Interval = itog;
            timerClicker.Tick += new EventHandler(timerClicker_Tick);
            timerClicker.Start();
        }
        void stopAutoClicker() //Остановка кликера
        {
            timerClicker.Stop();
            if (radioButton2.Checked == true && isStarted == true) { startAutoClicker_Interval(timerOt, timerDo); }
        }
        Int32 timerOt = 0, timerDo = 0;
        Boolean isRec = false; Boolean playIsRec = false; Boolean syhRec = false;
        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6 && isStarted == true && winOpen == false)
            {
                stopAutoClicker();
                isStarted = false;
            }
            else if (e.KeyCode == Keys.F6 && isStarted == false && winOpen == false)
            {
                int timerMM = 0, timerSec = 0, timerMin = 0, timerSum = 0;

                //Проверка на пустоту
                if (radioButton1.Checked == true)
                {
                    if (textBoxMM_Timer.Text == "") { textBoxMM_Timer.Text = "0"; }
                    if (textBoxSec_Timer.Text == "") { textBoxSec_Timer.Text = "0"; }
                    if (textBoxMin_Timer.Text == "") { textBoxMin_Timer.Text = "0"; }
                    //Конвертиция для суммы времени
                    timerMM = Convert.ToInt32(textBoxMM_Timer.Text);
                    timerSec = Convert.ToInt32(textBoxSec_Timer.Text);
                    timerMin = Convert.ToInt32(textBoxMin_Timer.Text);
                    timerSum = timerMM + (timerSec * 1000) + (timerMin * 60000);

                    if (timerSum != 0) { startAutoClicker_Norm(timerSum); isStarted = true; }
                    else { MessageBox.Show("Недопутимый аргумент!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); ; }
                }
                else if (radioButton2.Checked == true)
                {
                    if (textBoxInterval_Ot.Text == "") { textBoxInterval_Ot.Text = "0"; }
                    if (textBoxInterval_Do.Text == "") { textBoxInterval_Do.Text = "0"; }
                    //Конвертиция для суммы времени
                    timerOt = Convert.ToInt32(textBoxInterval_Ot.Text);
                    timerDo = Convert.ToInt32(textBoxInterval_Do.Text);
                    if (timerOt >= timerDo) { MessageBox.Show("Первое значение не может быть больше второго!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return; }
                    if (timerOt != 0 && timerDo != 0) { startAutoClicker_Interval(timerOt, timerDo); isStarted = true; }
                    else { MessageBox.Show("Недопутимый аргумент!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                }
                else if (radioButton3.Checked == true)
                {
                    if (textBox3MM_Timer.Text == "") { textBox3MM_Timer.Text = "0"; }
                    if (textBox3Sec_Timer.Text == "") { textBox3Sec_Timer.Text = "0"; }
                    if (textBox3Min_Timer.Text == "") { textBox3Min_Timer.Text = "0"; }
                    //Конвертиция для суммы времени
                    timerMM = Convert.ToInt32(textBox3MM_Timer.Text);
                    timerSec = Convert.ToInt32(textBox3Sec_Timer.Text);
                    timerMin = Convert.ToInt32(textBox3Min_Timer.Text);
                    timerSum = timerMM + (timerSec * 1000) + (timerMin * 60000);

                    if (timerSum != 0 && textBoxCursorIntervalX.Text != "X" && textBoxCursorIntervalY.Text != "Y") { startAutoClicker_Norm(timerSum); isStarted = true; }
                    else { MessageBox.Show("Недопутимый аргумент!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                }
                else if (radioButton4.Checked == true) 
                {
                    if (syhRec == true)
                    {
                        if (playIsRec == false && isRec == false) { playIsRec = true; PlayRec(); timer4.Start(); }
                        else { playIsRec = false; timer4.Stop(); mm2 = 0; IArray = 0; arrayLenghTimer = 0; arrayLengh = 0; }
                    }
                    else { MessageBox.Show("Вначале задайте координаты (F7)!", "!ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
            }
            if (e.KeyCode == Keys.F7)
            {
                if (radioButton3.Checked == true)
                {
                    if (isCursorPosition == false && radioButton3.Checked == true) { timer2.Start(); isCursorPosition = true; buttonCursorPosition.Text = "Скрыть координаты (F7)"; }
                    else if (isCursorPosition == true) { timer2.Stop(); isCursorPosition = false; buttonCursorPosition.Text = "Показать координаты (F7)"; }
                }
                else if (radioButton4.Checked == true)
                {
                    if (isRec == false && playIsRec == false)
                    {
                        Class_CursorPosition.MouseHook.Start(); arrayLenghTimer = 0; IArray = 0; isRec = true; timer3.Enabled = true; labelTimeRec.Text = "0"; mm = 0; dataGridViewCursorLocation.Rows.Clear(); kolKlickov = 0; syhRec = true;
                    }
                    else { Class_CursorPosition.MouseHook.stop(); isRec = false; timer3.Enabled = false; }
                }
            }
        }

        Int32 klick = 0; //Количество кликов
        void timerClicker_Tick(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true && winOpen == false)
            {
                //Вызов импортируемой функции с текущей позиции курсора
                int X = (int)Cursor.Position.X;
                int Y = (int)Cursor.Position.Y;
                DoMouseClick(X, Y);
                labelKolKlikov.Text = $"{klick++}";
            }
            else if (radioButton2.Checked == true && winOpen == false)
            {
                //Вызов импортируемой функции с текущей позиции курсора
                int X = (int)Cursor.Position.X;
                int Y = (int)Cursor.Position.Y;
                DoMouseClick(X, Y);
                labelKolKlikov.Text = $"{klick++}";
                stopAutoClicker();
            }
            else if (radioButton3.Checked == true && winOpen == false)
            {
                int X = Convert.ToInt32(textBoxCursorIntervalX.Text);
                int Y = Convert.ToInt32(textBoxCursorIntervalY.Text);

                //Перемещение курсора на указанные координаты
                Cursor.Position = new Point(X, Y);
                DoMouseClick(Convert.ToInt32(X), Convert.ToInt32(Y));
                labelKolKlikov.Text = $"{klick++}";
            }
        }
        public void DoMouseClick(int X, int Y)
        {
            if (winOpen == false) { mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), 4700, 62000, 0, UIntPtr.Zero); }
        }

        //Проверка активного окна
        bool winOpen = true;
        private void Form1_Activated(object sender, EventArgs e) { winOpen = true; }
        private void Form1_Deactivate(object sender, EventArgs e) { winOpen = false; }

        //Переключение режимов
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        { if (radioButton1.Checked == true) { groupBox2.Enabled = false; groupBox3.Enabled = false; groupBox1.Enabled = true; groupBox4.Enabled = false; groupBox5.Enabled = false; radioButton2.Checked = false; radioButton3.Checked = false; radioButton4.Checked = false; } }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        { if (radioButton2.Checked == true) { groupBox1.Enabled = false; groupBox3.Enabled = false; groupBox2.Enabled = true; groupBox4.Enabled = false; groupBox5.Enabled = false; radioButton1.Checked = false; radioButton3.Checked = false; radioButton4.Checked = false; } }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        { if (radioButton3.Checked == true) { groupBox1.Enabled = false; groupBox2.Enabled = false; groupBox3.Enabled = true; groupBox4.Enabled = false; groupBox5.Enabled = false; radioButton1.Checked = false; radioButton2.Checked = false; radioButton4.Checked = false; } }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        { if (radioButton4.Checked == true) { groupBox1.Enabled = false; groupBox2.Enabled = false; groupBox3.Enabled = false; groupBox4.Enabled = true; groupBox5.Enabled = true; radioButton1.Checked = false; radioButton2.Checked = false; radioButton3.Checked = false; } }

        //Показывать координаты курсора
        private void timer2_Tick(object sender, EventArgs e)
        {
            Point pt = new Point();
            GetCursorPos(ref pt);
            labelCursorPosition.Text = $"X:{pt.X.ToString()} Y:{pt.Y.ToString()}";
            textBoxCursorIntervalX.Text = $"{pt.X}"; textBoxCursorIntervalY.Text = $"{pt.Y}";
        }

        //Кнопка показа координат курсора
        Boolean isCursorPosition = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (isCursorPosition == false) { timer2.Start(); isCursorPosition = true; buttonCursorPosition.Text = "Скрыть координаты (F7)"; }
            else if (isCursorPosition == true) { timer2.Stop(); isCursorPosition = false; buttonCursorPosition.Text = "Показать координаты (F7)"; }
        }

        //События нажатия кнопки мыши для REC
        int kolKlickov = 0;
        private void Event(object sender, EventArgs e)
        {
            Point pt = new Point();
            GetCursorPos(ref pt);
            dataGridViewCursorLocation.Rows.Add(mm, pt.X, pt.Y);
            kolKlickov++;
            labelClickRec.Text = kolKlickov.ToString();
        }

        //Кнопка очистки количества кликов
        private void button1_Click(object sender, EventArgs e) { klick = 0; labelKolKlikov.Text = "0"; }

        //Секундомер для записи
        uint mm = 0;
        private void timer3_Tick(object sender, EventArgs e)
        {
            mm++;
            labelTimeRec.Text = mm.ToString();
        }

        //Запись
        List<string> tableTime = new List<string>();
        List<string> tableX = new List<string>();
        List<string> tableY = new List<string>();
        int numberColumn = 0;
        Int32 arrayLengh;
        Boolean restartRec = false;
        public void PlayRec()
        {
            tableTime.Clear(); tableX.Clear(); tableY.Clear();
            labelClickRec.Text = "0";
            numberColumn = 0;
            if ( restartRec == false)
            {
                for (int col = 0; col < dataGridViewCursorLocation.Rows.Count; col++)
                {
                    for (int rows = 0; rows < dataGridViewCursorLocation.Rows.Count; rows++)
                    {
                        if (numberColumn == 0) { tableTime.Add(dataGridViewCursorLocation[numberColumn, rows].Value.ToString()); }
                        if (numberColumn == 1) { tableX.Add(dataGridViewCursorLocation[numberColumn, rows].Value.ToString()); }
                        if (numberColumn == 2) { tableY.Add(dataGridViewCursorLocation[numberColumn, rows].Value.ToString()); }
                    }
                    numberColumn++;
                }
                timer4.Start();
                restartRec = true;
            }
            arrayLengh = tableTime.Count;
            restartRec = false;
        }

        //Таймер для воспроизвдения записи
        uint mm2 = 0;
        int IArray = 0; int arrayLenghTimer = 0;
        private void timer4_Tick(object sender, EventArgs e)
        {
            labelTimeRec.Text = mm2.ToString();
            if (arrayLenghTimer < arrayLengh && mm2 == Convert.ToInt32(tableTime[IArray]))
            {
                Cursor.Position = new Point(Convert.ToInt32(tableX[IArray]), Convert.ToInt32(tableY[IArray]));
                DoMouseClick(Convert.ToInt32(tableX[IArray]), Convert.ToInt32(tableY[IArray]));
                IArray++;
                arrayLenghTimer++;
                labelClickRec.Text = $"{IArray.ToString()}";
                labelKolKlikov.Text = $"{klick++}";
            }
            else if (arrayLenghTimer >= arrayLengh)
            {
                mm2 = 0; arrayLenghTimer = 0;
                timer4.Stop();
                restartTimer4();
            }
            else { mm2++; }
        }

        //Сохранить текущую таблицу записи в файл
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_rec.ShowDialog() == DialogResult.Cancel) return;

            string fileName = saveFileDialog_rec.FileName;
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fs);

            try
            {
                for (int i = 0; i < dataGridViewCursorLocation.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridViewCursorLocation.Rows[i].Cells.Count; j++)
                    {
                        streamWriter.Write(dataGridViewCursorLocation.Rows[i].Cells[j].Value + " ");
                    }
                    streamWriter.WriteLine();
                }

                streamWriter.Close();
                fs.Close();

                MessageBox.Show("Данные успешно сохранены", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Что-то пошло не так!", "Сохранение - ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Восстановить записи из файла
        private void loadButton_Click(object sender, EventArgs e)
        {
            dataGridViewCursorLocation.Rows.Clear();
            if (openFileDialog_rec.ShowDialog() == DialogResult.Cancel) return;

            string fileName = openFileDialog_rec.FileName;

            try
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    var array = line.Split();
                    dataGridViewCursorLocation.Rows.Add(array);
                }

                tableTime.Clear(); tableX.Clear(); tableY.Clear();
                foreach (DataGridViewRow item in dataGridViewCursorLocation.Rows)
                {
                    if (item.Cells.Count >= 2 && //atleast two columns
                        item.Cells[1].Value != null) //value is not null
                    {
                        tableTime.Add(item.Cells[0].Value.ToString());
                        tableX.Add(item.Cells[1].Value.ToString());
                        tableY.Add(item.Cells[2].Value.ToString());
                    }
                }

                //Удаление пустых строк
                for (int i = 0; i < dataGridViewCursorLocation.RowCount - 1; i++)
                {
                    if (dataGridViewCursorLocation.Rows[i].Cells[1].Value.ToString() == "")
                    {
                        dataGridViewCursorLocation.Rows.RemoveAt(i);
                        i--;
                    }
                }
                syhRec = true;
                MessageBox.Show("Данные из файла успешно загружены", "Восстановление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                tableTime.Clear(); tableX.Clear(); tableY.Clear(); dataGridViewCursorLocation.Rows.Clear();
                MessageBox.Show("Файл неподходящего типа!", "Восстановление - ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void restartTimer4()
        {
            mm2 = 0; IArray = 0; arrayLenghTimer = 0;
            timer4.Start(); 
        }

        //Кастомные кнопки закрыть, свернуть, переместить
        private void pictureBox8_Click(object sender, EventArgs e) //Закрыть
        {
            Application.Exit();
        }
        private void pictureBox9_Click(object sender, EventArgs e) //Свернуть
        {
            WindowState = FormWindowState.Minimized;
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void label13_MouseDown(object sender, MouseEventArgs e) //Заголовок
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        private void pictureBox11_MouseDown(object sender, MouseEventArgs e) //Картинка заголовка
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void helpLabel_Click(object sender, EventArgs e)
        {
            HelpMenu forma = new HelpMenu();
            forma.ShowDialog();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            HelpMenu forma = new HelpMenu();
            forma.ShowDialog();
        }

        private void pictureBox10_MouseDown(object sender, MouseEventArgs e) //Полоса
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
    }
}