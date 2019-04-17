using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilot_Study
{
   
    public class AlertMessage
    {
        private int certainty;
        private string message;
        private bool isCorrect;

        public AlertMessage(int certainty, string message, bool isCorrect)
        {
            this.certainty = certainty;
            this.message = message;
            this.isCorrect = isCorrect;
        }

        public int getCertainty()
        {
            return this.certainty;
        }

        public string getMessage()
        {
            return this.message;
        }

        public bool isAccurate()
        {
            return this.isCorrect;
        }



    }
}
