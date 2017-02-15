using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diagramm
{
    public class DLL_KTPS_Conf
    {
        TMA_Config_MainWindow main;

        public DLL_KTPS_Conf()
        {
            main = new TMA_Config_MainWindow();
        }

        public TMA_Config_MainWindow TMA_Config_MainWindow
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public int Init()
        {
            throw new System.NotImplementedException();
        }

        public int Register()
        {
            throw new System.NotImplementedException();
        }

        public int Action()
        {
            throw new System.NotImplementedException();
        }

        public int Set()
        {
            throw new System.NotImplementedException();
        }

        public int Done()
        {
            throw new System.NotImplementedException();
        }

        public int ProcessMessage()
        {
            throw new System.NotImplementedException();
        }

        public int ProcessNotify()
        {
            throw new System.NotImplementedException();
        }

        public int ProcessTypeNotyfy()
        {
            throw new System.NotImplementedException();
        }

        public void CreateFormat()
        {
            throw new System.NotImplementedException();
        }

        public int ProcessChanel()
        {
            main.UpdateWelcomePhone("name", "ip");
            return 0;
        }

        public int ProcessGate()
        {
            throw new System.NotImplementedException();
        }
    }
}
