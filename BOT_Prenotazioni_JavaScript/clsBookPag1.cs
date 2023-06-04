using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOT_Prenotazioni_JavaScript
{
    /// <summary>
    /// Field 1 - 7.
    /// Time xx:00
    /// </summary>
    class clsBookPag1 : clsBook
    {
        

        public clsBookPag1(WebBrowser _webBrowser,
                           string _booking_hour1,
                           string _booking_hour2) : base(_webBrowser,
                                                         _booking_hour1,
                                                         _booking_hour2)
        {
            booking_page = "&page=1";
            start_el = 22;
            end_el = 28;
        }
    }
}
