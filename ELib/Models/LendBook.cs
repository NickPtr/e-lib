using System;
using System.Collections.Generic;
using System.Text;

namespace ELib.Models
{
    class LendBook
    {
        public PrintBook Book { get; set; }

        public DateTime ReturnDate { get; set; }

        public string Username { get; set; }
    }
}
