using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOT_Prenotazioni_JavaScript
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// Offset di giorni rispetto a oggi in cui si vuole prenotare
        /// </summary>
        private const int DAY_TO_BOOK_FROM_NOW = 8;

        /// <summary>
        /// Gestore per prenotare la pagina 1 dalle 19:00 alle 21:00
        /// </summary>
        clsBookPag1 bookPag1_1900_2100;
        clsBookPag2 bookPag2_1830_2030;
        clsBookPag1 bookPag1_2000_2100;
        clsBookPag2 bookPag2_1930_2030;
        string date;

        public frmMain()
        {
            InitializeComponent();

            // Inizio a cercare di prenotare le ultime due ore, se è tutto
            // occupato cerco di prenotare almeno l'ultima ora
            bookPag1_1900_2100 = new clsBookPag1(_webBrowser, "19:00", "20:00");
            bookPag2_1830_2030 = new clsBookPag2(_webBrowser, "18:30", "19:30");
            bookPag1_2000_2100 = new clsBookPag1(_webBrowser, "20:00", "");
            bookPag2_1930_2030 = new clsBookPag2(_webBrowser, "19:30", "");
        }

        private void btnPrenota_Click(object sender, EventArgs e)
        {

            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = false;

            //Task task = Task.Run(() => 
            //    bookPag1.bookNow()
            //);
            date = DateTime.Now.AddDays(DAY_TO_BOOK_FROM_NOW).ToString("yyyy-MM-dd");
            if (!bookPag2_1830_2030.bookNow(date, true))
                if(!bookPag1_1900_2100.bookNow(date, false)) // se non ho trovato un campo libero alla pag2, provo con la pag1
                    if(!bookPag1_2000_2100.bookNow(date, false))
                        bookPag2_1930_2030.bookNow(date, false);

            System.Threading.Thread.Sleep(5000);
            Application.DoEvents();
            // Alla fine della prenotazione spengo il computer
            shutdownThisComputer(15000);
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            while (now.Hour > 1 || now.Minute > 5)
            {
                System.Threading.Thread.Sleep(5000);
                Application.DoEvents();
                now = DateTime.Now;
            }
            btnPrenota_Click(null, null);
        }

        private void shutdownThisComputer(int wait)
        {
            System.Threading.Thread.Sleep(wait);
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
    }
}
