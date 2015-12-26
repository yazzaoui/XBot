using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MxBots.Panels_de_configuration.ClassesCustomSettings
{
    public partial class GeneralClasses : UserControl
    {
        Classes CurClass;
        Control[] Boxes;
        ArrayList ClassArray;
        public GeneralClasses()
        {
            InitializeComponent();
            //if (ClassSettings.Default.Test == null)
            //{
            //    for (int i = 0; i < (int)Classes.None; i++)
            //    {
            //        ClassSettings.Default.Test[i] = new ArrayList();
            //        ArrayList nbrOpt = (ArrayList)ClassSettings.Default.Test[i];

            //        ArrayList curOptions = new ArrayList();
            //        //
            //    }
            //}
        }

        public void ChangeClass(Classes c)
        {
            //    CurClass = c;
            //    label1.Text = Bot.ClassToString(c) + " General Configuration";
            //    Boxes = new Control[((ArrayList)ClassSettings.Default.Test[(int)c]).Count];
            //    ClassArray = (ArrayList)ClassSettings.Default.Test[(int)c];
            //    int x = 37, y = 51;

            //    for (int i = 0; i < ClassArray.Count; i++)
            //    {
            //        ArrayList CurOptions = (ArrayList)ClassArray[i];
            //        if ((formType)CurOptions[0] == formType.CheckBox)
            //        {
            //            Boxes[i] = new CheckBox();
            //            Boxes[i].Location = new System.Drawing.Point(x, y);

            //        }
            //    }
            //
        }

    }
}
