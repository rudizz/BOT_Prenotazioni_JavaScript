using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOT_Prenotazioni_JavaScript
{
    class clsBook
    {

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x0800;

        private void DoMouseClick()
        {
            DoMouseLeftClickDown();
            System.Threading.Thread.Sleep(434);
            //Application.DoEvents();
            DoMouseLeftClickUp();
            //Application.DoEvents();

        }
        private void DoMouseLeftClickDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        private void DoMouseLeftClickUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        private void DoScrollWheelDown(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
            {
                mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -120, 0);
                System.Threading.Thread.Sleep(224);
                Application.DoEvents();
            }
        }

        protected WebBrowser web;

        protected const string USERNAME = "Rudi";
        protected const string PWD = "rPZhsBeach2022";
        protected const string MAIN_URL = "https://ssl.forumedia.eu/zhs-courtbuchung.de/reservations.php?action=showReservations&type_id=2&date=";
        protected string URL;
        protected string date;

        protected int start_el;
        protected int end_el;
        protected string booking_page;
        protected string booking_hour1;
        protected string booking_hour2;

        public clsBook(WebBrowser _webBrowser,
                           string _booking_hour1,
                           string _booking_hour2)
        {
            web = _webBrowser;
            booking_hour1 = _booking_hour1;
            booking_hour2 = _booking_hour2;
        }

        private void waitLoading()
        {
            // Attendi il completamento del caricamento della pagina
            while (web.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }

        public bool bookNow(string _date, bool perfomLogIn)
        {
            date = _date;
            URL = MAIN_URL + date + booking_page;

            if (perfomLogIn)
            {
                goToURL();
                logIN();
            }
            goToURL();
            System.Threading.Thread.Sleep(1100);
            if (selectFirstFreeField())
            {
                System.Threading.Thread.Sleep(1100);
                return bookSelectedField();
            }

            return false;
        }

        public void goToURL()
        {

            web.Navigate(URL);
            waitLoading();
        }

        public bool logIN()
        {
            var log = web.Document.GetElementById("login_block");
            if (log == null)
            {
                MessageBox.Show("User already logged in!");
                return false;
            }
            log.InvokeMember("click");
            waitLoading();
            System.Threading.Thread.Sleep(100);

            HtmlElementCollection inputs = web.Document.GetElementsByTagName("input");
            HtmlElement usr = inputs.GetElementsByName("username")[0];
            usr.SetAttribute("value", USERNAME);

            System.Threading.Thread.Sleep(100);

            HtmlElement password = inputs.GetElementsByName("password")[0];
            password.SetAttribute("value", PWD);
            System.Threading.Thread.Sleep(100);

            var btnAnmeldung = web.Document.GetElementsByTagName("input").GetElementsByName("")[0];
            btnAnmeldung.InvokeMember("click");
            
            return true;
        }

        public bool selectFirstFreeField()
        {
            // Indice del campo prenotato, in base 0.
            int indField = 0;
            // Ciclo le checkbox
            for (int el = start_el; el <= end_el; el++)
            {
                var ck1 = web.Document.GetElementById("order_el_" + el + "_" + booking_hour1);
                var ck2 = web.Document.GetElementById("order_el_" + el + "_" + booking_hour2);
                if (ck1 != null && ck1.GetAttribute("checked") == "False" &&
                    (booking_hour2 == "" || (ck2 != null && ck2.GetAttribute("checked") == "False")))
                {
                    ck1.InvokeMember("click");
                    if (booking_hour2 != "")
                    {
                        ck2.InvokeMember("click");
                    }

                    System.Threading.Thread.Sleep(100);
                    // Premo il pulsante 'Buchung' del campo prenotato corrispondente.
                    var btnBuchung = web.Document.GetElementsByTagName("input").GetElementsByName("go");
                    btnBuchung[indField].InvokeMember("click");
                    break;
                }
                indField++;
            }
            waitLoading();
            Application.DoEvents();
            return indField < end_el-start_el+1;
        }

        private bool bookSelectedField()
        {
            while (web.IsBusy)
            {
                Application.DoEvents();
            }
            System.Threading.Thread.Sleep(512);
            moveCursorFromCurrentPositionTo(new Point(321, 88));
            Application.DoEvents();
            DoMouseClick();
            Application.DoEvents();
            DoScrollWheelDown(10);
            //web.Document.InvokeScript("eval", new object[] { "$( document ).scrollTop(1700);" });
            System.Threading.Thread.Sleep(494);
            var btnBestatigen = web.Document.GetElementsByTagName("input").GetElementsByName("process")[0];
            // If button bestatigen doesn't exist, return false
            if (btnBestatigen == null)
                return false;
            Application.DoEvents();


            //moveCursorFromRegistration("cursorPositionsGoToScrollBar.csv");
            //Application.DoEvents();
            //DoMouseLeftClickDown();
            //Application.DoEvents();
            //moveCursorFromRegistration("cursorPositionsDragScrollBar.csv");
            //DoMouseLeftClickUp();
            //Application.DoEvents();
            //registraCursorPosition("cursorPositionsGoToBuchungButton.csv", 4);
            moveCursorFromRegistration("cursorPositions.csv");

            Application.DoEvents();
            //System.Threading.Thread.Sleep(3242);
            //Application.DoEvents();
            DoMouseClick();

            //btnBestatigen.InvokeMember("click");

            return true;
        }

        private void moveCursorFromCurrentPositionTo(Point pEnd)
        {
            // Porto il mouse dalla posizione attuale alla posizione in cui è avvenuta la registrazione
            // del movimento del mouse
            Point pStart = GetCursorPosition();
            Point pCurrent = pStart;
            Random random = new Random();
            while (pCurrent != pEnd)
            {
                pCurrent.X -= Math.Sign(pCurrent.X - pEnd.X);
                pCurrent.Y -= Math.Sign(pCurrent.Y - pEnd.Y);
                SetCursorPos(pCurrent.X, pCurrent.Y);
                System.Threading.Thread.Sleep(random.Next(1, 3));
                Application.DoEvents();
            }

        }

        private void moveCursorFromRegistration(string filename_cursor_position)
        {
            string strPoints = File.ReadAllText(filename_cursor_position);
            char newLine = System.Environment.NewLine.ToCharArray().FirstOrDefault();
            string[] arrPoints = strPoints.Split(newLine);

            moveCursorFromCurrentPositionTo(getPointFromString(arrPoints[0]));

            foreach (var strP in arrPoints)
            {
                string[] p = strP.Replace(System.Environment.NewLine, "").Split(';');
                if (p.Length > 1)
                {
                    SetCursorPos(int.Parse(p[0]), int.Parse(p[1]));
                    System.Threading.Thread.Sleep(1);
                    Application.DoEvents();
                }
            }
        }

        private void registraCursorPosition(string filename_cursor_position, int secondOfAcquisition)
        {

            MessageBox.Show("Registra");
            List<Point> points = new List<Point>();
            DateTime ora = DateTime.Now;
            while (DateTime.Now.Subtract(ora).TotalSeconds < secondOfAcquisition)
            {
                points.Add(GetCursorPosition());
                System.Threading.Thread.Sleep(1);
            }

            System.Threading.Thread.Sleep(100);
            string body = "";
            foreach (var p in points)
            {
                body += p.X + ";" + p.Y + System.Environment.NewLine;
            }
            File.WriteAllText(filename_cursor_position, body);
            MessageBox.Show("Fine registrazione");

        }

        private Point getPointFromString(string strPoint)
        {
            string[] p = strPoint.Replace(System.Environment.NewLine, "").Split(';');
            return new Point(int.Parse(p[0]), int.Parse(p[1]));
        }
    }
}
