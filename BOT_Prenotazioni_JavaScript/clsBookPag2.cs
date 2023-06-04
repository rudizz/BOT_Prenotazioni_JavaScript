using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOT_Prenotazioni_JavaScript
{

    /// <summary>
    /// Field 8 - 13.
    /// Time xx:30
    /// </summary>
    class clsBookPag2 : clsBook
    {

        public clsBookPag2(WebBrowser _webBrowser,
                           string _booking_hour1,
                           string _booking_hour2) : base(_webBrowser,
                                                         _booking_hour1,
                                                         _booking_hour2)
        {
            booking_page = "&page=2";
            start_el = 51;
            end_el = 56;
        }
    }
}
